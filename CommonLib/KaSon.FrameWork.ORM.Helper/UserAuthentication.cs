using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityModel;
using EntityModel.CoreModel;
using EntityModel.Enum;
using KaSon.FrameWork.Common;

namespace KaSon.FrameWork.ORM.Helper
{
    public class UserAuthentication : DBbase
    {
        private static List<C_Auth_MethodFunction_List> _allMethodFunctionList = new List<C_Auth_MethodFunction_List>();
        /// <summary>
        /// 验证用户是否具有该方法的权限
        /// </summary>
        public string ValidateUserAuthentication(string userToken)
        {
            if (_allMethodFunctionList == null || _allMethodFunctionList.Count == 0)
            {

                _allMethodFunctionList = DB.CreateQuery<C_Auth_MethodFunction_List>().ToList();
            }

            var method = new System.Diagnostics.StackFrame(1).GetMethod();
            var currentFullName = string.Format("{0}.{1}", method.ReflectedType.FullName, method.Name);
            var config = _allMethodFunctionList.Where(p => p.MethodFullName == currentFullName).FirstOrDefault();
            if (config == null)
                throw new Exception(string.Format("没有配置方法 {0} 的调用权限数据", currentFullName));

            var userId = string.Empty;
            ValidateAuthentication(userToken, config.Mode, config.FunctionId, out userId);
            return userId;
        }
        /// <summary>
        /// 验证用户身份及权限，并返回用户名称
        /// </summary>
        public static void ValidateAuthentication(string userToken, string needRight, string functionId, out string userId)
        {
            try
            {
                var rlt = ValidateAuthentication(userToken, needRight, functionId, "LI");
                if (!rlt.ContainsKey("LI"))
                {
                    throw new Exception("UserToken不完整，缺少UserId信息");
                }
                userId = rlt["LI"];
            }
            catch (Exception ex)
            {
                throw new Exception("用户身份验证失败，请检查是否已登录", ex);
            }
        }
        /// <summary>
        /// 验证用户身份及权限，并返回用户名称
        /// </summary>
        public static Dictionary<string, string> ValidateAuthentication(string userToken, string needRight, string functionId, params string[] keys)
        {
            var dic = UserTokenHandler.AnalyzeUserToken(userToken);
            if (!dic.ContainsKey("IA") || dic["IA"] != "IA")
            {
                if (!dic[functionId].Contains(needRight))
                {
                    throw new Exception("权限不足");
                }
            }
            var rlt = new Dictionary<string, string>(keys.Length);
            foreach (var key in keys)
            {
                rlt.Add(key, dic[key]);
            }
            return rlt;
        }


        #region 获取游客权限
        private static string _guestToken = null;
        public string GetGuestToken()
        {
            if (string.IsNullOrEmpty(_guestToken))
            {
                IList<AccessControlItem> acl = new List<AccessControlItem>();
                var role = GetGuestRole();
                MergeRoleAccessControlList(ref acl, role);
                _guestToken = GetUserToken(role.RoleId, acl);
            }
            return _guestToken;
        }
        private static SystemRole _guestRole = null;
        private SystemRole GetGuestRole()
        {
            if (_guestRole == null)
            {
                string roleId = "Guest";
                if (ConfigHelper.AllConfigInfo != null && ConfigHelper.AllConfigInfo["GuestRoleId"] != null)
                {
                    roleId = ConfigHelper.AllConfigInfo["GuestRoleId"].ToString();
                }
                _guestRole = GetRoleById(roleId);
                //_guestRole.FunctionList = new List<RoleFunction>();
            }
            return _guestRole;
        }

        public SystemRole GetRoleById(string roleId)
        {
            var model = DB.CreateQuery<C_Auth_Roles>().Where(p => p.RoleId == roleId).ToList().Select(p => new SystemRole
            {
                IsAdmin = p.IsAdmin,
                IsInner = p.IsInner,
                RoleId = p.RoleId,
                RoleName = p.RoleName,
                RoleType = (RoleType)p.RoleType,

            }).ToList().FirstOrDefault();
            if (model != null)
            {
                model.FunctionList = DB.CreateQuery<C_Auth_RoleFunction>().Where(p => p.RoleId == roleId).Select(p => new RoleFunction()
                {
                    FunctionId = p.FunctionId,
                    IId = p.IId,
                    Mode = p.Mode,
                    Status = (EnableStatus)p.Status
                }).ToList();
            }
            return model;
            //return DB <SystemRole>(roleId);
        }

        private void MergeRoleAccessControlList(ref IList<AccessControlItem> acl, SystemRole role)
        {
            if (role.ParentRole != null)
            {
                MergeRoleAccessControlList(ref acl, role.ParentRole);
            }
            acl = MergeAccessControlList<AccessControlItem, RoleFunction>(acl, role.FunctionList);
        }


        // 合并两个控制访问列表。T2覆盖T1的列表
        public IList<AccessControlItem> MergeAccessControlList<T1, T2>(IList<T1> acl1, IList<T2> acl2)
            where T1 : AccessControlItem
            where T2 : AccessControlItem
        {
            if (acl1.Where(a => a.Status != EnableStatus.Enable).Count() > 0)
            {
                throw new Exception("被合并的访问控制列表的原始表出现了禁用项");
            }
            if (acl1.Where(a => a.Mode.Length < 1 || a.Mode.Length > 2 || (a.Mode != "R" && a.Mode != "W" && a.Mode != "RW")).Count() > 0)
            {
                throw new Exception("被合并的访问控制列表的原始表出现了无效的Mode值，只允许R、W或RW");
            }
            if (acl2.Where(a => a.Mode.Length < 1 || a.Mode.Length > 2 || (a.Mode != "R" && a.Mode != "W" && a.Mode != "RW")).Count() > 0)
            {
                throw new Exception("被合并的访问控制列表的目标表出现了无效的Mode值，只允许R、W或RW");
            }
            var result = new List<AccessControlItem>();
            if (acl2.Count == 0)
            {
                result.AddRange(acl1);
                return result;
            }
            foreach (var t2 in acl2)
            {
                if (t2.Status == EnableStatus.Enable)
                {
                    result.Add(t2);
                }
                else if (t2.Status == EnableStatus.Disable)
                {
                    var tmp = FindItem<T1>(acl1, t2.FunctionId);
                    if (tmp != null)
                    {
                        if (tmp.Mode == t2.Mode || t2.Mode.Contains(tmp.Mode))
                        {
                            acl1.Remove(tmp);
                        }
                        else if (tmp.Mode.Contains(t2.Mode))
                        {
                            tmp.Mode = (t2.Mode == "R" ? "W" : "R");
                        }
                    }
                }
                else
                {
                    throw new Exception("访问控制项状态为未知");
                }
            }
            foreach (var t1 in acl1)
            {
                if (!IsContains<AccessControlItem>(result, t1))
                {
                    result.Add(t1);
                }
            }
            return result;
        }

        private T FindItem<T>(IList<T> acl, string functionId)
            where T : AccessControlItem
        {
            foreach (var t in acl)
            {
                if (t.FunctionId == functionId)
                {
                    return t;
                }
            }
            return null;
        }

        private bool IsContains<T>(IList<T> acl, AccessControlItem item)
            where T : AccessControlItem
        {
            foreach (var t in acl)
            {
                if (t.Equals(item))
                {
                    return true;
                }
            }
            return false;
        }

        public string GetUserToken(string userId, IList<AccessControlItem> acl)
        {
            var dic = new Dictionary<string, string>(acl.Count);
            foreach (var item in acl)
            {
                if (item.Status != EnableStatus.Enable)
                {
                    throw new Exception("被禁止的权限控制项不能出现在此");
                }
                if (!dic.ContainsKey(item.FunctionId))
                {
                    dic.Add(item.FunctionId, item.Mode);
                }
                else
                {
                    dic[item.FunctionId] = MergeFunctionMode(dic[item.FunctionId], item.Mode);
                }
            }
            dic.Add("LI", userId);
            return UserTokenHandler.GetUserToken(dic);
        }

        private string MergeFunctionMode(string mode1, string mode2)
        {
            return new string(mode1.Union(mode2).ToArray());
        }
        #endregion
    }
    public static class UserTokenHandler
    {
        private const CryptType _type = CryptType.Rijndael;
        private const string _key = "S312ifM8gM5LnWS3nK+6MI+33nqTJUPu+EONiDVZ4oY=";
        private const string _iv = "r+bveoBETg6ENOitGw+4IQ==";
        private const char _spliter_outer = (char)20;
        private const char _spliter_inner = (char)21;
        public static string GetUserToken(Dictionary<string, string> acl)
        {
            List<string> tokenList = new List<string>();
            foreach (var key in acl.Keys)
            {
                tokenList.Add(string.Format("{1}{0}{2}", _spliter_inner, key, acl[key]));
            }
            var userToken = string.Join(_spliter_outer.ToString(), tokenList.ToArray());
            SymmetricCrypt crypt = new SymmetricCrypt(_type);
            return crypt.Encrypt(userToken, _key, _iv);
        }
        public static bool IsAdmin(string userToken)
        {
            var dic = AnalyzeUserToken(userToken);
            return dic.ContainsKey("IA") && dic["IA"] == "IA";
        }
        /// <summary>
        /// 验证用户身份及权限，并返回用户名称
        /// </summary>
        public static Dictionary<string, string> ValidateAuthentication(string userToken, string needRight, string functionId, params string[] keys)
        {
            var dic = AnalyzeUserToken(userToken);
            if (!dic.ContainsKey("IA") || dic["IA"] != "IA")
            {
                if (!dic[functionId].Contains(needRight))
                {
                    throw new Exception("权限不足");
                }
            }
            var rlt = new Dictionary<string, string>(keys.Length);
            foreach (var key in keys)
            {
                rlt.Add(key, dic[key]);
            }
            return rlt;
        }
        public static Dictionary<string, string> AnalyzeUserToken(string userToken)
        {
            SymmetricCrypt crypt = new SymmetricCrypt(_type);
            userToken = crypt.Decrypt(userToken, _key, _iv);
            var tokenList = userToken.Split(_spliter_outer);

            var dic = new Dictionary<string, string>();
            foreach (var item in tokenList)
            {
                var tmp = item.Split(_spliter_inner);
                if (tmp.Length == 2)
                {
                    dic.Add(tmp[0], tmp[1]);
                }
                else
                {
                    throw new ArgumentException("口令格式错误 - " + item);
                }
            }
            return dic;
        }
        private static int GetRandom(int min, int max)
        {
            var r = new Random(Guid.NewGuid().GetHashCode());
            return r.Next(min, max);
        }

    }


}

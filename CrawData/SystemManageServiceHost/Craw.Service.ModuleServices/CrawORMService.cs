using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using KaSon.FrameWork.ORM.Helper.WinNumber;
using KaSon.FrameWork.Common;
using KaSon.FrameWork.Common.Net;
using KaSon.FrameWork.ORM.Helper.BonusPool;
using EntityModel.BonusPool;
using MongoDB.Driver;
using KaSon.FrameWork.Common.JSON;
using MongoDB.Bson;
using KaSon.FrameWork.ORM.Provider;
using EntityModel;
using KaSon.FrameWork.ORM.Helper;
using EntityModel.Communication;

namespace Craw.Service.ModuleServices
{
    /// <summary>
    /// 采集到数据要做处理
    /// </summary>
    public class CrawORMService
    {
        private IMongoDatabase mDB;
        public CrawORMService(IMongoDatabase _mDB)
        {
            mDB = _mDB;
        }
        /// <summary>
        /// 数字彩录入
        /// </summary>
        /// <param name="gameName"></param>
        /// <param name="all"></param>
        /// <param name="dic"></param>
        /// <returns></returns>
        public bool Start(string gameName, ConcurrentDictionary<string, string> all, Dictionary<string, string> dic)
        {


            if (all == null) return false;

            var query = from q in dic where !all.ContainsKey(q.Key) select q;
            if (query.Count() > 0)
            {
                foreach (var item in query)//
                {

                    // 采集到数据
                    //导入数据
                    ILotteryDataBusiness instan = KaSon.FrameWork.ORM.Helper.WinNumber.LotteryDataBusiness.GetTypeImport(gameName);// = new LotteryDataBusiness();
                    instan.ImportWinNumber(item.Key, item.Value);

                    //奖期派奖
                    new KJGameIssuseBusiness().IssusePrize(gameName, item.Key, item.Value);
                }
                //生成相关静态数据
                try
                {
                    //step 3 生成相关静态数据
                    var dpc = new string[] { "FC3D", "PL3", "SSQ", "DLT" };
                    //  this.WriteLog("开始生成静态相关数据.");

                    //this.WriteLog("1.生成最新开奖号");
                    //var log = this.SendBuildStaticFileNotice("401", gameName);
                    //// this.WriteLog("1.生成最新开奖号结果:" + log);

                    ////if (dpc.Contains(gameCode))
                    ////{
                    ////  this.WriteLog("2.生成开奖结果首页");
                    //log = this.SendBuildStaticFileNotice("301");
                    //// this.WriteLog("2.生成开奖结果首页结果：" + log);
                    ////}

                    ////  this.WriteLog("3.生成彩种开奖历史");
                    //log = this.SendBuildStaticFileNotice("302", gameName);
                    ////    this.WriteLog("3.生成彩种开奖历史结果：" + log);

                    ////  this.WriteLog("4.生成彩种开奖详细");
                    //log = this.SendBuildStaticFileNotice("303", gameName);
                    ////   this.WriteLog("4.生成彩种开奖详细结果：" + log);

                    //if (dpc.Contains(gameName))
                    //{
                    //    //   this.WriteLog("5.生成网站首页");
                    //    log = this.SendBuildStaticFileNotice("10");
                    //    //  this.WriteLog("5.生成网站首页结果：" + log);
                    //}

                    //// this.WriteLog("6.生成走势图");
                    //log = this.SendBuildStaticFileNotice("900", gameName);
                    //  this.WriteLog("6.生成走势图结果：" + log);

                    //  this.WriteLog("生成静态相关数据完成.");
                }
                catch (Exception ex)
                {
                    // this.WriteLog("生成静态数据异常：" + ex.Message);
                    return false;
                }

                return true;
            }
            return false;
        }

        /// <summary>
        /// 奖金池录入
        /// </summary>
        /// <param name="gameName"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        public bool BonusPoolStart(string gameName, OpenDataInfo info)
        {
            bool bol = true;
            string tablename = Lottery.CrawGetters.InitConfigInfo.MongoSettings["BonusPoolTableName"].ToString();
            string content = KaSon.FrameWork.Common.JSON.JsonHelper.Serialize(info);
            BsonDocument bson = new BsonDocument();
            bson.Add("GameCode", info.GameCode);
            bson.Add("IssuseNumber", info.IssuseNumber);
            bson.Add("Content", content);
            //  new BsonDocument { { "GameCode", info.GameCode }, { "Age", 20 } }
            var fileName = string.Format("{0}_{1}.json", info.GameCode, info.IssuseNumber);
            //  this.WriteLog(string.Format("已成功采集到{0}第{1}期奖池数据，开始写入文件{2}", info.GameCode, info.IssuseNumber, fileName));
            var coll = mDB.GetCollection<BsonDocument>(tablename);
            //var options = new UpdateOptions { IsUpsert = true };
            var mFilter = MongoDB.Driver.Builders<MongoDB.Bson.BsonDocument>.Filter.Eq("GameCode", info.GameCode) & Builders<BsonDocument>.Filter.Eq("IssuseNumber", info.IssuseNumber);
            // var mUpdateDocument =Builders<MongoDB.Bson.BsonDocument>.Update.Set("Content", content);

            var count = coll.Find(mFilter).CountDocuments();
            try
            {
                if (count > 0)
                {
                    //Thread.Sleep(2000);

                }
                else
                {
                    coll.DeleteMany(mFilter);
                    coll.InsertOne(bson);
                }
                try
                {
                    BonusPoolManager bm = new BonusPoolManager(content);
                    bm.UpdateBonusPool_SZC(info.GameCode, info.IssuseNumber);
                }
                catch (Exception)
                {

                    bol = false;
                }
            }
            catch (Exception)
            {

                bol = false;
            }





            return bol;
        }

        /// <summary>
        /// 发送通知到网站生成静态页或静态数据
        /// </summary>
        public string SendBuildStaticFileNotice(string pageType, string key = "")
        {


            var result = new List<string>();
            var urlArray = Lottery.CrawGetters.InitConfigInfo.BuildStaticFileSendUrl.Split('|'); //ConfigurationManager.AppSettings["BuildStaticFileSendUrl"].Split('|');
            var code = Encipherment.MD5(string.Format("Home_BuildSpecificPage_{0}", pageType), Encoding.UTF8);
            foreach (var item in urlArray)
            {
                var fullUrl = string.Format("{0}/StaticHtml/BuildSpecificPage?pageType={1}&code={2}&key={3}", item, pageType, code, key);
                result.Add(PostManager.Get(fullUrl, Encoding.UTF8));
            }
            return string.Join(Environment.NewLine, result.ToArray());
        }
        public bool HK6IssuseStart(List<string> list)
        {
            var db = new DbProvider();

            //// db.Init("Default");
            try
            {
                db.Init("MySql.Default", false);
                list = (from items in list orderby items select items).ToList();
                DateTime oneDate = DateTime.Parse(list[0]);
                string Year = oneDate.Year + "";
                string atcNo = "";
                int index = 73;
                var timelist = db.CreateQuery<blast_lhc_time>().ToList(); ;
                foreach (var item in list)
                {
                    if (index > 142)
                    {
                        Console.WriteLine(item);
                    }

                    if (DateTime.Parse(item).Year != oneDate.Year)
                    {
                        index = 1;
                        Year = DateTime.Parse(item).Year + "";
                        oneDate = DateTime.Parse(item);
                    }
                    atcNo = index + "";
                    atcNo = Year + "" + index;
                    if (index < 10)
                    {
                        atcNo = Year + "00" + index;
                    }
                    if (index >= 10 && index < 100)
                    {
                        atcNo = Year + "0" + index;
                    }
                    if (index > 100)
                    {
                        atcNo = Year + "" + index;
                    }
                    DateTime actionTime = DateTime.Parse(item);
                    var one = timelist.Where(b => b.actionTime == actionTime).FirstOrDefault();
                    if (one == null)
                    {
                        blast_lhc_time b = new blast_lhc_time()
                        {
                            // actionDate = item,
                            typeid = 1,
                            actionNo = int.Parse(atcNo),
                            actionTime = DateTime.Parse(item),
                            stopTime = DateTime.Parse(DateTime.Parse(item).ToShortDateString()).AddHours(21).AddMinutes(30)

                        };
                        db.GetDal<blast_lhc_time>().Add(b);
                    }
                    else
                    {
                        //db.GetDal<blast_lhc_time>().Update(b => new blast_lhc_time
                        //{
                        //    actionNo = int.Parse(atcNo)
                        //}, b => b.actionTime == actionTime);
                    }

                    index++;
                }
            }
            catch (Exception)
            {
                db.Dispose();
                throw;
            }
            finally
            {
                db.Dispose();
            }






            return true;
        }


        public bool HK6HostoryNum(List<blast_data> list)
        {
            var db = new DbProvider();
            //// db.Init("Default");
            try
            {
                db.Init("MySql.Default", false);
                var datalist = db.CreateQuery<blast_data>().ToList();
                foreach (var item in list)
                {
                    if (item.issueNo < 2000)
                    {
                        //加上2018
                        item.issueNo = int.Parse(item.kjtime.Year + "" + item.issueNo);

                    }
                    var one = datalist.Where(b => b.issueNo == item.issueNo).FirstOrDefault();



                    if (one == null)
                    {

                        string str = item.kjdata;
                        if (str.Contains(",") && !str.Contains("+"))
                        {
                            int index = str.LastIndexOf(',');
                            string temp = str.Substring(0, index);
                            string temp1 = str.Substring(index);
                            item.kjdata = temp + "+" + temp1.Replace(",", "");
                        }
                        item.typeid = 1;
                        db.GetDal<blast_data>().Add(item);
                    }
                    else
                    {
                        //db.GetDal<blast_data_time>().Update(b=>new blast_data_time {
                        //     actionNo
                        //},b=>b.actionDate==item);
                    }

                    // index++;
                }
                if (list.Count > 0)
                {
                    var one = list[0];
                    DateTime kjtime = one.kjtime;
                    var p = (from b in db.CreateQuery<blast_lhc_time>()
                             where b.actionTime >= kjtime
                             orderby b.actionTime ascending
                             select b).ToList();

                    var pone = p.Where(b => b.actionTime == one.kjtime).FirstOrDefault();
                    if (pone != null)
                    {
                        int index = one.issueNo;
                        if (index > 2000)
                        {
                            index = int.Parse(index.ToString().Substring(4));
                        }
                        DateTime dt = one.kjtime;
                        var bol = true;
                        int action = 0;
                        foreach (var item in p)
                        {
                            if (item.actionTime.Year != dt.Year)
                            {
                                index = 1;
                                dt = item.actionTime;
                            }
                            action = int.Parse(item.actionTime.Year + "" + index);
                            if (item.actionNo == action)
                            {
                                //正确

                            }
                            else
                            {
                                // 校正
                                bol = false;

                                break;
                            }
                            index++;
                        }
                        if (!bol)
                        {
                            index = one.issueNo;
                            if (index > 2000)
                            {
                                index = int.Parse(index.ToString().Substring(4));
                            }
                            dt = pone.actionTime;
                            DateTime temp = DateTime.Now;
                            foreach (var item in p)
                            {

                                if (item.actionTime.Year != dt.Year)
                                {
                                    index = 1;
                                    dt = item.actionTime;
                                }
                                temp = item.actionTime;
                                action = int.Parse(dt.Year + "" + index);
                                db.GetDal<blast_lhc_time>().Update(b => new blast_lhc_time()
                                {
                                    actionNo = action
                                }, b => b.actionTime == temp && b.typeid == 1);

                                index++;
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                db.Dispose();
                throw;
            }
            finally
            {
                db.Dispose();
            }






            return true;
        }

        public CommonActionResult HK6OpenwinNum(List<blast_data> list)
        {
            CommonActionResult result = new CommonActionResult();
            var db = new DbProvider();
            //// db.Init("Default");
            try
            {
                int actionNo = 0;
                string winNum = "";
                db.Init("MySql.Default", false);

                var datalist = db.CreateQuery<blast_data>().Where(b => b.typeid == 1).ToList();
                var orderdetail = db.CreateQuery<blast_bet_orderdetail>().Where(b => b.typeid == 1).ToList<blast_bet_orderdetail>();
                orderdetail = orderdetail.Where(b => b.BonusStatus == 0).ToList();
                var playedlist = db.CreateQuery<blast_played>().Where(b => b.typeid == 1).ToList();
                db.Begin();
                try
                {

                    string act = "";
                    string tm = "";
                    string zm = "";
                    foreach (var item in list)
                    {
                        actionNo = item.issueNo;

                        var one = datalist.Where(b => b.issueNo == actionNo && b.typeid == 1).FirstOrDefault();


                        if (one == null)
                        {
                            item.typeid = 1;
                            // item.isOpen = 1;
                            db.GetDal<blast_data>().Add(item);
                            one = item;
                        }
                        else
                        {
                            int did = item.id;
                            //db.GetDal<blast_data>().Update(b => new blast_data
                            //{

                            //    isOpen = 1
                            //}, b => b.id == did);
                        }


                        {
                            winNum = one.kjdata.Replace("+", "|");

                            //结算
                            act = actionNo + "";
                            var orderdetailList = orderdetail.Where(b => b.issueNo == act).ToList();

                            // BaseOrderHelper bh = new BaseOrderHelper();
                            foreach (blast_bet_orderdetail oritem in orderdetailList)
                            {
                                //开始结算

                                tm = winNum.Split('|')[1];
                                zm = winNum.Split('|')[0];



                                BaseOrderHelper winHelper = BaseOrderHelper.GetOrderHelper(oritem, db);
                                winHelper.WinMoney(oritem, winNum);


                            }


                        }
                    }

                    db.Commit();
                    result.Message = "成功结算";
                    result.IsSuccess = true;
                }
                catch (Exception ex)
                {
                    result.ReturnValue = ex.ToString();
                    result.Message = "失败";
                    result.IsSuccess = false;
                    //throw;
                    db.Rollback();
                }

                // index++;








            }
            catch (Exception ex)
            {
                result.ReturnValue = ex.ToString();
                result.Message = "开奖成功";
                result.IsSuccess = false;
            }
            finally
            {

                db.Dispose();
            }
            return result;
        }


        public CommonActionResult HK6OpenwinNumByIssus(string userid, blast_data data)
        {
            CommonActionResult result = new CommonActionResult();
            var db = new DbProvider();
          //  var LettoryDB = new DbProvider();
            //// db.Init("Default");
            db.Init("MySql.Default", false);
          //  LettoryDB.Init("SqlServer.Default", true);
            #region 校验token 权限校验
            // string userid = KaSon.FrameWork.Common.CheckToken.UserAuthentication.ValidateAuthentication(tokens);
            //C_Auth_UserRole
            var UserRole = LettoryDB.CreateQuery<C_Auth_UserRole>().Where(b => b.UserId == userid).FirstOrDefault();
            int issueNo = data.issueNo;
            var datetime = db.CreateQuery<blast_lhc_time>().Where(b => b.actionNo == issueNo).FirstOrDefault();
            var mb = new DataServiceHelper(db).GetissueNo();
            string n1 = "";
            string n2 = "";
            if (mb != null)
            {
                n1 = data.issueNo + "";
                n2 = mb.actionNo + "";
                if (n1.Length == 5)
                {
                    n1 = n1.Substring(0, 4) + "00" + n1.Substring(4, 1);
                }
                if (n1.Length == 6)
                {
                    n1 = n1.Substring(0, 4) + "0" + n1.Substring(4, 2);
                }
                if (n2.Length == 5)
                {
                    n2 = n2.Substring(0, 4) + "00" + n2.Substring(4, 1);
                }
                if (n2.Length == 6)
                {
                    n2 = n2.Substring(0, 4) + "0" + n2.Substring(4, 2);
                }

                if (int.Parse(n1) > int.Parse(n2))
                {
                    result.Message = "不能提前几期开奖";
                    result.IsSuccess = false;
                    result.Code = 300;
                    result.StatuCode = 300;
                    LettoryDB.Dispose();
                    db.Dispose();
                    return result;
                }

            }
            if (datetime == null)
            {
                // result.ReturnValue = ex.ToString();
                result.Message = "期号无效无法开奖";
                result.IsSuccess = false;
                result.Code = 300;
                result.StatuCode = 300;
                LettoryDB.Dispose();
                db.Dispose();
                return result;
            }
            if (UserRole == null)
            {
                // result.ReturnValue = ex.ToString();
                result.Message = "没有权限操作";
                result.IsSuccess = false;
                result.Code = 300;
                result.StatuCode = 300;
                LettoryDB.Dispose();
                db.Dispose();
                return result;
            }
            string roleid = UserRole.RoleId;
            var Auth_Roles = LettoryDB.CreateQuery<C_Auth_Roles>().Where(b => b.RoleId == roleid).FirstOrDefault();
            if (Auth_Roles != null && Auth_Roles.IsAdmin)
            {

            }
            else
            {
                var Auth_RoleFunction = LettoryDB.CreateQuery<C_Auth_RoleFunction>().Where(b => b.RoleId == roleid).FirstOrDefault();
                if (Auth_RoleFunction == null || Auth_RoleFunction.FunctionId.Trim() != "GLHKJ100")
                {
                    result.Message = "没有权限操作";
                    result.IsSuccess = false;
                    result.Code = 300;
                    result.StatuCode = 300;
                    LettoryDB.Dispose();
                    db.Dispose();
                    return result;

                }

            }


            #endregion
            try
            {
                int actionNo = 0;
                string winNum = "";


                var timelist = db.CreateQuery<blast_lhc_time>().ToList();
                var datalist = db.CreateQuery<blast_data>().Where(b => b.typeid == 1).ToList();
                var orderdetail = db.CreateQuery<blast_bet_orderdetail>().Where(b => b.BonusStatus == 0 && b.typeid == 1).ToList<blast_bet_orderdetail>();
                var playedlist = db.CreateQuery<blast_played>().Where(b => b.typeid == 1).ToList();
                db.Begin();
                try
                {
                    var item = data;
                    string act = "";
                    string tm = "";
                    string zm = "";
                    //foreach (var item in list)
                    //{
                    actionNo = int.Parse(n1);

                    var one = datalist.Where(b => b.issueNo == actionNo && b.typeid == 1).FirstOrDefault();
                    var time = timelist.Where(b => b.actionNo == actionNo).FirstOrDefault();

                    if (one == null)
                    {
                        item.typeid = 1;
                        // item.isOpen = 1;
                        item.kjdata = item.kjdata.Replace("|", "+");
                        if (time != null)
                        {
                            item.kjtime = time.actionTime;
                        }
                        item.issueNo = actionNo;
                        db.GetDal<blast_data>().Add(item);
                        one = item;
                    }
                    else
                    {
                        int did = item.id;
                        //db.GetDal<blast_data>().Update(b => new blast_data
                        //{

                        //    isOpen = 1
                        //}, b => b.id == did);
                    }



                    winNum = one.kjdata.Replace("+", "|");

                    //结算
                    act = actionNo + "";
                    var orderdetailList = orderdetail.Where(b => b.issueNo == act).ToList();

                    // BaseOrderHelper bh = new BaseOrderHelper();
                    foreach (blast_bet_orderdetail oritem in orderdetailList)
                    {
                        //开始结算

                        tm = winNum.Split('|')[1];
                        zm = winNum.Split('|')[0];



                        BaseOrderHelper winHelper = BaseOrderHelper.GetOrderHelper(oritem, db);
                        winHelper.WinMoney(oritem, winNum);


                    }



                    //  }

                    db.Commit();
                    result.Message = "成功结算";
                    result.IsSuccess = true;
                }
                catch (Exception ex)
                {
                    result.ReturnValue = ex.ToString();
                    result.Message = "失败";
                    result.IsSuccess = false;
                    //throw;
                    db.Rollback();
                }

                // index++;








            }
            catch (Exception ex)
            {
                result.ReturnValue = ex.ToString();
                result.Message = "开奖成功";
                result.IsSuccess = false;
            }
            finally
            {

             //   LettoryDB.Dispose();
                db.Dispose();
            }
            return result;
        }
        public CommonActionResult BJPKOpenwinNumByIssus(string userid, blast_data data)
        {
            CommonActionResult result = new CommonActionResult();
            var db = new DbProvider();
            var LettoryDB = new DbProvider();
            LettoryDB.Init("SqlServer.Default", true);
            db.Init("MySql.Default", false);
            //// db.Init("Default");
            ///
               #region 校验token 权限校验
            // string userid = KaSon.FrameWork.Common.CheckToken.UserAuthentication.ValidateAuthentication(tokens);
            //C_Auth_UserRole
            var UserRole = LettoryDB.CreateQuery<C_Auth_UserRole>().Where(b => b.UserId == userid).FirstOrDefault();
            if (UserRole == null)
            {
                // result.ReturnValue = ex.ToString();
                result.Message = "没有权限操作";
                result.IsSuccess = false;
                result.Code = 300;
                result.StatuCode = 300;
                LettoryDB.Dispose();
                db.Dispose();
                return result;
            }
            string roleid = UserRole.RoleId;
            var Auth_Roles = LettoryDB.CreateQuery<C_Auth_Roles>().Where(b => b.RoleId == roleid).FirstOrDefault();
            if (Auth_Roles != null && Auth_Roles.IsAdmin)
            {

            }
            else
            {
                var Auth_RoleFunction = LettoryDB.CreateQuery<C_Auth_RoleFunction>().Where(b => b.RoleId == roleid).FirstOrDefault();
                if (Auth_RoleFunction == null || Auth_RoleFunction.FunctionId.Trim() != "GLHKJ100")
                {
                    result.Message = "没有权限操作";
                    result.IsSuccess = false;
                    result.Code = 300;
                    result.StatuCode = 300;
                    LettoryDB.Dispose();
                    db.Dispose();
                    return result;

                }

            }
            #endregion
            try
            {
                int actionNo = 0;
                string winNum = "";


                var datalist = db.CreateQuery<blast_data>().Where(b => b.typeid == 2).ToList();
                var orderdetail = db.CreateQuery<blast_bet_orderdetail>().Where(b => b.BonusStatus == 0 && b.typeid == 2).ToList<blast_bet_orderdetail>();
                var playedlist = db.CreateQuery<blast_played>().Where(b => b.typeid == 2).ToList();
                db.Begin();
                try
                {

                    string act = "";
                    string tm = "";
                    string zm = "";
                    var item = data;
                    //foreach (var item in list)
                    //{
                    actionNo = item.issueNo;

                    var one = datalist.Where(b => b.issueNo == actionNo && b.typeid == 2).FirstOrDefault();


                    if (one == null)
                    {
                        item.typeid = 2;
                        // item.isOpen = 1;
                        db.GetDal<blast_data>().Add(item);
                        one = item;
                    }
                    else
                    {
                        int did = item.id;
                        //db.GetDal<blast_data>().Update(b => new blast_data
                        //{

                        //    isOpen = 1
                        //}, b => b.id == did);
                    }


                    {
                        winNum = one.kjdata;//.Replace("+", "|");

                        //结算
                        act = actionNo + "";
                        var orderdetailList = orderdetail.Where(b => b.issueNo == act).ToList();

                        // BaseOrderHelper bh = new BaseOrderHelper();
                        foreach (blast_bet_orderdetail oritem in orderdetailList)
                        {
                            //开始结算

                            // tm = winNum.Split('|')[1];
                            //zm = winNum.Split('|')[0];



                            BaseOrderHelper winHelper = BaseOrderHelper.GetOrderHelper(oritem, db);
                            winHelper.WinMoney(oritem, winNum);


                        }


                    }
                    //   }

                    db.Commit();
                    result.Message = "成功结算";
                    result.IsSuccess = true;
                }
                catch (Exception ex)
                {
                    result.ReturnValue = ex.ToString();
                    result.Message = "失败";
                    result.IsSuccess = false;
                    //throw;
                    db.Rollback();
                }

                // index++;








            }
            catch (Exception ex)
            {
                result.ReturnValue = ex.ToString();
                result.Message = "开奖成功";
                result.IsSuccess = false;
            }
            finally
            {

                LettoryDB.Dispose();
                db.Dispose();
            }
            return result;
        }

        public bool UpdateErrorIssue()
        {
            var db = new DbProvider();
            // var LettoryDB = new DbProvider();
            //  LettoryDB.Init("SqlServer.Default", true);
            db.Init("MySql.Default", false);
            var datalist = db.CreateQuery<blast_data>().Where(b=>b.typeid==1).ToList();
            var orderdlist = db.CreateQuery<blast_bet_order>().ToList();
            var timelist = db.CreateQuery<blast_lhc_time>().ToList();
            var orderdetail = db.CreateQuery<blast_bet_orderdetail>().ToList();

           db.GetDal<blast_bet_order>().Delete(b=>b.issueNo=="141");
            db.GetDal<blast_bet_orderdetail>().Delete(b => b.issueNo == "141");

            foreach (var item in orderdetail)
            {
                string nissue = item.issueNo + "";
                if (item.id== 567)
                {
                    nissue = item.issueNo + "";
                }
                if (nissue.Length > 4 && nissue.Length < 7)
                {
                    string s = nissue.Substring(4);
                    if (s.Length == 1)
                    {
                        nissue = nissue.Substring(0, 4) + "00" + s;
                    }
                    if (s.Length == 2)
                    {
                        nissue = nissue.Substring(0, 4) + "0" + s;
                    }
                    int id = item.id;
                    db.GetDal<blast_bet_orderdetail>().Update(b => new blast_bet_orderdetail()
                    {
                        issueNo = nissue
                    }, b => b.id == id);
                }



            }
            foreach (var item in orderdlist)
            {
                string nissue = item.issueNo + "";
                if (nissue.Length > 4 && nissue.Length < 7)
                {
                    string s = nissue.Substring(4);
                    if (s.Length == 1)
                    {
                        nissue = nissue.Substring(0, 4) + "00" + s;
                    }
                    if (s.Length == 2)
                    {
                        nissue = nissue.Substring(0, 4) + "0" + s;
                    }
                    int id = item.id;
                    db.GetDal<blast_bet_order>().Update(b => new blast_bet_order()
                    {
                        issueNo = nissue
                    }, b => b.id == id);
                   
                  

                }


            }

            foreach (var item in timelist)
            {
                string nissue = item.actionNo + "";
                if (nissue.Length > 4 && nissue.Length < 7)
                {
                    string s = nissue.Substring(4);
                    if (s.Length == 1)
                    {
                        nissue = nissue.Substring(0, 4) + "00" + s;
                    }
                    if (s.Length == 2)
                    {
                        nissue = nissue.Substring(0, 4) + "0" + s;
                    }
                    int actionNo = int.Parse(nissue);
                    int id = item.id;
                    db.GetDal<blast_lhc_time>().Update(b => new blast_lhc_time()
                    {
                        actionNo = actionNo
                    }, b => b.id == id);
                }


            }

            foreach (var item in datalist)
            {
                string nissue = item.issueNo + "";
                if (nissue.Length > 4 && nissue.Length < 7)
                {
                    string s = nissue.Substring(4);
                    if (s.Length == 1)
                    {
                        nissue = nissue.Substring(0, 4) + "00" + s;
                    }
                    if (s.Length == 2)
                    {
                        nissue = nissue.Substring(0, 4) + "0" + s;
                    }
                    int actionNo = int.Parse(nissue);
                    int id = item.id;
                  
                    try
                    {
                        db.GetDal<blast_data>().Update(b => new blast_data()
                        {
                            issueNo = actionNo
                        }, b => b.id == id);
                    }
                    catch (Exception)
                    {

                        db.GetDal<blast_data>().Delete(b => b.id == id);
                    }
                }


            }

            db.Dispose();

            Console.WriteLine("成功更新错误期号");

            return true;
        }
        public CommonActionResult BJPKOpenwinNum(List<blast_data> list)
        {
            CommonActionResult result = new CommonActionResult();
            var db = new DbProvider();
            //// db.Init("Default");
            try
            {
                int actionNo = 0;
                string winNum = "";
                db.Init("MySql.Default", false);

                var datalist = db.CreateQuery<blast_data>().Where(b => b.typeid == 2).ToList();
                var orderdetail = db.CreateQuery<blast_bet_orderdetail>().Where(b => b.BonusStatus == 0 && b.typeid == 2).ToList<blast_bet_orderdetail>();
                var playedlist = db.CreateQuery<blast_played>().Where(b => b.typeid == 2).ToList();
                db.Begin();
                try
                {

                    string act = "";
                    string tm = "";
                    string zm = "";
                    foreach (var item in list)
                    {
                        actionNo = item.issueNo;

                        var one = datalist.Where(b => b.issueNo == actionNo && b.typeid == 2).FirstOrDefault();


                        if (one == null)
                        {
                            item.typeid = 2;
                            // item.isOpen = 1;
                            db.GetDal<blast_data>().Add(item);
                            one = item;
                        }
                        else
                        {
                            int did = item.id;
                            //db.GetDal<blast_data>().Update(b => new blast_data
                            //{

                            //    isOpen = 1
                            //}, b => b.id == did);
                        }


                        {
                            winNum = one.kjdata;//.Replace("+", "|");

                            //结算
                            act = actionNo + "";
                            var orderdetailList = orderdetail.Where(b => b.issueNo == act).ToList();

                            // BaseOrderHelper bh = new BaseOrderHelper();
                            foreach (blast_bet_orderdetail oritem in orderdetailList)
                            {
                                //开始结算

                                // tm = winNum.Split('|')[1];
                                //zm = winNum.Split('|')[0];



                                BaseOrderHelper winHelper = BaseOrderHelper.GetOrderHelper(oritem, db);
                                winHelper.WinMoney(oritem, winNum);


                            }


                        }
                    }

                    db.Commit();
                    result.Message = "成功结算";
                    result.IsSuccess = true;
                }
                catch (Exception ex)
                {
                    result.ReturnValue = ex.ToString();
                    result.Message = "失败";
                    result.IsSuccess = false;
                    //throw;
                    db.Rollback();
                }

                // index++;








            }
            catch (Exception ex)
            {
                result.ReturnValue = ex.ToString();
                result.Message = "开奖成功";
                result.IsSuccess = false;
            }
            finally
            {

                db.Dispose();
            }
            return result;
        }
    }
}

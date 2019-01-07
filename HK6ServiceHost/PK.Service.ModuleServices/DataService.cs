using Kason.Sg.Core.CPlatform.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using EntityModel.Enum;
using EntityModel.CoreModel;
using KaSon.FrameWork.Common;

using EntityModel.Communication;
using KaSon.FrameWork.ORM.Helper;
using KaSon.FrameWork.Common.Sport;
using System.Threading.Tasks;

using System.IO;
using System.Text;
using EntityModel.ExceptionExtend;

using System.Linq;
using Kason.Sg.Core.ProxyGenerator;
using Microsoft.Extensions.Logging;
using KaSon.FrameWork.ORM;
using EntityModel;
using KaSon.FrameWork.Analyzer.Hk6Model;
using KaSon.FrameWork.Common.Utilities;
using KaSon.FrameWork.Common.Hk6;
using KaSon.FrameWork.Services.ORM;
using KaSon.FrameWork.Services.Enum;
using PK.Service.IModuleServices;
using HK6.ModuleBaseServices;
using KaSon.FrameWork.ORM.Helper.BJPK;

namespace PK.Service.ModuleServices
{
    /// <summary>
    /// 管理系统服务
    /// </summary>
    [ModuleName("Data")]
    public class DataService : KgBaseService, IDataService
    {

        //IKgLog log = null;
        //public BettingService()
        //{
        //    log = new Log4Log();
        //}
        ILogger<BettingService> _Log;
        private readonly Repository _rep;
        private IDbProvider DB = null;
        private IDbProvider LettoryDB = null;

        public DataService(Repository repository)
        {
            // _Log = log;
            this._rep = repository;
            DB = _rep.LDB.Init("MySql.Default", true);
            LettoryDB = _rep.DB.Init("SqlServer.Default", true);
        }

        public Task<CommonActionResult> PlayCategory()
        {
            CommonActionResult result = new CommonActionResult();
            
            try
            {
                var mblist = DB.CreateQuery<blast_played_group>().Where(b => b.enable == true && b.typeid==2).ToList();
                var mPlayedList = DB.CreateQuery<blast_played>().Where(b => b.enable == true && b.typeid == 2).ToList();

                foreach ( var item in mblist)
                {
                    item.PlayedList = mPlayedList.Where(b => b.groupId == item.groupId).ToList();
                }
                result.IsSuccess = true;
                result.Value = mblist;
            }
            catch (Exception ex)
            {
                LettoryDB.Rollback();
                DB.Rollback();
                result.Message = "系统错误";
                result.IsSuccess = false;

                result.Code = 500;
                result.StatuCode = 500;
                result.ReturnValue = ex.ToString();
            }
            finally
            {
                DB.Dispose();
                LettoryDB.Dispose();

            }

            // LettoryDB.GetDal<C_Game_Transfer>().Add(ctransfer);

            return Task.FromResult(result);


        }
      

        public Task<CommonActionResult> GetCurrentIssuseNo()
        {
            //CommonActionResult result = new CommonActionResult();
            //result.IsSuccess = true;
            //var mb = new DataServiceHelper(DB).GetissueNo();
            CommonActionResult result = new DataHelper(DB).GetGames(2);
            //result.Value = mb;
            return Task.FromResult(result);
        }

        
        public Task<CommonActionResult> PlayInfo()
        {
            CommonActionResult result = new CommonActionResult();
            //playGroup
            var pmb = DB.CreateQuery<blast_played>().Where(b=>b.typeid==2).ToList();
            var mb = DB.CreateQuery<blast_antecode>().Where(b => b.typeid == 2).ToList();
            var q = from b in mb
                    group b by b.playid into g
                    select g;

            List<playGroup> pgroupList = new List<playGroup>();
            int pid = 0;
            var pp = new blast_played();
            var antecodeList = new List<blast_antecode>();
            foreach (var item in q)
            {
                pid = item.Key;
                pp = pmb.Where(b => b.playId == pid).FirstOrDefault();
                antecodeList = item.ToList<blast_antecode>().OrderBy(b => b.sort).ToList();
                playGroup pg = new playGroup();
                switch (item.Key)
                {   
                    case 59://前2
                    case 60:
                    case 61:
                    case 62:
                    case 63:
                        pg = new playGroup()
                        {
                            CodeList = antecodeList.Where(b=>b.cateNum==1).ToList(),
                            CodeList1 = antecodeList.Where(b => b.cateNum == 2).ToList(),
                            CodeList2 = antecodeList.Where(b => b.cateNum == 3).ToList(),
                            CodeList3 = antecodeList.Where(b => b.cateNum == 4).ToList(),
                            CodeList4 = antecodeList.Where(b => b.cateNum == 5).ToList(),
                            CodeList5 = antecodeList.Where(b => b.cateNum == 6).ToList(),
                            CodeList6 = antecodeList.Where(b => b.cateNum == 7).ToList(),
                            CodeList7 = antecodeList.Where(b => b.cateNum == 8).ToList(),
                            CodeList8 = antecodeList.Where(b => b.cateNum == 9).ToList(),
                            CodeList9 = antecodeList.Where(b => b.cateNum == 10).ToList(),
                            Key = item.Key + "",
                            Name = pp.name
                        };
                        break;
                    default:
                         pg = new playGroup()
                        {
                            CodeList = antecodeList,
                            Key = item.Key + "",
                            Name = pp.name
                        };
                        break;
                }
                
                pgroupList.Add(pg);
            }


            result.Value = pgroupList;
            result.IsSuccess = true;

            return Task.FromResult(result);
        }
        /// <summary>
        /// 获取订单信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Task<CommonActionResult> OrderInfo(string userId, int PageIndex)
        {
            CommonActionResult result = new CommonActionResult();
            //PageIndex
            var query = DB.CreateQuery<blast_bet_orderdetail>();

            var wlist = new List<WhereField>();
            wlist.Add(new WhereField()
            {
                Field = "userId",
                Value = userId,
                WhereType = WhereType.Equal

            });
            wlist.Add(new WhereField()
            {
                Field = "typeid",
                Value = 2+"",
                WhereType = WhereType.Equal

            });

            
            var slist = new List<SortField>();
            slist.Add(new SortField()
            {
                Field = "Id",
                IsASC = false

            });
            QueryArgs qargs = new QueryArgs()
            {
                PageIndex = PageIndex,
                PageSize = 20,
                WhereFields = wlist,
                SortFields = slist

            };

            var data = DB.CreateComQuery().Query<blast_bet_orderdetail>(query, qargs);

            // var list = DB.CreateQuery<blast_bet_orderdetail>().Where(b => b.userId == userId).ToList();
            result.Value = data.Data;
            result.IsSuccess = true;
            if (data.RowCount <= 0)
            {
                result.IsSuccess = false;
                result.Code = 500;
                result.StatuCode = 500;
            }
            //if (list)
            //{
            //    result.ReturnObj = new blast_member();
            //}


            return Task.FromResult(result);
        }


        public Task<CommonActionResult> OrderDetail(string oId)
        {
            CommonActionResult result = new CommonActionResult();
            //PageIndex
            var orderdetail = DB.CreateQuery<blast_bet_orderdetail>().Where(b => b.anteSchemeId == oId && b.typeid==2).FirstOrDefault();


            // var list = DB.CreateQuery<blast_bet_orderdetail>().Where(b => b.userId == userId).ToList();
            result.Value = orderdetail;
            result.IsSuccess = true;

            //if (list)
            //{
            //    result.ReturnObj = new blast_member();
            //}


            return Task.FromResult(result);
        }

        public Task<CommonActionResult> HostoryData(string userId, int PageIndex)
        {
            CommonActionResult result = new CommonActionResult();
            int lhc = (int)MGGameType.LHC;

            //PageIndex
            var query = LettoryDB.CreateQuery<blast_data>();

            var wlist = new List<WhereField>();
            //wlist.Add(new WhereField()
            //{
            //    Field = "UserId",
            //    Value = userId,
            //    WhereType = WhereType.Equal

            //});
            wlist.Add(new WhereField()
            {
                Field = "typeid",
                Value = "2",
                WhereType = WhereType.Equal

            });

            var slist = new List<SortField>();
            slist.Add(new SortField()
            {
                Field = "kjtime",
                IsASC = false

            });
            QueryArgs qargs = new QueryArgs()
            {
                PageIndex = PageIndex,
                PageSize = 20,
                WhereFields = wlist,
                SortFields = slist

            };

            var data = LettoryDB.CreateComQuery().Query<blast_data>(query, qargs);

            // var list = DB.CreateQuery<blast_bet_orderdetail>().Where(b => b.userId == userId).ToList();
            result.Value = data.Data;
            result.IsSuccess = true;
            if (data.RowCount <= 0)
            {
                result.IsSuccess = false;
                result.Code = 500;
                result.StatuCode = 500;
            }
            


            return Task.FromResult(result);

        }

    }
}

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
                db.Init("MySql.Default", true);
                list = (from items in list orderby items select items).ToList();
                DateTime oneDate = DateTime.Parse(list[0]);
                string Year = oneDate.Year + "";
                string atcNo = "";
                int index = 73;
                var timelist = db.CreateQuery<blast_data_time>().ToList(); ;
                foreach (var item in list)
                {
                    if (index>142)
                    {
                        Console.WriteLine(item);
                    }

                    if (DateTime.Parse(item).Year > oneDate.Year)
                    {
                        index = 1;
                        Year = DateTime.Parse(item).Year + "";
                    }
                    atcNo = index + "";
                    atcNo = Year + "" + index;
                    //if (index < 10)
                    //{
                    //    atcNo = Year + "00" + index;
                    //}
                    //if (index >= 10 && index < 100)
                    //{
                    //    atcNo = Year + "0" + index;
                    //}
                    //if (index > 100)
                    //{
                    //    atcNo = Year + "" + index;
                    //}
                    var one = timelist.Where(b => b.actionDate == item).FirstOrDefault();
                    if (one == null)
                    {
                        blast_data_time b = new blast_data_time()
                        {
                            actionDate = item,
                            actionNo = int.Parse(atcNo),
                            actionTime = DateTime.Parse(item),
                            stopTime = DateTime.Parse(DateTime.Parse(item).ToShortDateString()).AddHours(21).AddMinutes(30)

                        };
                        db.GetDal<blast_data_time>().Add(b);
                    }
                    else
                    {
                        db.GetDal<blast_data_time>().Update(b => new blast_data_time {
                             actionNo = int.Parse(atcNo)
                        }, b => b.actionDate == item);
                    }

                    index++;
                }
            }
            catch (Exception)
            {

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
                db.Init("MySql.Default", true);
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
            }
            catch (Exception ex)
            {

                throw;
            }
            finally
            {
                db.Dispose();
            }






            return true;
        }

        public CommonActionResult HK6OpenwinNum(List<blast_data_time> list)
        {
            CommonActionResult result = new CommonActionResult();
            var db = new DbProvider();
            //// db.Init("Default");
            try
            {
                int actionNo = 0;
                string winNum = "";
                db.Init("MySql.Default", true);
                
                var datalist = db.CreateQuery<blast_data_time>().Where(b => b.isOpen != 1 || b.isOpen==null).ToList();
                var orderdetail = db.CreateQuery<blast_bet_orderdetail>().Where(b => b.BonusStatus == 0).ToList<blast_bet_orderdetail>();
                var playedlist = db.CreateQuery<blast_played>().ToList();
                db.Begin();
                try
                {

                    string act = "";
                    string tm = "";
                    string zm = "";
                    foreach (var item in datalist)
                    {
                        actionNo = item.actionNo;

                        var one = list.Where(b => b.actionNo == actionNo).FirstOrDefault();


                        if (one == null)
                        {

                            // db.GetDal<blast_data>().Add(item);
                        }
                        else
                        {
                            winNum = one.winNum;

                            //结算
                            act = actionNo + "";
                            var orderdetailList = orderdetail.Where(b => b.issueNo == act).ToList();

                            db.GetDal<blast_data_time>().Update(b => new blast_data_time
                            {
                                winNum = winNum,
                                isOpen = 1
                            }, b => b.actionNo == actionNo);
                            // BaseOrderHelper bh = new BaseOrderHelper();
                            foreach (blast_bet_orderdetail oritem in orderdetailList)
                            {
                                //开始结算

                                tm = winNum.Split('|')[1];
                                zm = winNum.Split('|')[0];



                                BaseOrderHelper winHelper = BaseOrderHelper.GetOrderHelper(oritem, db);
                                winHelper.WinMoney(oritem, winNum);


                            }

                            var bdata = db.CreateQuery<blast_data>().Where(b => b.issueNo == item.actionNo).FirstOrDefault();
                            if (bdata == null)
                            {
                                bdata = new blast_data()
                                {
                                    createTime = DateTime.Now,
                                    issueNo = item.actionNo,
                                    kjdata = item.winNum.Replace("|", "+"),
                                    kjtime = item.stopTime,
                                    typeid = 1,
                                    kjnumber = item.winNum.Replace("|", "+"),
                                };
                                db.GetDal<blast_data>().Add(bdata);
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
                    result.Message = "开奖成功";
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

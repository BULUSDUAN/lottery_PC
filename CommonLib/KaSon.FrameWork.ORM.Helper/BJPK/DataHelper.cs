using EntityModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using KaSon.FrameWork.Common.BJPK;
using KaSon.FrameWork.Common.Utilities;
using EntityModel.Communication;

namespace KaSon.FrameWork.ORM.Helper.BJPK
{
   public class DataHelper
    {
        private IDbProvider DB = null;

        public DataHelper(IDbProvider _DB)
        {
            DB = _DB;
        }
        public CommonActionResult GetGames(int type)
        {
            CommonActionResult sresult = new CommonActionResult();
            try
            {
                blast_type result = null;
                //await this.data_manager.UseConnectionAsync(async db =>
                //{
                //    result = await db.QueryFirstOrDefaultAsync<blast_type>("select id,type,sort,name,title,shortName,info,data_ftime,enable from blast_type where isDelete=0 and id=@id order by sort", new { id = type });
                //});
                result = DB.CreateQuery<blast_type>().Where(b => b.id == type).FirstOrDefault();

                if (result == null)
                    throw new ArgumentException("未有该彩种ID，请传正确的彩种ID");


                var thistime = DateTime.Now;
                var lasttime = DateTime.Now;
                //TimeSpan.FromSeconds
                string actionTime = DateTime.Now.AddSeconds(result.data_ftime).ToString("HH:mm:ss");
                var now = DateTimeHelper.LocalDateTimeToUnixTimeStamp(DateTime.Now);
                string sqltalbe = "blast_data_time";
                //if (result.id == 34)
                //{
                //    sqltalbe = "blast_lhc_time";
                //    actionTime = DateTime.Now.AddSeconds(result.data_ftime).ToString();
                //}
                int gameid = result.id;
                blast_data_time nextgameno = null;
                blast_data_time lastgameno = null;
                //string kjhao = "";
                //nextgameno = (from b in DB.CreateQuery<blast_data_time>()
                //             where b.typeid == type && b.actionhours > actionTime
                //             orderby b.actionhours
                //             select b).FirstOrDefault();

                nextgameno = DB.CreateSQLQuery($"select * from " +
                    $"{sqltalbe} where typeid=@type and actionhours>@actionTime " +
                    $"order by actionhours limit 1")
                    .SetString("@type", type + "")
                    .SetString("@actionTime", actionTime + "").First<blast_data_time>();

                if (nextgameno == null)
                {
                    //nextgameno = (from b in DB.CreateQuery<blast_data_time>()
                    //              where b.typeid == type
                    //              orderby b.actionhours
                    //              select b).FirstOrDefault();
                    nextgameno = DB.CreateSQLQuery($"select * from {sqltalbe} " +
                        $"where typeid=@type  order by actionhours limit 1")
                         .SetString("@type", type + "").First<blast_data_time>();
                    thistime = thistime.AddDays(1);
                }
                //lastgameno = (from b in DB.CreateQuery<blast_data_time>()
                //              where b.typeid == type && b.actionhours <= actionTime
                //              orderby b.actionhours
                //              select b).FirstOrDefault();
                //new { type = gameid, actionTime = actionTime }
                lastgameno = DB.CreateSQLQuery($"select * from {sqltalbe} " +
                    $"where typeid=@type and actionhours<=@actionTime order by actionhours desc limit 1")
               .SetString("@type", gameid + "")
                    .SetString("@actionTime", actionTime + "").First<blast_data_time>();
                if (lastgameno == null)
                {
                    lastgameno = (from b in DB.CreateQuery<blast_data_time>()
                                  where b.typeid == type
                                  orderby b.actionhours
                                  select b).FirstOrDefault();

                    //  lastgameno = await db.QueryFirstOrDefaultAsync($"select actionNo, actionTime from {sqltalbe} where type=@type order by actionNo desc limit 1", new { type = gameid });
                    lasttime = lasttime.AddDays(-1);
                }
                //开奖时间
                #region 当前期
                //if (result.id == 34)
                //{
                //    actionTime = nextgameno.actionTime.ToString();
                //}
                //else
                {
                    string ts = nextgameno.actionhours;


                    Console.WriteLine($"TimeSpan: {ts.ToString()}");


                    actionTime = $"{thistime.ToString("yyyy-MM-dd")} {ts}";
                }
                thistime = Convert.ToDateTime(actionTime);
                var bagintime = DateTimeHelper.LocalDateTimeToUnixTimeStamp(thistime.Date);
                string actionNo = nextgameno.actionNo;
                string date = thistime.ToString("yyyyMMdd");
                string number = UsefullHelper.NumberFormat(gameid, date, thistime, int.Parse(actionNo), bagintime);
                var thistimenow = DateTimeHelper.LocalDateTimeToUnixTimeStamp(thistime);
                var kjDiffTime = thistimenow - now;
                var diffTime = kjDiffTime - (ulong)result.data_ftime;
                #endregion


                #region 上一期
                string lastactionTime = string.Empty;
                if (result.id == 34)
                {
                    lastactionTime = lastgameno.actionTime.ToString();
                }
                else
                {
                    string ts = lastgameno.actionhours;
                    lastactionTime = $"{lasttime.ToString("yyyy-MM-dd")} {ts}";
                }
                var lastbagintime = DateTimeHelper.LocalDateTimeToUnixTimeStamp(lasttime.Date);
                string lastactionNo = lastgameno.actionNo;
                string lastdate = lasttime.ToString("yyyyMMdd");
                string lastnumber = UsefullHelper.NumberFormat(gameid, lastdate, lasttime, int.Parse(lastactionNo), lastbagintime);
                #endregion
                int lastnum = int.Parse(lastnumber);
                //kjhao = await db.QueryFirstOrDefaultAsync<string>($"select data from blast_data where type=@type and number=@number", new { type = gameid, number = lastnumber });
                var kjhao = DB.CreateQuery<blast_data>().Where(b => b.typeid == gameid && b.issueNo == lastnumber).FirstOrDefault();
                //await this.data_manager.UseConnectionAsync(async db =>
                //{

                //});



                sresult.IsSuccess = true;
                sresult.Value = new
                {
                    lastactionNum = lastactionNo,//上一期期号
                    lastactionNo = lastnumber,//期号
                    id = result.id,
                    type = result.id,
                    gamecode = result.name,
                    sort = result.sort,
                    title = result.title,
                    gamename = result.shortName,
                    info = result.info,
                    actionNum = actionNo,//期数
                    actionTime = DateTimeHelper.LocalDateTimeToUnixTimeStamp(thistime),//开奖时间
                    actionNo = number,//期号
                    nowTime = DateTimeHelper.LocalDateTimeToUnixTimeStamp(DateTime.Now),//当前服务器时间
                    diffTime = diffTime,//下注截至倒计时
                    dataftime = result.data_ftime,//开奖前停止下注时间差
                    kjDiffTime = kjDiffTime, //离开奖时间还剩多少秒
                    kjdata = kjhao,
                    actionTime1 = actionTime,
                    actionTime2 = nextgameno.actionTime,
                    enable = result.enable
                };
            }
            catch (ArgumentException exp)
            {
                sresult.IsSuccess = false;
                sresult.Code = 300;
                sresult.StatuCode = 300;
                sresult.ReturnValue = exp.ToString();
            }
            catch (Exception exp)
            {
                sresult.IsSuccess = false;
                sresult.Code = 300;
                sresult.StatuCode = 300;
                sresult.ReturnValue = exp.ToString();
            }
            finally {
                DB.Dispose();
            }
            return sresult;
        }
    }
}

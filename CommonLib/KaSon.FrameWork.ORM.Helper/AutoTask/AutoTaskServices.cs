using KaSon.FrameWork.Common;
using KaSon.FrameWork.ORM.Helper.BusinessLib;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace KaSon.FrameWork.ORM.Helper.AutoTask
{
    public class AutoTaskServices
    {
        /// <summary>
        /// 缓存bjdc,jclq,jczq
        /// </summary>
        public static void AutoCaheData(int seconds)
        {
            Task.WhenAll(new Task[] {
                     CTZQ_BJDC(),
                       // JCLQ(),
                      //  JCZQ(),
                        //StartTaskByWriteChaseOrderToDb(seconds)
            });
        }

        public static async Task CTZQ_BJDC()
        {
            while (true)
            {
                try
                {
                    EntityModel.CoreModel.Issuse_QueryInfoEX val = GameServiceCache.QueryCurretNewIssuseInfoByList();
                    try
                    {
                        HashTableCache.Init_CTZQ_Issuse_Data();
                    }
                    catch(Exception ex)
                    {
                        //获取期号出错

                    }

                    HashTableCache.Init_CTZQ_Data(val);
                    HashTableCache.Init_BJDC_Data(val.BJDC_IssuseNumber.IssuseNumber);
                }
                catch (Exception ex)
                {

                }
                await Task.Delay(30000);
            }
        }

        public static async Task JCLQ()
        {
            while (true)
            {
                try
                {
                    HashTableCache.Init_JCLQ_Data();
                }
                catch (Exception ex)
                {

                }
                await Task.Delay(30000);
            }
        }

        public static async Task JCZQ()
        {
            while (true)
            {
                try
                {
                    HashTableCache.Init_JCZQ_Data("1");
                    HashTableCache.Init_JCZQ_Data();
                }
                catch (Exception ex)
                {

                }
                await Task.Delay(30000);
            }
        }

        /// <summary>
        /// 处理追号订单
        /// </summary>
        /// <param name="Sports_SchemeJobSeconds"></param>
        public static async Task StartTaskByWriteChaseOrderToDb(int seconds)
        {
            while (true)
            {
                try
                {
                    Console.WriteLine(string.Format("追号作业启动...每{0}秒执行一次", seconds));
                    Sports_BusinessBy.WriteChaseOrderToDb();
                }
                catch (Exception ex)
                {

                }
                await Task.Delay(10000 * seconds);
            }
        }
    }
}

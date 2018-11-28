using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Configuration;
using Common.JSON;
using System.Diagnostics;
using StackExchange.Redis;
using GameBiz.Service;
using Activity.Service;

namespace RedisBusinessManager
{
    public partial class AutoTaskManager : Form
    {
        public AutoTaskManager()
        {
            InitializeComponent();
        }

        private void AutoTaskManager_Load(object sender, EventArgs e)
        {
            var taskList = Tools.FindAutoTaskList().OrderBy(p => p.TaskOrder);
            foreach (var item in taskList)
            {
                var ctrl = new CtrlAutoTaskItem(item as IAutoTask);
                fpContent.Controls.Add(ctrl);
            }

            txt_fund_cache_date.Text = DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd");
        }

        private void bt_start_all_Click(object sender, EventArgs e)
        {
            foreach (var item in fpContent.Controls)
            {
                var c = (item as CtrlAutoTaskItem);
                if (c == null)
                    continue;
                c.StartTask();
            }

        }

        private void bt_stop_all_Click(object sender, EventArgs e)
        {
            foreach (var item in fpContent.Controls)
            {
                var c = (item as CtrlAutoTaskItem);
                if (c == null)
                    continue;
                c.StopTask();
            }
        }

        /// <summary>
        /// 添加日志到界面
        /// </summary>
        public void AddLogToList(string log)
        {
            var fLog = string.Format("{0}=>{1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), log);
            Tools.InvokeAddListBoxItem(list_log, fLog);
            var count = Tools.InvokeGetListBoxItemCount(list_log);
            if (count < 100)
            {
                Tools.InvokeSelectListBoxItem(list_log, count - 1);
                return;
            }
            Tools.InvokeClearListBox(list_log);
        }

        /// <summary>
        /// 手工生成资金明细缓存
        /// </summary>
        private void bt_build_fund_cache_Click(object sender, EventArgs e)
        {
            Tools.ExcuteByTimer(1000, () =>
            {
                //资金缓存根目录
                var basePath = ConfigurationManager.AppSettings["FundDetailCacheBasePath"];
                var dateTime = DateTime.Parse(txt_fund_cache_date.Text);
                var dateStr = dateTime.ToString("yyyyMMdd");
                this.AddLogToList(string.Format("开始查询{0}的资金明细数据", txt_fund_cache_date.Text));
                var fundList = string.IsNullOrEmpty(txt_fund_userId.Text.Trim()) ? new GameBizWcfService_Fund().QueryFundDetailByDateTime(dateTime)
                                                                                : new GameBizWcfService_Fund().QueryFundDetailByDateTimeUserId(dateTime, txt_fund_userId.Text.Trim());
                this.AddLogToList(string.Format("已查询到数据{0}条", fundList.FundDetailList.Count));
                var g = fundList.FundDetailList.GroupBy(p => p.UserId);
                var userCount = g.Count();
                this.AddLogToList(string.Format("分组后用户数：{0} 条", userCount));
                var index = 0;
                var watch = new Stopwatch();
                watch.Start();
                foreach (var item in g)
                {
                    try
                    {
                        ++index;
                        this.AddLogToList(string.Format("生成用户 {0}={1}/{2}", item.Key, index, userCount));
                        var dayList = fundList.FundDetailList.Where(p => p.UserId == item.Key).OrderByDescending(p => p.CreateTime).ToList();
                        if (dayList.Count <= 0)
                        {
                            this.AddLogToList("无资金明细数据");
                            continue;
                        }
                        var content = JsonSerializer.Serialize<List<GameBiz.Core.FundDetailInfo>>(dayList);
                        //var r = ServiceHelper.GameFundClient.BuildFundDetailByDate(content, item.Key, dateStr);
                        var path = System.IO.Path.Combine(basePath, item.Key);
                        if (!System.IO.Directory.Exists(path))
                            System.IO.Directory.CreateDirectory(path);

                        var filePath = System.IO.Path.Combine(path, string.Format("{0}.json", dateStr));
                        System.IO.File.WriteAllText(filePath, content, Encoding.UTF8);

                        this.AddLogToList("生成成功");
                    }
                    catch (Exception ex)
                    {
                        this.AddLogToList("生成异常：" + ex.Message);
                    }
                }
                watch.Stop();

                this.AddLogToList(string.Format("==============全部生成完成，用时{0}秒===============", watch.Elapsed.TotalSeconds));

            });
        }

        /// <summary>
        /// 手工执行订单派奖
        /// </summary>
        private void bt_mu_do_prize_Click(object sender, EventArgs e)
        {
            if (com_mu_gameCode.SelectedIndex < 0)
            {
                MessageBox.Show("请选择彩种编码");
                return;
            }
            var gameCode = com_mu_gameCode.Items[com_mu_gameCode.SelectedIndex].ToString();
            var schemeIdArray = rich_schemeid_all.Text.Trim().Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            Tools.ExcuteByTimer(500, () =>
            {
                var errorSchemeIdList = new List<string>();
                foreach (var item in schemeIdArray)
                {
                    this.AddLogToList(string.Format("手工派奖：{0}", item));
                    var log = new GameBizWcfService_Core().Redis_ManualPrizeOrder(gameCode, item);
                    this.AddLogToList(log.Message);
                    if (!log.IsSuccess)
                    {
                        errorSchemeIdList.Add(item);
                    }
                }

                Tools.InvokeSetRichTextBoxText(rich_schemeid_all, string.Join(Environment.NewLine, errorSchemeIdList.ToArray()));
            });
        }

        /// <summary>
        /// 把SQL中的订单加入Redis待拆票库中
        /// </summary>
        private void bt_load_un_split_order_Click(object sender, EventArgs e)
        {
            var schemeIdArray = rich_un_split_order.Text.Trim().Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            Tools.ExcuteByTimer(1000, () =>
            {
                //var schemeIdArray = new GameBizWcfService_Core().QuerySQLUnSplitTicketsOrder().Split('|');
                this.AddLogToList(string.Format("已查询到未拆票的订单{0}条", schemeIdArray.Length));
                for (int i = 0; i < schemeIdArray.Length; i++)
                {
                    var schemeId = schemeIdArray[i];
                    try
                    {
                        this.AddLogToList(string.Format("开始处理订单{0}.{1}/{2}", schemeId, i, schemeIdArray.Length));
                        var log = new GameBizWcfService_Core().AddSQLOrderToWaitSplitList(schemeId);
                        this.AddLogToList(log);
                    }
                    catch (Exception ex)
                    {
                        this.AddLogToList(ex.Message);
                    }
                }

                this.AddLogToList("===全部加载完成===");

            });
        }

        /// <summary>
        /// 添加SQL订单数据到Redis库中
        /// </summary>
        private void bt_load_runningOrder_Click(object sender, EventArgs e)
        {
            var schemeIdArray = rich_un_prizeOrder.Text.Trim().Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            Tools.ExcuteByTimer(1000, () =>
            {
                //var schemeIdArray = new GameBizWcfService_Core().QuerySQLUnPrizeOrder().Split('|');
                //this.AddLogToList(string.Format("已查询到未派奖的订单{0}条", schemeIdArray.Length));
                for (int i = 0; i < schemeIdArray.Length; i++)
                {
                    var schemeId = schemeIdArray[i];
                    try
                    {
                        this.AddLogToList(string.Format("开始处理订单{0}.{1}/{2}", schemeId, i, schemeIdArray.Length));
                        var log = new GameBizWcfService_Core().AddSQLOrderToRunningOrder(schemeId);
                        this.AddLogToList(log);
                    }
                    catch (Exception ex)
                    {
                        this.AddLogToList(ex.Message);
                    }
                }
                this.AddLogToList("===全部加载完成===");
            });
        }

        /// <summary>
        /// 删除redis中的订单数据 
        /// </summary>
        private void bt_delete_order_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("本次操作会删除掉Redis的未派奖订单数据，是否继续？", "确认继续", MessageBoxButtons.YesNo) != System.Windows.Forms.DialogResult.Yes)
                return;

            if (com_mu_gameCode.SelectedIndex < 0)
            {
                MessageBox.Show("请选择彩种编码");
                return;
            }
            var gameCode = com_mu_gameCode.Items[com_mu_gameCode.SelectedIndex].ToString();
            var schemeIdArray = rich_schemeid_all.Text.Trim().Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            Tools.ExcuteByTimer(500, () =>
            {
                var errorSchemeIdList = new List<string>();
                foreach (var item in schemeIdArray)
                {
                    this.AddLogToList(string.Format("手工派奖：{0}", item));
                    //var log = new GameBizWcfService_Core().Redis_ManualPrizeOrder(gameCode, item);
                    //this.AddLogToList(log.Message);
                    //if (!log.IsSuccess)
                    //{
                    //    errorSchemeIdList.Add(item);
                    //}
                }

                Tools.InvokeSetRichTextBoxText(rich_schemeid_all, string.Join(Environment.NewLine, errorSchemeIdList.ToArray()));
            });
        }

        private void bt_do_repirChaseOrder_Click(object sender, EventArgs e)
        {
            MessageBox.Show("todo");
            return;

            var orderId = txt_repair_chaseOrderid.Text.Trim();
            if (string.IsNullOrEmpty(orderId))
            {
                MessageBox.Show("订单号不能为空");
                return;
            }
            if (!orderId.StartsWith("CHASE"))
            {
                MessageBox.Show("追号订单号应是以CHASE开头");
                return;
            }


            Tools.ExcuteByTimer(500, () =>
            {


            });
        }

        private void bt_addMoney_start_Click(object sender, EventArgs e)
        {
            var schemeIdArray = rich_addmoney_schemeId.Text.Trim().Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            Tools.ExcuteByTimer(1000, () =>
            {
                for (int i = 0; i < schemeIdArray.Length; i++)
                {
                    var schemeId = schemeIdArray[i];
                    try
                    {
                        this.AddLogToList(string.Format("开始处理订单{0}.{1}/{2}", schemeId, i, schemeIdArray.Length));
                        var r =new ActivityService().ManualAddMoney(schemeId);
                        this.AddLogToList(r.Message);
                    }
                    catch (Exception ex)
                    {
                        this.AddLogToList(ex.Message);
                    }
                }
                this.AddLogToList("===全部加载完成===");
            });
        }

        /// <summary>
        /// 批量撤单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBetFail_Click(object sender, EventArgs e)
        {
            var schemeIdArray = rich_betFail_schemeId.Text.Trim().Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            Tools.ExcuteByTimer(1000, () =>
            {
                for (int i = 0; i < schemeIdArray.Length; i++)
                {
                    var schemeId = schemeIdArray[i];
                    try
                    {
                        this.AddLogToList(string.Format("开始处理订单{0}.{1}/{2}", schemeId, i, schemeIdArray.Length));
                        var r = new GameBizWcfService_Core().BetFails(schemeId);
                        this.AddLogToList(r.Message);
                    }
                    catch (Exception ex)
                    {
                        this.AddLogToList(ex.Message);
                    }
                }
                this.AddLogToList("===全部加载完成===");
            });
            
        }

        private void fpContent_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}

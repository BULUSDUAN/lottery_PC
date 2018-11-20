using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RedisBusinessManager
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        #region 任务

        //private IAutoTask task_split_ticket = new AutoTask_SplitTicket();
        //private IAutoTask task_split_ticket_single = new AutoTask_SplitTicket_Single();
        //private IAutoTask task_prize_szc = new AutoTask_Prize_SZC();

        #endregion

        #region 拆票

        private void bt_split_ticket_start_Click(object sender, EventArgs e)
        {
            //Tools.ExcuteByTimer(1000, () =>
            //{
            //    task_split_ticket.Start();
            //});
            this.bt_split_ticket_start.Enabled = false;
            this.bt_split_ticket_stop.Enabled = true;
        }

        private void bt_split_ticket_stop_Click(object sender, EventArgs e)
        {
            //task_split_ticket.Stop();

            this.bt_split_ticket_start.Enabled = true;
            this.bt_split_ticket_stop.Enabled = false;
        }

        private void bt_split_ticket_single_start_Click(object sender, EventArgs e)
        {
            //Tools.ExcuteByTimer(1000, () =>
            //{
            //    task_split_ticket_single.Start();
            //});

            this.bt_split_ticket_single_start.Enabled = false;
            this.bt_split_ticket_single_stop.Enabled = true;
        }

        private void bt_split_ticket_single_stop_Click(object sender, EventArgs e)
        {
            //task_split_ticket_single.Stop();

            this.bt_split_ticket_single_start.Enabled = true;
            this.bt_split_ticket_single_stop.Enabled = false;
        }

        #endregion

        #region 派奖

        private void bt_prize_szc_start_Click(object sender, EventArgs e)
        {
            //task_prize_szc.Start();

            this.bt_prize_szc_start.Enabled = false;
            this.bt_prize_szc_stop.Enabled = true;
        }

        private void bt_prize_szc_stop_Click(object sender, EventArgs e)
        {
            //task_prize_szc.Stop();
            this.bt_prize_szc_start.Enabled = true;
            this.bt_prize_szc_stop.Enabled = false;
        }

        #endregion

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }
    }
}

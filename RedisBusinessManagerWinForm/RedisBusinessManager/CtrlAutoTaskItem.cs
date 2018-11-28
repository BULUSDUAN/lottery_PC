using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RedisBusinessManager
{
    public partial class CtrlAutoTaskItem : UserControl
    {
        private IAutoTask _currentTask;
        /// <summary>
        /// 当前任务
        /// </summary>
        public IAutoTask CurrentTask
        {
            get { return _currentTask; }
        }

        public CtrlAutoTaskItem(IAutoTask task)
        {
            _currentTask = task;
            InitializeComponent();
        }

        private void CtrlAutoTaskItem_Load(object sender, EventArgs e)
        {
            var b = (CurrentTask as AutoTaskBase);
            this.groupBox4.Text = b.TaskName;
        }

        /// <summary>
        /// 开始任务
        /// </summary>
        public void StartTask()
        {
            if (!this.bt_start.Enabled)
                return;

            this.CurrentTask.Start();
            this.bt_start.Enabled = false;
            this.bt_stop.Enabled = true;
        }

        /// <summary>
        /// 停止任务
        /// </summary>
        public void StopTask()
        {
            this.CurrentTask.Stop();
            this.bt_start.Enabled = true;
            this.bt_stop.Enabled = false;
        }

        private void bt_start_Click(object sender, EventArgs e)
        {
            this.StartTask();
        }

        private void bt_stop_Click(object sender, EventArgs e)
        {
            this.StopTask();
        }
    }
}

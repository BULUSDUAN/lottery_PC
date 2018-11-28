namespace RedisBusinessManager
{
    partial class AutoTaskManager
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.fpContent = new System.Windows.Forms.FlowLayoutPanel();
            this.bt_start_all = new System.Windows.Forms.Button();
            this.bt_stop_all = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.bt_addMoney_start = new System.Windows.Forms.Button();
            this.rich_addmoney_schemeId = new System.Windows.Forms.RichTextBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.bt_do_repirChaseOrder = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.txt_repair_chaseOrderid = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.rich_un_prizeOrder = new System.Windows.Forms.RichTextBox();
            this.bt_load_runningOrder = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.rich_un_split_order = new System.Windows.Forms.RichTextBox();
            this.bt_load_un_split_order = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.bt_delete_order = new System.Windows.Forms.Button();
            this.rich_schemeid_all = new System.Windows.Forms.RichTextBox();
            this.bt_mu_do_prize = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.com_mu_gameCode = new System.Windows.Forms.ComboBox();
            this.list_log = new System.Windows.Forms.ListBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txt_fund_userId = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txt_fund_cache_date = new System.Windows.Forms.TextBox();
            this.bt_build_fund_cache = new System.Windows.Forms.Button();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.btnBetFail = new System.Windows.Forms.Button();
            this.rich_betFail_schemeId = new System.Windows.Forms.RichTextBox();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.SuspendLayout();
            // 
            // fpContent
            // 
            this.fpContent.AutoScroll = true;
            this.fpContent.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.fpContent.Location = new System.Drawing.Point(4, 97);
            this.fpContent.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.fpContent.Name = "fpContent";
            this.fpContent.Size = new System.Drawing.Size(1800, 681);
            this.fpContent.TabIndex = 0;
            this.fpContent.Paint += new System.Windows.Forms.PaintEventHandler(this.fpContent_Paint);
            // 
            // bt_start_all
            // 
            this.bt_start_all.Location = new System.Drawing.Point(20, 36);
            this.bt_start_all.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.bt_start_all.Name = "bt_start_all";
            this.bt_start_all.Size = new System.Drawing.Size(139, 29);
            this.bt_start_all.TabIndex = 1;
            this.bt_start_all.Text = "启动全部任务";
            this.bt_start_all.UseVisualStyleBackColor = true;
            this.bt_start_all.Click += new System.EventHandler(this.bt_start_all_Click);
            // 
            // bt_stop_all
            // 
            this.bt_stop_all.Location = new System.Drawing.Point(187, 36);
            this.bt_stop_all.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.bt_stop_all.Name = "bt_stop_all";
            this.bt_stop_all.Size = new System.Drawing.Size(160, 29);
            this.bt_stop_all.TabIndex = 1;
            this.bt_stop_all.Text = "停止全部任务";
            this.bt_stop_all.UseVisualStyleBackColor = true;
            this.bt_stop_all.Click += new System.EventHandler(this.bt_stop_all_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1816, 811);
            this.tabControl1.TabIndex = 2;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.bt_stop_all);
            this.tabPage1.Controls.Add(this.fpContent);
            this.tabPage1.Controls.Add(this.bt_start_all);
            this.tabPage1.Location = new System.Drawing.Point(4, 25);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabPage1.Size = new System.Drawing.Size(1808, 782);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "自动任务";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.groupBox6);
            this.tabPage2.Controls.Add(this.groupBox5);
            this.tabPage2.Controls.Add(this.groupBox3);
            this.tabPage2.Controls.Add(this.groupBox4);
            this.tabPage2.Controls.Add(this.groupBox2);
            this.tabPage2.Controls.Add(this.list_log);
            this.tabPage2.Controls.Add(this.groupBox1);
            this.tabPage2.Location = new System.Drawing.Point(4, 25);
            this.tabPage2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabPage2.Size = new System.Drawing.Size(1808, 782);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "手工任务";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.bt_addMoney_start);
            this.groupBox6.Controls.Add(this.rich_addmoney_schemeId);
            this.groupBox6.Location = new System.Drawing.Point(1496, 15);
            this.groupBox6.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox6.Size = new System.Drawing.Size(305, 342);
            this.groupBox6.TabIndex = 7;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "批量加奖";
            // 
            // bt_addMoney_start
            // 
            this.bt_addMoney_start.Location = new System.Drawing.Point(8, 26);
            this.bt_addMoney_start.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.bt_addMoney_start.Name = "bt_addMoney_start";
            this.bt_addMoney_start.Size = new System.Drawing.Size(100, 29);
            this.bt_addMoney_start.TabIndex = 14;
            this.bt_addMoney_start.Text = "开始";
            this.bt_addMoney_start.UseVisualStyleBackColor = true;
            this.bt_addMoney_start.Click += new System.EventHandler(this.bt_addMoney_start_Click);
            // 
            // rich_addmoney_schemeId
            // 
            this.rich_addmoney_schemeId.Location = new System.Drawing.Point(8, 78);
            this.rich_addmoney_schemeId.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.rich_addmoney_schemeId.Name = "rich_addmoney_schemeId";
            this.rich_addmoney_schemeId.Size = new System.Drawing.Size(288, 245);
            this.rich_addmoney_schemeId.TabIndex = 14;
            this.rich_addmoney_schemeId.Text = "";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.bt_do_repirChaseOrder);
            this.groupBox5.Controls.Add(this.label5);
            this.groupBox5.Controls.Add(this.txt_repair_chaseOrderid);
            this.groupBox5.Location = new System.Drawing.Point(11, 192);
            this.groupBox5.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox5.Size = new System.Drawing.Size(296, 165);
            this.groupBox5.TabIndex = 6;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "修复追号订单";
            // 
            // bt_do_repirChaseOrder
            // 
            this.bt_do_repirChaseOrder.Location = new System.Drawing.Point(96, 79);
            this.bt_do_repirChaseOrder.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.bt_do_repirChaseOrder.Name = "bt_do_repirChaseOrder";
            this.bt_do_repirChaseOrder.Size = new System.Drawing.Size(100, 29);
            this.bt_do_repirChaseOrder.TabIndex = 2;
            this.bt_do_repirChaseOrder.Text = "修复";
            this.bt_do_repirChaseOrder.UseVisualStyleBackColor = true;
            this.bt_do_repirChaseOrder.Click += new System.EventHandler(this.bt_do_repirChaseOrder_Click);
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(9, 30);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(79, 29);
            this.label5.TabIndex = 1;
            this.label5.Text = "订单号:";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txt_repair_chaseOrderid
            // 
            this.txt_repair_chaseOrderid.Location = new System.Drawing.Point(96, 30);
            this.txt_repair_chaseOrderid.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txt_repair_chaseOrderid.Name = "txt_repair_chaseOrderid";
            this.txt_repair_chaseOrderid.Size = new System.Drawing.Size(183, 25);
            this.txt_repair_chaseOrderid.TabIndex = 0;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.rich_un_prizeOrder);
            this.groupBox3.Controls.Add(this.bt_load_runningOrder);
            this.groupBox3.Location = new System.Drawing.Point(1148, 15);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox3.Size = new System.Drawing.Size(332, 342);
            this.groupBox3.TabIndex = 4;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "加载SQL库中未结算的订单到Redis";
            // 
            // rich_un_prizeOrder
            // 
            this.rich_un_prizeOrder.Location = new System.Drawing.Point(0, 66);
            this.rich_un_prizeOrder.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.rich_un_prizeOrder.Name = "rich_un_prizeOrder";
            this.rich_un_prizeOrder.Size = new System.Drawing.Size(323, 256);
            this.rich_un_prizeOrder.TabIndex = 13;
            this.rich_un_prizeOrder.Text = "";
            // 
            // bt_load_runningOrder
            // 
            this.bt_load_runningOrder.Location = new System.Drawing.Point(20, 26);
            this.bt_load_runningOrder.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.bt_load_runningOrder.Name = "bt_load_runningOrder";
            this.bt_load_runningOrder.Size = new System.Drawing.Size(100, 29);
            this.bt_load_runningOrder.TabIndex = 0;
            this.bt_load_runningOrder.Text = "开始";
            this.bt_load_runningOrder.UseVisualStyleBackColor = true;
            this.bt_load_runningOrder.Click += new System.EventHandler(this.bt_load_runningOrder_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.rich_un_split_order);
            this.groupBox4.Controls.Add(this.bt_load_un_split_order);
            this.groupBox4.Location = new System.Drawing.Point(803, 15);
            this.groupBox4.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox4.Size = new System.Drawing.Size(337, 342);
            this.groupBox4.TabIndex = 5;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "加载SQL库中未拆票的订单到Redis";
            // 
            // rich_un_split_order
            // 
            this.rich_un_split_order.Location = new System.Drawing.Point(8, 66);
            this.rich_un_split_order.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.rich_un_split_order.Name = "rich_un_split_order";
            this.rich_un_split_order.Size = new System.Drawing.Size(320, 256);
            this.rich_un_split_order.TabIndex = 14;
            this.rich_un_split_order.Text = "";
            // 
            // bt_load_un_split_order
            // 
            this.bt_load_un_split_order.Location = new System.Drawing.Point(8, 21);
            this.bt_load_un_split_order.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.bt_load_un_split_order.Name = "bt_load_un_split_order";
            this.bt_load_un_split_order.Size = new System.Drawing.Size(100, 29);
            this.bt_load_un_split_order.TabIndex = 0;
            this.bt_load_un_split_order.Text = "开始";
            this.bt_load_un_split_order.UseVisualStyleBackColor = true;
            this.bt_load_un_split_order.Click += new System.EventHandler(this.bt_load_un_split_order_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.bt_delete_order);
            this.groupBox2.Controls.Add(this.rich_schemeid_all);
            this.groupBox2.Controls.Add(this.bt_mu_do_prize);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.com_mu_gameCode);
            this.groupBox2.Location = new System.Drawing.Point(315, 8);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox2.Size = new System.Drawing.Size(453, 350);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "订单手工派奖";
            // 
            // bt_delete_order
            // 
            this.bt_delete_order.Location = new System.Drawing.Point(320, 74);
            this.bt_delete_order.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.bt_delete_order.Name = "bt_delete_order";
            this.bt_delete_order.Size = new System.Drawing.Size(100, 29);
            this.bt_delete_order.TabIndex = 13;
            this.bt_delete_order.Text = "删除订单";
            this.bt_delete_order.UseVisualStyleBackColor = true;
            this.bt_delete_order.Click += new System.EventHandler(this.bt_delete_order_Click);
            // 
            // rich_schemeid_all
            // 
            this.rich_schemeid_all.Location = new System.Drawing.Point(117, 61);
            this.rich_schemeid_all.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.rich_schemeid_all.Name = "rich_schemeid_all";
            this.rich_schemeid_all.Size = new System.Drawing.Size(160, 204);
            this.rich_schemeid_all.TabIndex = 12;
            this.rich_schemeid_all.Text = "";
            // 
            // bt_mu_do_prize
            // 
            this.bt_mu_do_prize.Location = new System.Drawing.Point(320, 29);
            this.bt_mu_do_prize.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.bt_mu_do_prize.Name = "bt_mu_do_prize";
            this.bt_mu_do_prize.Size = new System.Drawing.Size(100, 29);
            this.bt_mu_do_prize.TabIndex = 11;
            this.bt_mu_do_prize.Text = "执行派奖";
            this.bt_mu_do_prize.UseVisualStyleBackColor = true;
            this.bt_mu_do_prize.Click += new System.EventHandler(this.bt_mu_do_prize_Click);
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(19, 61);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(91, 29);
            this.label4.TabIndex = 9;
            this.label4.Text = "订单号:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(19, 29);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(91, 29);
            this.label3.TabIndex = 8;
            this.label3.Text = "彩种编码:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // com_mu_gameCode
            // 
            this.com_mu_gameCode.BackColor = System.Drawing.SystemColors.Window;
            this.com_mu_gameCode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.com_mu_gameCode.ForeColor = System.Drawing.SystemColors.WindowText;
            this.com_mu_gameCode.FormattingEnabled = true;
            this.com_mu_gameCode.Items.AddRange(new object[] {
            "JCZQ",
            "JCLQ",
            "BJDC"});
            this.com_mu_gameCode.Location = new System.Drawing.Point(117, 29);
            this.com_mu_gameCode.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.com_mu_gameCode.Name = "com_mu_gameCode";
            this.com_mu_gameCode.Size = new System.Drawing.Size(160, 23);
            this.com_mu_gameCode.TabIndex = 7;
            // 
            // list_log
            // 
            this.list_log.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.list_log.FormattingEnabled = true;
            this.list_log.ItemHeight = 15;
            this.list_log.Location = new System.Drawing.Point(4, 504);
            this.list_log.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.list_log.Name = "list_log";
            this.list_log.Size = new System.Drawing.Size(1800, 274);
            this.list_log.TabIndex = 2;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txt_fund_userId);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.txt_fund_cache_date);
            this.groupBox1.Controls.Add(this.bt_build_fund_cache);
            this.groupBox1.Location = new System.Drawing.Point(11, 8);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox1.Size = new System.Drawing.Size(280, 154);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "手工生成资金明细缓存";
            // 
            // txt_fund_userId
            // 
            this.txt_fund_userId.Location = new System.Drawing.Point(96, 76);
            this.txt_fund_userId.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txt_fund_userId.Name = "txt_fund_userId";
            this.txt_fund_userId.Size = new System.Drawing.Size(132, 25);
            this.txt_fund_userId.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 80);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(75, 15);
            this.label2.TabIndex = 2;
            this.label2.Text = "用户编号:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(41, 48);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 15);
            this.label1.TabIndex = 1;
            this.label1.Text = "日期:";
            // 
            // txt_fund_cache_date
            // 
            this.txt_fund_cache_date.Location = new System.Drawing.Point(96, 42);
            this.txt_fund_cache_date.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txt_fund_cache_date.Name = "txt_fund_cache_date";
            this.txt_fund_cache_date.Size = new System.Drawing.Size(132, 25);
            this.txt_fund_cache_date.TabIndex = 0;
            // 
            // bt_build_fund_cache
            // 
            this.bt_build_fund_cache.Location = new System.Drawing.Point(96, 110);
            this.bt_build_fund_cache.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.bt_build_fund_cache.Name = "bt_build_fund_cache";
            this.bt_build_fund_cache.Size = new System.Drawing.Size(109, 29);
            this.bt_build_fund_cache.TabIndex = 0;
            this.bt_build_fund_cache.Text = "执行";
            this.bt_build_fund_cache.UseVisualStyleBackColor = true;
            this.bt_build_fund_cache.Click += new System.EventHandler(this.bt_build_fund_cache_Click);
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.groupBox7);
            this.tabPage3.Location = new System.Drawing.Point(4, 25);
            this.tabPage3.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(1808, 782);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "订单手工派奖";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.btnBetFail);
            this.groupBox7.Controls.Add(this.rich_betFail_schemeId);
            this.groupBox7.Location = new System.Drawing.Point(11, 25);
            this.groupBox7.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox7.Size = new System.Drawing.Size(305, 342);
            this.groupBox7.TabIndex = 8;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "批量撤单";
            // 
            // btnBetFail
            // 
            this.btnBetFail.Location = new System.Drawing.Point(8, 26);
            this.btnBetFail.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnBetFail.Name = "btnBetFail";
            this.btnBetFail.Size = new System.Drawing.Size(100, 29);
            this.btnBetFail.TabIndex = 14;
            this.btnBetFail.Text = "开始";
            this.btnBetFail.UseVisualStyleBackColor = true;
            this.btnBetFail.Click += new System.EventHandler(this.btnBetFail_Click);
            // 
            // rich_betFail_schemeId
            // 
            this.rich_betFail_schemeId.Location = new System.Drawing.Point(8, 78);
            this.rich_betFail_schemeId.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.rich_betFail_schemeId.Name = "rich_betFail_schemeId";
            this.rich_betFail_schemeId.Size = new System.Drawing.Size(288, 245);
            this.rich_betFail_schemeId.TabIndex = 14;
            this.rich_betFail_schemeId.Text = "";
            // 
            // AutoTaskManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1816, 811);
            this.Controls.Add(this.tabControl1);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "AutoTaskManager";
            this.Text = "任务管理器";
            this.Load += new System.EventHandler(this.AutoTaskManager_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.groupBox6.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.groupBox7.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel fpContent;
        private System.Windows.Forms.Button bt_start_all;
        private System.Windows.Forms.Button bt_stop_all;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txt_fund_cache_date;
        private System.Windows.Forms.Button bt_build_fund_cache;
        private System.Windows.Forms.ListBox list_log;
        private System.Windows.Forms.TextBox txt_fund_userId;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button bt_mu_do_prize;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox com_mu_gameCode;
        private System.Windows.Forms.RichTextBox rich_schemeid_all;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button bt_load_runningOrder;
        private System.Windows.Forms.Button bt_load_un_split_order;
        private System.Windows.Forms.Button bt_delete_order;
        private System.Windows.Forms.RichTextBox rich_un_prizeOrder;
        private System.Windows.Forms.RichTextBox rich_un_split_order;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Button bt_do_repirChaseOrder;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txt_repair_chaseOrderid;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.Button bt_addMoney_start;
        private System.Windows.Forms.RichTextBox rich_addmoney_schemeId;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.Button btnBetFail;
        private System.Windows.Forms.RichTextBox rich_betFail_schemeId;
    }
}
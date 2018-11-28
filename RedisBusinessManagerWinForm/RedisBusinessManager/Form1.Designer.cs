namespace RedisBusinessManager
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.bt_split_ticket_single_stop = new System.Windows.Forms.Button();
            this.bt_split_ticket_single_start = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.bt_split_ticket_stop = new System.Windows.Forms.Button();
            this.bt_split_ticket_start = new System.Windows.Forms.Button();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.bt_prize_szc_stop = new System.Windows.Forms.Button();
            this.bt_prize_szc_start = new System.Windows.Forms.Button();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.groupBox10 = new System.Windows.Forms.GroupBox();
            this.button9 = new System.Windows.Forms.Button();
            this.button10 = new System.Windows.Forms.Button();
            this.groupBox9 = new System.Windows.Forms.GroupBox();
            this.button7 = new System.Windows.Forms.Button();
            this.button8 = new System.Windows.Forms.Button();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.bt_bjdc_stop = new System.Windows.Forms.Button();
            this.bt_bjdc_start = new System.Windows.Forms.Button();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.bt_ctzq_stop = new System.Windows.Forms.Button();
            this.bt_ctzq_start = new System.Windows.Forms.Button();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.bt_szc_winNumber_stop = new System.Windows.Forms.Button();
            this.bt_szc_winNumber_start = new System.Windows.Forms.Button();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.bt_bonusPool_stop = new System.Windows.Forms.Button();
            this.bt_bonusPool_start = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.bt_szc_bonusRule_stop = new System.Windows.Forms.Button();
            this.bt_szc_bonusRule_start = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.groupBox10.SuspendLayout();
            this.groupBox9.SuspendLayout();
            this.groupBox8.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
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
            this.tabControl1.Size = new System.Drawing.Size(1380, 736);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.groupBox2);
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Location = new System.Drawing.Point(4, 25);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabPage1.Size = new System.Drawing.Size(1372, 707);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "出票";
            this.tabPage1.UseVisualStyleBackColor = true;
            this.tabPage1.Click += new System.EventHandler(this.tabPage1_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.bt_split_ticket_single_stop);
            this.groupBox2.Controls.Add(this.bt_split_ticket_single_start);
            this.groupBox2.Location = new System.Drawing.Point(288, 20);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox2.Size = new System.Drawing.Size(239, 70);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "单式订单拆票";
            // 
            // bt_split_ticket_single_stop
            // 
            this.bt_split_ticket_single_stop.Enabled = false;
            this.bt_split_ticket_single_stop.Location = new System.Drawing.Point(116, 25);
            this.bt_split_ticket_single_stop.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.bt_split_ticket_single_stop.Name = "bt_split_ticket_single_stop";
            this.bt_split_ticket_single_stop.Size = new System.Drawing.Size(100, 29);
            this.bt_split_ticket_single_stop.TabIndex = 1;
            this.bt_split_ticket_single_stop.Text = "停止";
            this.bt_split_ticket_single_stop.UseVisualStyleBackColor = true;
            this.bt_split_ticket_single_stop.Click += new System.EventHandler(this.bt_split_ticket_single_stop_Click);
            // 
            // bt_split_ticket_single_start
            // 
            this.bt_split_ticket_single_start.Location = new System.Drawing.Point(8, 25);
            this.bt_split_ticket_single_start.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.bt_split_ticket_single_start.Name = "bt_split_ticket_single_start";
            this.bt_split_ticket_single_start.Size = new System.Drawing.Size(100, 29);
            this.bt_split_ticket_single_start.TabIndex = 0;
            this.bt_split_ticket_single_start.Text = "启动";
            this.bt_split_ticket_single_start.UseVisualStyleBackColor = true;
            this.bt_split_ticket_single_start.Click += new System.EventHandler(this.bt_split_ticket_single_start_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.bt_split_ticket_stop);
            this.groupBox1.Controls.Add(this.bt_split_ticket_start);
            this.groupBox1.Location = new System.Drawing.Point(11, 20);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox1.Size = new System.Drawing.Size(239, 70);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "普通订单拆票";
            // 
            // bt_split_ticket_stop
            // 
            this.bt_split_ticket_stop.Enabled = false;
            this.bt_split_ticket_stop.Location = new System.Drawing.Point(116, 25);
            this.bt_split_ticket_stop.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.bt_split_ticket_stop.Name = "bt_split_ticket_stop";
            this.bt_split_ticket_stop.Size = new System.Drawing.Size(100, 29);
            this.bt_split_ticket_stop.TabIndex = 1;
            this.bt_split_ticket_stop.Text = "停止";
            this.bt_split_ticket_stop.UseVisualStyleBackColor = true;
            this.bt_split_ticket_stop.Click += new System.EventHandler(this.bt_split_ticket_stop_Click);
            // 
            // bt_split_ticket_start
            // 
            this.bt_split_ticket_start.Location = new System.Drawing.Point(8, 25);
            this.bt_split_ticket_start.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.bt_split_ticket_start.Name = "bt_split_ticket_start";
            this.bt_split_ticket_start.Size = new System.Drawing.Size(100, 29);
            this.bt_split_ticket_start.TabIndex = 0;
            this.bt_split_ticket_start.Text = "启动";
            this.bt_split_ticket_start.UseVisualStyleBackColor = true;
            this.bt_split_ticket_start.Click += new System.EventHandler(this.bt_split_ticket_start_Click);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.groupBox3);
            this.tabPage2.Location = new System.Drawing.Point(4, 25);
            this.tabPage2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabPage2.Size = new System.Drawing.Size(1372, 707);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "派奖";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.bt_prize_szc_stop);
            this.groupBox3.Controls.Add(this.bt_prize_szc_start);
            this.groupBox3.Location = new System.Drawing.Point(11, 8);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox3.Size = new System.Drawing.Size(239, 70);
            this.groupBox3.TabIndex = 1;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "数字彩派奖";
            // 
            // bt_prize_szc_stop
            // 
            this.bt_prize_szc_stop.Enabled = false;
            this.bt_prize_szc_stop.Location = new System.Drawing.Point(116, 25);
            this.bt_prize_szc_stop.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.bt_prize_szc_stop.Name = "bt_prize_szc_stop";
            this.bt_prize_szc_stop.Size = new System.Drawing.Size(100, 29);
            this.bt_prize_szc_stop.TabIndex = 1;
            this.bt_prize_szc_stop.Text = "停止";
            this.bt_prize_szc_stop.UseVisualStyleBackColor = true;
            this.bt_prize_szc_stop.Click += new System.EventHandler(this.bt_prize_szc_stop_Click);
            // 
            // bt_prize_szc_start
            // 
            this.bt_prize_szc_start.Location = new System.Drawing.Point(8, 25);
            this.bt_prize_szc_start.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.bt_prize_szc_start.Name = "bt_prize_szc_start";
            this.bt_prize_szc_start.Size = new System.Drawing.Size(100, 29);
            this.bt_prize_szc_start.TabIndex = 0;
            this.bt_prize_szc_start.Text = "启动";
            this.bt_prize_szc_start.UseVisualStyleBackColor = true;
            this.bt_prize_szc_start.Click += new System.EventHandler(this.bt_prize_szc_start_Click);
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.groupBox10);
            this.tabPage3.Controls.Add(this.groupBox9);
            this.tabPage3.Controls.Add(this.groupBox8);
            this.tabPage3.Controls.Add(this.groupBox7);
            this.tabPage3.Controls.Add(this.groupBox6);
            this.tabPage3.Controls.Add(this.groupBox5);
            this.tabPage3.Controls.Add(this.groupBox4);
            this.tabPage3.Location = new System.Drawing.Point(4, 25);
            this.tabPage3.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(1372, 707);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "初始化数据";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // groupBox10
            // 
            this.groupBox10.Controls.Add(this.button9);
            this.groupBox10.Controls.Add(this.button10);
            this.groupBox10.Location = new System.Drawing.Point(11, 486);
            this.groupBox10.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox10.Name = "groupBox10";
            this.groupBox10.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox10.Size = new System.Drawing.Size(239, 70);
            this.groupBox10.TabIndex = 10;
            this.groupBox10.TabStop = false;
            this.groupBox10.Text = "竞彩足球比赛结果";
            // 
            // button9
            // 
            this.button9.Enabled = false;
            this.button9.Location = new System.Drawing.Point(116, 25);
            this.button9.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.button9.Name = "button9";
            this.button9.Size = new System.Drawing.Size(100, 29);
            this.button9.TabIndex = 1;
            this.button9.Text = "停止";
            this.button9.UseVisualStyleBackColor = true;
            // 
            // button10
            // 
            this.button10.Location = new System.Drawing.Point(8, 25);
            this.button10.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.button10.Name = "button10";
            this.button10.Size = new System.Drawing.Size(100, 29);
            this.button10.TabIndex = 0;
            this.button10.Text = "启动";
            this.button10.UseVisualStyleBackColor = true;
            // 
            // groupBox9
            // 
            this.groupBox9.Controls.Add(this.button7);
            this.groupBox9.Controls.Add(this.button8);
            this.groupBox9.Location = new System.Drawing.Point(11, 409);
            this.groupBox9.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox9.Name = "groupBox9";
            this.groupBox9.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox9.Size = new System.Drawing.Size(239, 70);
            this.groupBox9.TabIndex = 9;
            this.groupBox9.TabStop = false;
            this.groupBox9.Text = "竞彩篮球比赛结果";
            // 
            // button7
            // 
            this.button7.Enabled = false;
            this.button7.Location = new System.Drawing.Point(116, 25);
            this.button7.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(100, 29);
            this.button7.TabIndex = 1;
            this.button7.Text = "停止";
            this.button7.UseVisualStyleBackColor = true;
            // 
            // button8
            // 
            this.button8.Location = new System.Drawing.Point(8, 25);
            this.button8.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(100, 29);
            this.button8.TabIndex = 0;
            this.button8.Text = "启动";
            this.button8.UseVisualStyleBackColor = true;
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.bt_bjdc_stop);
            this.groupBox8.Controls.Add(this.bt_bjdc_start);
            this.groupBox8.Location = new System.Drawing.Point(11, 331);
            this.groupBox8.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox8.Size = new System.Drawing.Size(239, 70);
            this.groupBox8.TabIndex = 8;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "北京单场比赛结果";
            // 
            // bt_bjdc_stop
            // 
            this.bt_bjdc_stop.Enabled = false;
            this.bt_bjdc_stop.Location = new System.Drawing.Point(116, 25);
            this.bt_bjdc_stop.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.bt_bjdc_stop.Name = "bt_bjdc_stop";
            this.bt_bjdc_stop.Size = new System.Drawing.Size(100, 29);
            this.bt_bjdc_stop.TabIndex = 1;
            this.bt_bjdc_stop.Text = "停止";
            this.bt_bjdc_stop.UseVisualStyleBackColor = true;
            // 
            // bt_bjdc_start
            // 
            this.bt_bjdc_start.Location = new System.Drawing.Point(8, 25);
            this.bt_bjdc_start.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.bt_bjdc_start.Name = "bt_bjdc_start";
            this.bt_bjdc_start.Size = new System.Drawing.Size(100, 29);
            this.bt_bjdc_start.TabIndex = 0;
            this.bt_bjdc_start.Text = "启动";
            this.bt_bjdc_start.UseVisualStyleBackColor = true;
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.bt_ctzq_stop);
            this.groupBox7.Controls.Add(this.bt_ctzq_start);
            this.groupBox7.Location = new System.Drawing.Point(11, 254);
            this.groupBox7.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox7.Size = new System.Drawing.Size(239, 70);
            this.groupBox7.TabIndex = 7;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "传统足球比赛结果";
            // 
            // bt_ctzq_stop
            // 
            this.bt_ctzq_stop.Enabled = false;
            this.bt_ctzq_stop.Location = new System.Drawing.Point(116, 25);
            this.bt_ctzq_stop.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.bt_ctzq_stop.Name = "bt_ctzq_stop";
            this.bt_ctzq_stop.Size = new System.Drawing.Size(100, 29);
            this.bt_ctzq_stop.TabIndex = 1;
            this.bt_ctzq_stop.Text = "停止";
            this.bt_ctzq_stop.UseVisualStyleBackColor = true;
            // 
            // bt_ctzq_start
            // 
            this.bt_ctzq_start.Location = new System.Drawing.Point(8, 25);
            this.bt_ctzq_start.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.bt_ctzq_start.Name = "bt_ctzq_start";
            this.bt_ctzq_start.Size = new System.Drawing.Size(100, 29);
            this.bt_ctzq_start.TabIndex = 0;
            this.bt_ctzq_start.Text = "启动";
            this.bt_ctzq_start.UseVisualStyleBackColor = true;
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.bt_szc_winNumber_stop);
            this.groupBox6.Controls.Add(this.bt_szc_winNumber_start);
            this.groupBox6.Location = new System.Drawing.Point(11, 176);
            this.groupBox6.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox6.Size = new System.Drawing.Size(239, 70);
            this.groupBox6.TabIndex = 6;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "数字彩开奖号";
            // 
            // bt_szc_winNumber_stop
            // 
            this.bt_szc_winNumber_stop.Enabled = false;
            this.bt_szc_winNumber_stop.Location = new System.Drawing.Point(116, 25);
            this.bt_szc_winNumber_stop.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.bt_szc_winNumber_stop.Name = "bt_szc_winNumber_stop";
            this.bt_szc_winNumber_stop.Size = new System.Drawing.Size(100, 29);
            this.bt_szc_winNumber_stop.TabIndex = 1;
            this.bt_szc_winNumber_stop.Text = "停止";
            this.bt_szc_winNumber_stop.UseVisualStyleBackColor = true;
            // 
            // bt_szc_winNumber_start
            // 
            this.bt_szc_winNumber_start.Location = new System.Drawing.Point(8, 25);
            this.bt_szc_winNumber_start.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.bt_szc_winNumber_start.Name = "bt_szc_winNumber_start";
            this.bt_szc_winNumber_start.Size = new System.Drawing.Size(100, 29);
            this.bt_szc_winNumber_start.TabIndex = 0;
            this.bt_szc_winNumber_start.Text = "启动";
            this.bt_szc_winNumber_start.UseVisualStyleBackColor = true;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.bt_bonusPool_stop);
            this.groupBox5.Controls.Add(this.bt_bonusPool_start);
            this.groupBox5.Location = new System.Drawing.Point(11, 99);
            this.groupBox5.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox5.Size = new System.Drawing.Size(239, 70);
            this.groupBox5.TabIndex = 5;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "数字彩奖池";
            // 
            // bt_bonusPool_stop
            // 
            this.bt_bonusPool_stop.Enabled = false;
            this.bt_bonusPool_stop.Location = new System.Drawing.Point(116, 25);
            this.bt_bonusPool_stop.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.bt_bonusPool_stop.Name = "bt_bonusPool_stop";
            this.bt_bonusPool_stop.Size = new System.Drawing.Size(100, 29);
            this.bt_bonusPool_stop.TabIndex = 1;
            this.bt_bonusPool_stop.Text = "停止";
            this.bt_bonusPool_stop.UseVisualStyleBackColor = true;
            // 
            // bt_bonusPool_start
            // 
            this.bt_bonusPool_start.Location = new System.Drawing.Point(8, 25);
            this.bt_bonusPool_start.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.bt_bonusPool_start.Name = "bt_bonusPool_start";
            this.bt_bonusPool_start.Size = new System.Drawing.Size(100, 29);
            this.bt_bonusPool_start.TabIndex = 0;
            this.bt_bonusPool_start.Text = "启动";
            this.bt_bonusPool_start.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.bt_szc_bonusRule_stop);
            this.groupBox4.Controls.Add(this.bt_szc_bonusRule_start);
            this.groupBox4.Location = new System.Drawing.Point(11, 21);
            this.groupBox4.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox4.Size = new System.Drawing.Size(239, 70);
            this.groupBox4.TabIndex = 4;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "数字彩中奖规则";
            // 
            // bt_szc_bonusRule_stop
            // 
            this.bt_szc_bonusRule_stop.Enabled = false;
            this.bt_szc_bonusRule_stop.Location = new System.Drawing.Point(116, 25);
            this.bt_szc_bonusRule_stop.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.bt_szc_bonusRule_stop.Name = "bt_szc_bonusRule_stop";
            this.bt_szc_bonusRule_stop.Size = new System.Drawing.Size(100, 29);
            this.bt_szc_bonusRule_stop.TabIndex = 1;
            this.bt_szc_bonusRule_stop.Text = "停止";
            this.bt_szc_bonusRule_stop.UseVisualStyleBackColor = true;
            // 
            // bt_szc_bonusRule_start
            // 
            this.bt_szc_bonusRule_start.Location = new System.Drawing.Point(8, 25);
            this.bt_szc_bonusRule_start.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.bt_szc_bonusRule_start.Name = "bt_szc_bonusRule_start";
            this.bt_szc_bonusRule_start.Size = new System.Drawing.Size(100, 29);
            this.bt_szc_bonusRule_start.TabIndex = 0;
            this.bt_szc_bonusRule_start.Text = "启动";
            this.bt_szc_bonusRule_start.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1380, 736);
            this.Controls.Add(this.tabControl1);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.Text = "Redis运行工具";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.groupBox10.ResumeLayout(false);
            this.groupBox9.ResumeLayout(false);
            this.groupBox8.ResumeLayout(false);
            this.groupBox7.ResumeLayout(false);
            this.groupBox6.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button bt_split_ticket_stop;
        private System.Windows.Forms.Button bt_split_ticket_start;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button bt_split_ticket_single_stop;
        private System.Windows.Forms.Button bt_split_ticket_single_start;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button bt_prize_szc_stop;
        private System.Windows.Forms.Button bt_prize_szc_start;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Button bt_bonusPool_stop;
        private System.Windows.Forms.Button bt_bonusPool_start;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button bt_szc_bonusRule_stop;
        private System.Windows.Forms.Button bt_szc_bonusRule_start;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.Button bt_szc_winNumber_stop;
        private System.Windows.Forms.Button bt_szc_winNumber_start;
        private System.Windows.Forms.GroupBox groupBox10;
        private System.Windows.Forms.Button button9;
        private System.Windows.Forms.Button button10;
        private System.Windows.Forms.GroupBox groupBox9;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.GroupBox groupBox8;
        private System.Windows.Forms.Button bt_bjdc_stop;
        private System.Windows.Forms.Button bt_bjdc_start;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.Button bt_ctzq_stop;
        private System.Windows.Forms.Button bt_ctzq_start;
    }
}


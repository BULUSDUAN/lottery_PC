namespace KaSon.FrameWork.Services
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.IO;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Excel导出参数配置
    /// </summary>
    public class ExcelExporterParameter
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ExcelExporterParameter()
        {
            string str = Guid.NewGuid().ToString().Replace("_", "");
            this.DocumentFileName = @"ExportFiles\" + DateTime.Now.ToString("yyyyMMdd") + @"\" + str + ".xls";
            this.DocumentFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, this.DocumentFileName);
            this.DataTitle = "";
            this.DataSubTitle = "";
            this.DisplayGridlines = false;
            this.GroupHeaders = new List<GroupHeaderConfig>();
            this.CommonHeaders = new List<CommonHeaderConfig>();
        }

        /// <summary>
        /// 数据表头列，支持多表头(Id,DisplayName,OrderNo,isLayer,ParentId,Level) 
        /// Id,显示名称，层内排序号，是否是分层（1是0否），父Id，层数（最上层为1）
        /// </summary>
        public List<CommonHeaderConfig> CommonHeaders { get; set; }

        /// <summary>
        /// 需要导出的数据, 如果有分组表头，分组列需要放在DataBody最靠前的列
        /// </summary>
        public DataTable DataBody { get; set; }

        /// <summary>
        /// 副标题
        /// </summary>
        public string DataSubTitle { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string DataTitle { get; set; }

        /// <summary>
        /// 导出文件是否显示网格线
        /// </summary>
        public bool DisplayGridlines { get; set; }

        /// <summary>
        /// 导出文件名(绝对路径)
        /// </summary>
        public string DocumentFileName { get; set; }

        /// <summary>
        /// 分组表头列,内容相同自动合并(DisplayName)
        /// </summary>
        public List<GroupHeaderConfig> GroupHeaders { get; set; }
    }
}


namespace KaSon.FrameWork.Services
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// 普通列配置
    /// </summary>
    public class CommonHeaderConfig
    {
        public CommonHeaderConfig()
        {
            this.Children = new List<CommonHeaderConfig>();
            this.ColumnWidth = 10;
        }

        public List<CommonHeaderConfig> Children { get; set; }

        /// <summary>
        /// 表头单元格的标注信息
        /// </summary>
        public string ColumnComment { get; set; }

        /// <summary>
        /// 数据列名称
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        /// 数据列宽度,X个字符宽
        /// </summary>
        public int ColumnWidth { get; set; }

        /// <summary>
        /// 数据格式
        /// </summary>
        public string DataFormat { get; set; }

        /// <summary>
        /// 数据类型，默认为字符串，如果为图片则设置为IMAGE
        /// </summary>
        public ExcelDataType DataType { get; set; }

        /// <summary>
        /// 显示名称
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 水平对齐方式
        /// </summary>
        public HorizontalAlignment HorizontalAlignment { get; set; }
    }
}


namespace KaSon.FrameWork.Services
{
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// 分组列配置
    /// </summary>
    public class GroupHeaderConfig
    {
        /// <summary>
        /// 表头单元格的标注信息
        /// </summary>
        public string ColumnComment { get; set; }

        /// <summary>
        /// 数据列名
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        /// 数据列宽度,X个字符宽
        /// </summary>
        public int ColumnWidth { get; set; }

        /// <summary>
        /// 显示名称
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 水平对齐方式
        /// </summary>
        public KaSon.FrameWork.Services.HorizontalAlignment HorizontalAlignment { get; set; }
    }
}


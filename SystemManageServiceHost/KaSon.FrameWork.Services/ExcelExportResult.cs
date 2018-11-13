namespace KaSon.FrameWork.Services
{
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Excel导出结果
    /// </summary>
    public class ExcelExportResult
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ExcelExportResult()
        {
            this.ExportStatus = KaSon.FrameWork.Services.ExportStatus.Success;
        }

        /// <summary>
        /// 错误异常信息（成功时为NULL）
        /// </summary>
        public System.Exception Exception { get; set; }

        /// <summary>
        /// 导出文件的文件名(含路径)
        /// </summary>
        public string ExportFileName { get; set; }

        /// <summary>
        /// 导出结果状态
        /// </summary>
        public KaSon.FrameWork.Services.ExportStatus ExportStatus { get; set; }
    }
}


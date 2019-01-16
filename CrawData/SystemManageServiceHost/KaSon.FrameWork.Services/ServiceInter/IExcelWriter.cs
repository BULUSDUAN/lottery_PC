namespace KaSon.FrameWork.Services.ServiceInter
{
    public interface IExcelWriter
    {
        /// <summary>
        /// 导出 xls 文件
        /// </summary>
        /// <param name="parameter">导出参数</param>
        /// <returns></returns>
        ExcelExportResult Write(ExcelExporterParameter parameter);
    }
}


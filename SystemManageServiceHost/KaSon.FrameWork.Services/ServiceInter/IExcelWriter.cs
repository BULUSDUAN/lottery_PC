namespace KaSon.FrameWork.Services.ServiceInter
{
    public interface IExcelWriter
    {
        /// <summary>
        /// ���� xls �ļ�
        /// </summary>
        /// <param name="parameter">��������</param>
        /// <returns></returns>
        ExcelExportResult Write(ExcelExporterParameter parameter);
    }
}


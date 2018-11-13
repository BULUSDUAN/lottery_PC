namespace KaSon.FrameWork.Services.ServiceInter
{
    using System;
    using System.Data;
    using System.Runtime.InteropServices;

    /// <summary>
    /// 读取Excel文件的接口
    /// </summary>
    public interface IExcelReader
    {
        /// <summary>
        /// 使用一个excel文件路径初始化阅读器对象
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns>返回导入器对象自身用以支持链式调用</returns>
        IExcelReader Init(string path);
        /// <summary>
        /// 读取sheet中指定单元格的文本内容
        /// </summary>
        /// <param name="sheetIndex">sheet序号，从0开始</param>
        /// <param name="row">单元格所在的行号,从0开始</param>
        /// <param name="col">单元格所在的列号,从0开始</param>
        /// <returns>单元格的文本内容</returns>
        object ReadCell(int sheetIndex, int row, int col);
        /// <summary>
        /// 读取sheet中指定单元格的批注内容
        /// </summary>
        /// <param name="sheetIndex">sheet序号，从0开始</param>
        /// <param name="row">单元格所在的行号,从0开始</param>
        /// <param name="col">单元格所在的行号,从0开始</param>
        /// <returns>单元格的批注内容</returns>
        string ReadCellComment(int sheetIndex, int row, int col);
        /// <summary>
        /// 将指定sheet的一个区域读取到datatable对象
        /// </summary>
        /// <param name="sheetIndex">sheet序号，从0开始</param>
        /// <param name="startRow">区域开始的行号，从0开始</param>
        /// <param name="startCol">区域开始的列号，从0开始</param>
        /// <param name="endRow">结束行号</param>
        /// <param name="endCol">结束列号</param>
        /// <returns>datatable</returns>
        DataTable ReadRegion(int sheetIndex, int startRow, int startCol, int endRow = 0, int endCol = 0);
    }
}


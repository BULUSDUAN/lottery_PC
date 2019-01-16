using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KaSon.FrameWork.Service;
using System.IO;

namespace KaSon.FrameWork.Services.ServiceInter
{
    public interface IDfsClinet
    {
        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="config"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        bool DeleteFile(DfsTrackerInfo config, string filename);
        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="config"></param>
        /// <param name="fileName"></param>
        /// <param name="localfileName">下载到本地的本地文件名，如果已经存在则自动删除</param>
        /// <param name="bufferSize">下载缓冲区大小,默认128K字节</param>
        /// <returns></returns>
        bool DownloadFile(DfsTrackerInfo config, string fileName, string localfileName, int bufferSize = 0x20000);
        /// <summary>
        /// 获取文件信息
        /// </summary>
        /// <param name="config"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        DfsFileInfo GetDfsFileInfo(DfsTrackerInfo config, string filename);
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="config"></param>
        /// <param name="content"></param>
        /// <param name="fileExt">不带.的文件扩展名,如：jpg,doc等</param>
        /// <returns></returns>
        string UploadFile(DfsTrackerInfo config, byte[] content, string fileExt);
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="config"></param>
        /// <param name="stream"></param>
        /// <param name="fileExt">不带.的文件扩展名,如：jpg,doc等</param>
        /// <returns></returns>
        string UploadFile(DfsTrackerInfo config, Stream stream, string fileExt);
    }
}

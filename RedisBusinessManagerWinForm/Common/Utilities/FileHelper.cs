﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Common.Utilities
{
    public static class FileHelper
    {
        public static void CopyDirectory(string strFromPath, string strToPath, Func<string, string, bool> beforeCopyFunc)
        {
            if (!Directory.Exists(strFromPath))
            {
                return;
            }
            if (!Directory.Exists(strToPath))
            {
                Directory.CreateDirectory(strToPath);
            }
            string strFolderName = strFromPath.Substring(strFromPath.LastIndexOf("\\") + 1, strFromPath.Length - strFromPath.LastIndexOf("\\") - 1);
            //创建数组保存源文件夹下的文件名 
            string[] strfiles = Directory.GetFiles(strFromPath);

            //循环拷贝文件 
            for (int i = 0; i < strfiles.Length; i++)
            {
                //取得拷贝的文件名，只取文件名，地址截掉。 
                string strFileName = strfiles[i].Substring(strfiles[i].LastIndexOf("\\") + 1, strfiles[i].Length - strfiles[i].LastIndexOf("\\") - 1);
                if (beforeCopyFunc != null && beforeCopyFunc(strfiles[i], strToPath + "\\" + strFileName))
                {
                    //开始拷贝文件,true表示覆盖同名文件
                    File.Copy(strfiles[i], strToPath + "\\" + strFileName, true);
                }
            }

            //创建directoryinfo实例 
            DirectoryInfo dirinfo = new DirectoryInfo(strFromPath);
            //取得源文件夹下的所有子文件夹名称 
            DirectoryInfo[] zipath = dirinfo.GetDirectories();
            for (int j = 0; j < zipath.Length; j++)
            {
                //获取所有子文件夹名 
                string strzipath = strFromPath + "\\" + zipath[j].ToString();
                if (beforeCopyFunc != null && beforeCopyFunc(strzipath, strToPath))
                {
                    //把得到的子文件夹当成新的源文件夹，从头开始新一轮的拷贝 
                    CopyChildDirectory(strzipath, strToPath, beforeCopyFunc);
                }
            }
        }
        public static void CopyChildDirectory(string strFromPath, string strToPath, Func<string, string, bool> beforeCopyFunc)
        {
            if (!Directory.Exists(strFromPath))
            {
                return;
            }

            string strFolderName = strFromPath.Substring(strFromPath.LastIndexOf("\\") + 1, strFromPath.Length - strFromPath.LastIndexOf("\\") - 1);
            //如果目标文件夹中没有源文件夹则在目标文件夹中创建源文件夹 
            if (!Directory.Exists(strToPath + "\\" + strFolderName))
            {
                Directory.CreateDirectory(strToPath + "\\" + strFolderName);
            }
            //创建数组保存源文件夹下的文件名 
            string[] strfiles = Directory.GetFiles(strFromPath);

            //循环拷贝文件 
            for (int i = 0; i < strfiles.Length; i++)
            {
                //取得拷贝的文件名，只取文件名，地址截掉。 
                string strFileName = strfiles[i].Substring(strfiles[i].LastIndexOf("\\") + 1, strfiles[i].Length - strfiles[i].LastIndexOf("\\") - 1);
                if (beforeCopyFunc != null && beforeCopyFunc(strfiles[i], strToPath + "\\" + strFolderName + "\\" + strFileName))
                {
                    //开始拷贝文件,true表示覆盖同名文件 
                    File.Copy(strfiles[i], strToPath + "\\" + strFolderName + "\\" + strFileName, true);
                }
            }

            //创建directoryinfo实例 
            DirectoryInfo dirinfo = new DirectoryInfo(strFromPath);
            //取得源文件夹下的所有子文件夹名称 
            DirectoryInfo[] zipath = dirinfo.GetDirectories();
            for (int j = 0; j < zipath.Length; j++)
            {
                //获取所有子文件夹名 
                string strzipath = strFromPath + "\\" + zipath[j].ToString();
                //把得到的子文件夹当成新的源文件夹，从头开始新一轮的拷贝 
                CopyChildDirectory(strzipath, strToPath + "\\" + strFolderName, beforeCopyFunc);
            }
        }
    }
}

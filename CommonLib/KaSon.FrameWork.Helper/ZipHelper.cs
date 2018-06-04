namespace KaSon.FrameWork.Helper
{
   // using ICSharpCode.SharpZipLib.Zip;
    using System;
    using System.IO;

    public static class ZipHelper
    {
        //public static bool AddFileToZip(string zipfile, string[] files)
        //{
        //    using (ZipFile file = new ZipFile(zipfile))
        //    {
        //        file.BeginUpdate();
        //        foreach (string str in files)
        //        {
        //            file.Add(str);
        //        }
        //        file.CommitUpdate();
        //    }
        //    return true;
        //}

        //public static bool CreateZipFile(string zipfile, string[] files)
        //{
        //    using (MemoryStream stream = new MemoryStream())
        //    {
        //        using (ZipOutputStream stream2 = new ZipOutputStream(stream))
        //        {
        //            stream2.SetLevel(9);
        //            foreach (string str in files)
        //            {
        //                byte[] buffer;
        //                using (FileStream stream3 = File.OpenRead(str))
        //                {
        //                    buffer = new byte[stream3.Length];
        //                    stream3.Read(buffer, 0, Convert.ToInt32(stream3.Length));
        //                    stream3.Close();
        //                }
        //                ZipEntry entry = new ZipEntry(new FileInfo(str).Name);
        //                stream2.PutNextEntry(entry);
        //                stream2.Write(buffer, 0, buffer.Length);
        //            }
        //            stream2.Finish();
        //            stream2.Close();
        //            File.WriteAllBytes(zipfile, stream.ToArray());
        //        }
        //    }
        //    return true;
        //}

        //public static bool CreateZipFile(string zipfile, string dir)
        //{
        //    new FastZip { CreateEmptyDirectories = true }.CreateZip(zipfile, dir, true, "");
        //    return true;
        //}

        //public static bool ExtractZipFile(string zipfile, string dir)
        //{
        //    if (!Directory.Exists(dir))
        //    {
        //        Directory.CreateDirectory(dir);
        //    }
        //    new FastZip().ExtractZip(zipfile, dir, "");
        //    return true;
        //}

        //public static string[] ListZipFile(string zipfile)
        //{
        //    using (ZipFile file = new ZipFile(zipfile))
        //    {
        //        string[] strArray = new string[file.Count];
        //        int index = 0;
        //        foreach (ZipEntry entry in file)
        //        {
        //            strArray[index] = entry.Name;
        //            index++;
        //        }
        //        return strArray;
        //    }
        //}

        //public static bool RemoveFileFromZip(string zipfile, string[] files)
        //{
        //    using (ZipFile file = new ZipFile(zipfile))
        //    {
        //        file.BeginUpdate();
        //        foreach (string str in files)
        //        {
        //            file.Delete(str);
        //        }
        //        file.CommitUpdate();
        //    }
        //    return true;
        //}
    }
}


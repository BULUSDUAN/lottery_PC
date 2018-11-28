using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.IISHelper
{
    public class IISVersionHelper
    {
        public static IISVersion GetIISVersion()
        {
            string regPath = @"software\microsoft\inetstp";
            Microsoft.Win32.RegistryKey pregkey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(regPath);
            if (null != pregkey)
            {
                // 判断IIS主版本号
                int MajorVersion = Convert.ToInt32(pregkey.GetValue("MajorVersion", 0));
                pregkey.Close();
                try
                {
                    return (IISVersion)MajorVersion;
                }
                catch
                {
                    return IISVersion.Unkonw;
                }
            }
            else
            {
                return IISVersion.None;
            }
        }
    }
}

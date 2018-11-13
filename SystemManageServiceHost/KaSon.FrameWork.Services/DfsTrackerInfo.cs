namespace KaSon.FrameWork.Service
{
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Tracker描述信息
    /// </summary>
    public sealed class DfsTrackerInfo
    {
        public DfsTrackerInfo()
        {
        }

        public DfsTrackerInfo(string group, string ip, int port, string fileSite)
        {
            this.Group = group;
            this.IP = ip;
            this.Port = port;
            this.FileSite = fileSite.TrimEnd(new char[] { '/' });
        }

        public string FileSite { get; set; }

        public string Group { get; set; }

        public string IP { get; set; }

        public int Port { get; set; }
    }
}


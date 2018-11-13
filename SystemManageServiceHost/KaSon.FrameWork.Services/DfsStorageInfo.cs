namespace KaSon.FrameWork.Service
{
    using System;

    public class DfsStorageInfo
    {
        public readonly string GroupName;
        public readonly string IP;
        public readonly int Port;
        public readonly byte Store_Path_Index;

        public DfsStorageInfo(string group, string ip, int port, byte storepathindex)
        {
            this.GroupName = group;
            this.IP = ip;
            this.Port = port;
            this.Store_Path_Index = storepathindex;
        }
    }
}


namespace KaSon.FrameWork.Service
{
    using System;

    /// <summary>
    /// Group描述信息
    /// </summary>
    public sealed class DfsGroupInfo
    {
        public readonly int ActiveServerCount;
        public readonly long Current_Trunk_FileId;
        public readonly int Current_Write_Server_Index;
        public readonly long FreeSpace;
        public const int GroupInfo_LEN = 0x61;
        public readonly string GroupName;
        public readonly int StorageServerCount;
        public readonly int StorageServerHttpPort;
        public readonly int StorageServerPort;
        public readonly int StorePathCount;
        public readonly int SubdirCount;
        public readonly long TrunkFreeSpace;

        public DfsGroupInfo(string group, long freespace, long trunkfreespace, int storagecount, int storageport, int httpport, int activeservercount, int currentwriteindex, int storepathcount, int subdircount, long currenttrunkfileId)
        {
            this.GroupName = group;
            this.FreeSpace = freespace;
            this.TrunkFreeSpace = trunkfreespace;
            this.StorageServerCount = storagecount;
            this.StorageServerPort = storageport;
            this.StorageServerHttpPort = httpport;
            this.ActiveServerCount = activeservercount;
            this.Current_Write_Server_Index = currentwriteindex;
            this.StorePathCount = storepathcount;
            this.SubdirCount = subdircount;
            this.Current_Trunk_FileId = currenttrunkfileId;
        }
    }
}


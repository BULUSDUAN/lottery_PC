using EntityModel.Enum;
using EntityModel.Xml;

using System;
using System.Collections.Generic;
using System.Text;

namespace EntityModel.Xml.Model
{
    #region 定制跟单

    [XmlMapping("item", 0)]
    public class ProfileFollowerInfo : XmlMappingObject
    {
        [XmlMapping("gameCode", 1, MappingType = MappingType.Attribute)]
        public string GameCode { get; set; }
        [XmlMapping("gameType", 2, MappingType = MappingType.Attribute)]
        public string GameType { get; set; }
        [XmlMapping("gameDisplayName", 3, MappingType = MappingType.Attribute)]
        public string GameDisplayName { get; set; }
        [XmlMapping("followerCount", 4, MappingType = MappingType.Attribute)]
        public int FollowerCount { get; set; }
        [XmlMapping("updateTime", 10, MappingType = MappingType.Attribute)]
        public DateTime UpdateTime { get; set; }
    }

    public class ProfileFollowerCollection : XmlMappingList<ProfileFollowerInfo>
    {
    }
    #endregion
    #region 超级发起人
    /// <summary>
    /// 超级发起人信息
    /// </summary>
    //[CommunicationObject]
    //[XmlMapping("item", 0)]
    public class SupperCreatorInfo : XmlMappingObject
    {
        //   [XmlMapping("userId", 0, MappingType = MappingType.Attribute)]
        public string UserId { get; set; }
        // [XmlMapping("displayName", 1, MappingType = MappingType.Attribute)]
        public string DisplayName { get; set; }
        // [XmlMapping("isSupper", 2, MappingType = MappingType.Attribute)]
        public bool IsSupper { get; set; }
        //[XmlMapping("isStar", 3, MappingType = MappingType.Attribute)]
        public bool IsStar { get; set; }
        //[XmlMapping("hasTogetherScheme", 4, MappingType = MappingType.Attribute)]
        public bool HasTogetherScheme { get; set; }
        // [XmlMapping("lastStopTime", 5, MappingType = MappingType.Attribute)]
        public DateTime LastStopTime { get; set; }
    }
    // [CommunicationObject]
    public class SupperCreatorCollection : XmlMappingList<SupperCreatorInfo>
    {
    }
    #endregion
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;

namespace Kason.Net.Common.Communication
{
    /// <summary>
    /// 通信层的序列化
    /// </summary>
    public static class CommunicationSerializer
    {
        /// <summary>
        /// 序列化一个datacontract属性的对象
        /// </summary>
        /// <param name="obj">要序列化的对象</param>
        /// <param name="rootName">xml字典的根名称,如:MyObject</param>
        /// <param name="rootNameSpace">xml的根命名空间,如:http://www.lhbis.com</param>
        /// <param name="knownTypes">其中包含可在对象图中呈现的类型</param>
        /// <returns>字节数组</returns>
        /// <remarks>只有在程序集里同一个命名空间下的对象才能互相序列化/反序列化. 否则反序列化可能不成功</remarks>
        public static byte[] SerializeDataContract(object obj, string rootName, string rootNameSpace, IEnumerable<Type> knownTypes)
        {
            var dictionary = new XmlDictionary();
            var rootString = dictionary.Add(rootName);
            var namesapceString = dictionary.Add(rootNameSpace);
            using (var ms = new MemoryStream())
            {
                using (var bw = XmlDictionaryWriter.CreateBinaryWriter(ms, dictionary))
                {
                    var ser = new DataContractSerializer(obj.GetType(), rootString, namesapceString, knownTypes);
                    ser.WriteObject(bw, obj);
                    bw.Flush();

                    ms.Position = 0;
                    return ms.ToArray();
                }
            }
        }
        /// <summary>
        /// 反序列化一个datacontract属性的对象
        /// </summary>
        /// <param name="type">要反序列化的对象的类型</param>
        /// <param name="rootName">xml字典的根名称,如:MyObject</param>
        /// <param name="rootNameSpace">xml的根命名空间,如:http://www.lhbis.com</param>
        /// <param name="knownTypes">其中包含可在对象图中呈现的类型</param>
        /// <param name="buffer">要反序列化为对象的数据</param>
        /// <returns>object对象</returns>
        /// <remarks>只有在程序集里同一个命名空间下的对象才能互相序列化/反序列化. 否则反序列化可能不成功</remarks>
        public static object DeserializeDataContract(Type type, string rootName, string rootNameSpace, IEnumerable<Type> knownTypes, byte[] buffer)
        {
            var dictionary = new XmlDictionary();
            var rootString = dictionary.Add(rootName);
            var namesapceString = dictionary.Add(rootNameSpace);
            using (var br = XmlDictionaryReader.CreateBinaryReader(buffer, 0, buffer.Length, dictionary, XmlDictionaryReaderQuotas.Max))
            {
                var ser = new DataContractSerializer(type, rootString, namesapceString, knownTypes);
                return ser.ReadObject(br);
            }
        }
    }
}

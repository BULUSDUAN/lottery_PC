using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Reflection;
using System.Collections;

using EntityModel.Enum;

namespace EntityModel.Xml
{
   
    public abstract class XmlMappingObject
    {
        public string XmlHeader { get; set; }

        public XmlMappingObject()
        {
            XmlHeader = "<?xml version=\"1.0\" encoding=\"utf-8\"?>";
        }

        public virtual void AnalyzeXmlNode(XmlNode xmlNode)
        {
            if (xmlNode != null)
            {
                foreach (PropertyInfo info in this.GetPropertyInfoList())
                {
                    object[] customAttributes = info.GetCustomAttributes(typeof(XmlMappingAttribute), true);
                    if (customAttributes.Length > 0)
                    {
                        XmlMappingAttribute attribute = customAttributes[0] as XmlMappingAttribute;
                        if (info.PropertyType.IsSubclassOf(typeof(XmlMappingObject)))
                        {
                            XmlNode nodeList;
                            XmlMappingObject obj2 = info.PropertyType.GetConstructor(Type.EmptyTypes).Invoke(null) as XmlMappingObject;
                            if (attribute.ObjectType == XmlObjectType.List)
                            {
                                nodeList = this.GetNodeList(xmlNode, attribute.MappingName);
                            }
                            else
                            {
                                nodeList = this.GetChildNode(xmlNode, attribute.MappingName);
                            }
                            obj2.AnalyzeXmlNode(nodeList ?? xmlNode);
                            info.SetValue(this, obj2, null);
                            continue;
                        }
                        if (attribute.ObjectType == XmlObjectType.List)
                        {
                            Type type = info.PropertyType.GetGenericArguments()[0];
                            IList list = info.PropertyType.GetConstructor(Type.EmptyTypes).Invoke(null) as IList;
                            XmlNode node2 = this.GetNodeList(xmlNode, attribute.MappingName);
                            if (type.IsSubclassOf(typeof(XmlMappingObject)))
                            {
                                foreach (XmlNode node3 in node2.FirstChild.ChildNodes)
                                {
                                    XmlMappingObject obj3 = type.GetConstructor(Type.EmptyTypes).Invoke(null) as XmlMappingObject;
                                    obj3.AnalyzeXmlNode(node3);
                                    list.Add(obj3);
                                }
                            }
                            else
                            {
                                foreach (XmlNode node4 in node2.ChildNodes)
                                {
                                    list.Add(this.GetNodeValue(node4, attribute.MappingType));
                                }
                            }
                            info.SetValue(this, list, null);
                            continue;
                        }
                        if (info.PropertyType.IsEnum)
                        {
                            object obj4 = System.Enum.Parse(info.PropertyType, this.GetNodeValue(xmlNode, attribute.MappingName, attribute.MappingType));
                            info.SetValue(this, obj4, null);
                        }
                        else
                        {
                            if (info.PropertyType.Equals(typeof(string)))
                            {
                                info.SetValue(this, this.GetNodeValue(xmlNode, attribute.MappingName, attribute.MappingType), null);
                                continue;
                            }
                            string str = this.GetNodeValue(xmlNode, attribute.MappingName, attribute.MappingType);
                            if (!string.IsNullOrEmpty(str))
                            {
                                Type conversionType = Nullable.GetUnderlyingType(info.PropertyType) ?? info.PropertyType;
                                info.SetValue(this, Convert.ChangeType(str, conversionType), null);
                            }
                        }
                    }
                }
            }
        }

        public virtual void BuildXmlNode(XmlDocument doc, ref XmlNode refNode, string subName)
        {
            foreach (PropertyInfo info in this.GetPropertyInfoList())
            {
                object[] customAttributes = info.GetCustomAttributes(typeof(XmlMappingAttribute), true);
                if (customAttributes.Length > 0)
                {
                    XmlMappingAttribute attribute = customAttributes[0] as XmlMappingAttribute;
                    if (info.PropertyType.IsSubclassOf(typeof(XmlMappingObject)))
                    {
                        XmlMappingObject obj2 = info.GetValue(this, null) as XmlMappingObject;
                        if (attribute.ObjectType == XmlObjectType.List)
                        {
                            obj2.BuildXmlNode(doc, ref refNode, attribute.MappingName);
                        }
                        else
                        {
                            XmlNode node = this.CreateElement(doc, attribute.MappingName, "");
                            obj2.BuildXmlNode(doc, ref node, attribute.MappingName);
                            refNode.AppendChild(node);
                        }
                        continue;
                    }
                    if (attribute.ObjectType == XmlObjectType.List)
                    {
                        IList list = info.GetValue(this, null) as IList;
                        foreach (object obj3 in list)
                        {
                            if (obj3 is XmlMappingObject)
                            {
                                XmlMappingObject obj4 = obj3 as XmlMappingObject;
                                if (attribute.ObjectType == XmlObjectType.List)
                                {
                                    obj4.BuildXmlNode(doc, ref refNode, attribute.MappingName);
                                }
                                else
                                {
                                    XmlNode node2 = this.CreateElement(doc, attribute.MappingName, "");
                                    obj4.BuildXmlNode(doc, ref node2, attribute.MappingName);
                                    refNode.AppendChild(node2);
                                }
                                continue;
                            }
                            if (obj3 is string)
                            {
                                XmlNode newChild = this.CreateElement(doc, attribute.MappingName, obj3);
                                refNode.AppendChild(newChild);
                            }
                        }
                        continue;
                    }
                    if (info.PropertyType.IsEnum)
                    {
                        int num = (int)info.GetValue(this, null);
                        if (attribute.MappingType == MappingType.Attribute)
                        {
                            refNode.Attributes.Append(this.CreateAttribute(doc, attribute.MappingName, num));
                        }
                        else
                        {
                            refNode.AppendChild(this.CreateElement(doc, attribute.MappingName, num));
                        }
                        continue;
                    }
                    object obj5 = info.GetValue(this, null);
                    if (attribute.MappingType == MappingType.Attribute)
                    {
                        refNode.Attributes.Append(this.CreateAttribute(doc, attribute.MappingName, obj5));
                    }
                    else
                    {
                        refNode.AppendChild(this.CreateElement(doc, attribute.MappingName, obj5));
                    }
                }
            }
        }

        private XmlAttribute CreateAttribute(XmlDocument doc, string mappingName, object value)
        {
            XmlAttribute attribute = doc.CreateAttribute(mappingName);
            if (value != null)
            {
                attribute.Value = value.ToString();
            }
            return attribute;
        }

        private XmlNode CreateElement(XmlDocument doc, string mappingName, object value)
        {
            XmlElement element = doc.CreateElement(mappingName);
            if (value != null)
            {
                element.InnerText = value.ToString();
            }
            return element;
        }

        private XmlNode GetChildNode(XmlNode xmlNode, string childName)
        {
            foreach (XmlNode node in xmlNode.ChildNodes)
            {
                if (node.Name.Equals(childName, StringComparison.OrdinalIgnoreCase))
                {
                    return node;
                }
            }
            return null;
        }

        public virtual string GetCode()
        {
            return "000";
        }

        private XmlNode GetNodeList(XmlNode xmlNode, string childName)
        {
            XmlNode node = xmlNode.CloneNode(true);
            for (int i = node.ChildNodes.Count - 1; i >= 0; i--)
            {
                XmlNode oldChild = node.ChildNodes[i];
                if (!oldChild.Name.Equals(childName, StringComparison.OrdinalIgnoreCase))
                {
                    node.RemoveChild(oldChild);
                }
            }
            return node;
        }

        private string GetNodeValue(XmlNode xmlNode, MappingType type)
        {
            if (type == MappingType.Element)
            {
                return xmlNode.InnerText;
            }
            return xmlNode.Value;
        }

        private string GetNodeValue(XmlNode xmlNode, string mappingName, MappingType type)
        {
            if (type == MappingType.Element)
            {
                foreach (XmlNode node in xmlNode.ChildNodes)
                {
                    if (node.Name.Equals(mappingName, StringComparison.OrdinalIgnoreCase))
                    {
                        return node.InnerText;
                    }
                }
            }
            else
            {
                foreach (XmlAttribute attribute in xmlNode.Attributes)
                {
                    if (attribute.Name.Equals(mappingName, StringComparison.OrdinalIgnoreCase))
                    {
                        return attribute.Value;
                    }
                }
            }
            return null;
        }

        public virtual string GetPrimaryKey()
        {
            return base.GetType().Name;
        }

        private List<PropertyInfo> GetPropertyInfoList()
        {
            PropertyInfo[] properties = base.GetType().GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            List<PropertyInfo> list = new List<PropertyInfo>();
            foreach (PropertyInfo info in properties)
            {
                if (info.IsDefined(typeof(XmlMappingAttribute), true))
                {
                    list.Add(info);
                }
            }
            list.Sort(new XmlMappingComparer());
            return list;
        }

        public virtual string ToInnerXmlString(string tagName)
        {
            XmlDocument doc = new XmlDocument();
            XmlNode refNode = doc.CreateElement(tagName);
            this.BuildXmlNode(doc, ref refNode, refNode.Name);
            return refNode.InnerXml;
        }

        public virtual string ToOuterXmlString(string tagName)
        {
            XmlDocument doc = new XmlDocument();
            XmlNode refNode = doc.CreateElement(tagName);
            this.BuildXmlNode(doc, ref refNode, refNode.Name);
            return refNode.OuterXml;
        }

        public virtual string ToXmlString(string tagName)
        {
            XmlDocument doc = new XmlDocument();
            XmlNode refNode = doc.CreateElement(tagName);
            this.BuildXmlNode(doc, ref refNode, refNode.Name);
            return (this.XmlHeader + refNode.OuterXml);
        }

        private class XmlMappingComparer : IComparer<PropertyInfo>
        {
            public int Compare(PropertyInfo x, PropertyInfo y)
            {
                XmlMappingAttribute attribute = x.GetCustomAttributes(typeof(XmlMappingAttribute), true)[0] as XmlMappingAttribute;
                XmlMappingAttribute attribute2 = y.GetCustomAttributes(typeof(XmlMappingAttribute), true)[0] as XmlMappingAttribute;
                return attribute.Index.CompareTo(attribute2.Index);
            }
        }
    }

    public class XmlMappingList<T> : XmlMappingObject, IList<T>
        where T : XmlMappingObject, new()
    {
        private IList<T> _list;
        public XmlMappingList()
        {
            _list = new List<T>();
        }

        #region 接口 IList<T> 成员

        public void Add(T item)
        {
            _list.Add(item);
        }

        public void Remove(T item)
        {
            _list.Remove(item);
        }

        public void RemoveAt(int index)
        {
            _list.RemoveAt(index);
        }

        public int IndexOf(T item)
        {
            return _list.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            _list.Insert(index, item);
        }

        public T this[int index]
        {
            get { return _list[index]; }
            set { _list[index] = value; }
        }

        public void Clear()
        {
            _list.Clear();
        }

        public bool Contains(T item)
        {
            return _list.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _list.Count; }
        }

        public bool IsReadOnly
        {
            get { return _list.IsReadOnly; }
        }

        bool ICollection<T>.Remove(T item)
        {
            return _list.Remove(item);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        #endregion

        public void AddRange(IEnumerable<T> collection)
        {
            foreach (var item in collection)
            {
                Add(item);
            }
        }
        public override void AnalyzeXmlNode(XmlNode xmlNode)
        {
            foreach (XmlNode child in xmlNode.ChildNodes)
            {
                T t = new T();
                t.AnalyzeXmlNode(child);
                Add(t);
            }
        }
        public override void BuildXmlNode(XmlDocument doc, ref XmlNode refNode, string subName)
        {
            foreach (T item in this)
            {
                XmlNode node = CreateElement(doc, subName, "");
                item.BuildXmlNode(doc, ref node, subName);
                refNode.AppendChild(node);
            }
        }
        private XmlNode CreateElement(XmlDocument doc, string mappingName, object value)
        {
            XmlElement ele = doc.CreateElement(mappingName);
            ele.InnerText = value.ToString();
            return ele;
        }
    }
}

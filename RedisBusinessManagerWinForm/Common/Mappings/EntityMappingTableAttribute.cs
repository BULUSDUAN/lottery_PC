using System;

namespace Common.Mappings
{
    /// <summary>
    /// ʵ��������ע�����ڱ�עʵ����洢�����ݿ��еı���
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class EntityMappingTableAttribute : Attribute
    {
        /// <summary>
        /// ʵ����ӳ�䵽���ݿ�ı���������Ϊnull��մ�
        /// </summary>
        public string TableName { get; set; }
        /// <summary>
        /// ָ��ӳ��Ķ����Ƿ���һ��ֻ������
        /// �����ָ��Ϊֻ������,�ڶԴ˶���������ɾ���Ĳ���ʱ,���׳���Ӧ�쳣
        /// </summary>
        public bool ReadOnly { get; set; }
        /// <summary>
        /// ����ʵ����ӳ��ı�������������Ϊnull��մ�
        /// </summary>
        /// <param name="tableName">��־û�����ӳ������ݿ����</param>
        public EntityMappingTableAttribute(string tableName)
        {
            this.TableName = tableName;
            this.ReadOnly = false;
        }
    }
}
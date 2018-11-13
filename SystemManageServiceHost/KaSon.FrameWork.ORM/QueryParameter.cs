using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace KaSon.FrameWork.ORM
{
    internal sealed class QueryParameter
    {
        private string _alias;
        private readonly List<ColumnMap> _columns;

        private QueryParameter(QueryParameter paramter)
        {
            this._alias = "";
            this._columns = new List<ColumnMap>();
            ColumnMap[] array = new ColumnMap[paramter.Columns.Count];
            paramter.Columns.CopyTo(array, 0);
            this._columns = array.ToList<ColumnMap>();
            this.Name = paramter.Name;
            this.GetAliasFunc = paramter.GetAliasFunc;
            this.IsEntity = paramter.IsEntity;
            this.IsAnonym = paramter.IsAnonym;
            this.ParameterType = paramter.ParameterType;
            this.HasWhere = paramter.HasWhere;
            this.HasAs = paramter.HasAs;
            this.GroupBy = paramter.GroupBy;
            this.IsGroupKey = paramter.IsGroupKey;
        }

        public QueryParameter(string name, string providerId, Type type)
        {
            this._alias = "";
            this._columns = new List<ColumnMap>();
            this.Name = name;
            this.ProviderId = providerId;
            this.ParameterType = type;
            this.IsEntity = EntityHelper.IsEntity(type);
            this.IsAnonym = type.Name.Contains("f__AnonymousType");
            if (this.IsEntity)
            {
                this.SetEntityMap(type);
            }
            else
            {
                this.SetObjectMap(type);
            }
        }

        public ColumnMap GetColumn(string name)
        {
            int index = this.IndexOf(name);
            if (index == -1)
            {
                throw new Exception("不存在属性名为" + name + "的列!");
            }
            return this._columns[index];
        }

        private int IndexOf(string name)
        {
            int num = 0;
            foreach (ColumnMap map in this._columns)
            {
                if (string.Compare(name, map.Name, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    return num;
                }
                num++;
            }
            return -1;
        }
        /// <summary>
        /// 实体地图
        /// </summary>
        /// <param name="type"></param>
        private void SetEntityMap(Type type)
        {
            EntityInfo entityInfo = EntityHelper.GetEntityInfo(type);
            foreach (KeyValuePair<string, FieldMap> pair in entityInfo.Fields)
            {
                ColumnMap item = new ColumnMap
                {
                    Name = pair.Value.Property.Info.Name,
                    FieldMap = pair.Key
                };
                this._columns.Add(item);
            }
        }
        /// <summary>
        /// 属性地图
        /// </summary>
        /// <param name="type"></param>
        private void SetObjectMap(Type type)
        {
            foreach (PropertyInfo info in type.GetProperties())
            {
                ColumnMap item = new ColumnMap
                {
                    Name = info.Name,
                    FieldMap = info.Name
                };
                this._columns.Add(item);
            }
        }

        public QueryParameter ShallowCopy()
        {
            return new QueryParameter(this);
        }

        public string Alias
        {
            get
            {
                if ((this._alias == "") && (this.GetAliasFunc != null))
                {
                    this._alias = this.GetAliasFunc();
                }
                return this._alias;
            }
        }

        public IList<ColumnMap> Columns
        {
            get
            {
                return this._columns;
            }
        }

        public Func<string> GetAliasFunc { get; set; }

        public GroupByParam GroupBy { get; set; }

        public bool HasAs { get; set; }

        public bool HasWhere { get; set; }

        public bool IsAnonym { get; private set; }

        public bool IsEntity { get; private set; }

        public bool IsGroupKey { get; set; }

        public string Name { get; private set; }

        public Type ParameterType { get; private set; }

        public string ProviderId { get; set; }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.ORM
{
    internal class QueryContext {
        private Provider.DbProvider _dbProvider;
      
        /// <summary>
        /// 参数集合
        /// </summary>
        [ThreadStatic]
        private static QueryParameterColletion _parameters;
        /// <summary>
        /// 查询提供者
        /// </summary>
        private readonly QueryProvider _queryProvider;
        public QueryContext(Provider.DbProvider dbProvider)
        {
            // TODO: Complete member initialization
            this._dbProvider = dbProvider;
            this._queryProvider = new QueryProvider(dbProvider);
        }
        public QueryParameterColletion Parameters
        {
            get
            {
                return (_parameters ?? (_parameters = new QueryParameterColletion()));
            }
        }
        
        /// <summary>
        /// 添加查询参数
        /// </summary>
        /// <param name="type"></param>
        public void SafeAddQueryParameter(Type type)
        {
            this.Parameters.SafeAdd(type.FullName, this._dbProvider.Id, type);
        }
        /// <summary>
        /// 是否保持连接
        /// </summary>
        internal bool IsLLink { get; set; } = false;
        public QueryProvider QueryProvider
        {
            get
            {
                return this._queryProvider;
            }
        }


    }
}

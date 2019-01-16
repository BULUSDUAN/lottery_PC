using KaSon.FrameWork.ORM.IBuilder;
using KaSon.FrameWork.ORM.Mdel;
using KaSon.FrameWork.ORM.Provider;
using KaSon.FrameWork.Services.Enum;
using KaSon.FrameWork.Services.ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace KaSon.FrameWork.ORM
{
    internal class QueryProviderContext
    {
        /// <summary>
        /// 别名编号
        /// </summary>
        private int _aliasIndex;
        /// <summary>
        /// 参数 Index
        /// </summary>
        private int _paramIndex;
        private readonly StringBuilder _sqlBuilder;
        private readonly StringBuilder _sqlWhereBuilder;
        private readonly Provider.DbProvider _dbProvider;
        private readonly DbParameterCollection _parameters;

        private IList<OrderKeyMap> _OrderKeyList;

        private Func<QueryProviderContext, string> _textFunc;
        public QueryProviderContext()
        {
           
           // _dbProvider = dbProvider;
            this._parameters = new DbParameterCollection();
            this._sqlBuilder = new StringBuilder();
          
            
        }
        public QueryProviderContext(DbProvider dbProvider)
        {
            this._aliasIndex = 0;
            _dbProvider = dbProvider;
            this._sqlBuilder = new StringBuilder();
            this._OrderKeyList = new List<OrderKeyMap>();
            this._parameters = new DbParameterCollection();
            this.QueryColletion = QueryParameterColletion.DeepCopy(dbProvider.QueryContext.Parameters);
            foreach (QueryParameter parameter in (IEnumerable<QueryParameter>)this.QueryColletion)
            {
                if (parameter.GetAliasFunc == null)
                {
                    parameter.GetAliasFunc = new Func<string>(this.GetAlias);
                }
            }
        }
        public QueryProviderContext(DbProvider dbProvider, QueryProviderContext parent)
            : this(dbProvider)
        {
            this._aliasIndex = parent.AliasIndex;
            this._paramIndex = parent.ParamIndex;
            this._parameters = parent.Parameters;
            this.QueryColletion = parent.QueryColletion;
        }
        //立即执行
     

        public string GetQueryText()
        {
            return this._textFunc(this);
        }
        public void SetTextFunc(Func<QueryProviderContext, string> textFunc)
        {
            this._textFunc = textFunc;
        }
        /// <summary>
        /// 获取别名
        /// </summary>
        public int AliasIndex
        {
            get
            {
                return this._aliasIndex;
            }
        }
      
   
        public DbParameterCollection Parameters
        {
            get
            {
                return this._parameters;
            }
        }
        public int? Take { get; set; }
        public int ParamIndex
        {
            get
            {
                return this._paramIndex++;
            }
        }
        public Page Page { get; set; }

        /// <summary>
        /// FromText
        /// </summary>
        public string FromText { get;  set; }
        /// <summary>
        /// SelectText
        /// </summary>
        public string SelectText { get; set; }
        public string JoinSelectText { get; set; }

        public StringBuilder WhereText
        {
            get
            {
                return this._sqlWhereBuilder;
            }
        }
        public string OrderByText { get; set; }

        public bool IsExecute { get; set; }

        public bool HasSelect { get; set; }

        public bool IsAverage { get; set; }

        public bool IsCount { get; set; }

        public bool IsGroup { get; set; }

        public bool IsDistinct { get; set; }

        public bool IsSingle { get; set; }

        /// <summary>
        ///  是否逃过前几条
        /// </summary>
        public bool IsSkip { get; set; }
        /// <summary>
        ///  逃过前几条
        /// </summary>
        public int? Skip { get; set; }


        public bool IsMax { get; set; }

        public bool IsMin { get; set; }

        public bool IsSum { get; set; }
        /// <summary>
        /// 分组类型
        /// </summary>
        public Type GroupType { get; set; }

        /// <summary>
        /// 分组类型
        /// </summary>
        public string GroupModelAlas { get; set; }
        /// <summary>
        /// 分组key类型
        /// </summary>
        public Type GroupKeyType { get; set; }


        public Type SelectNodeType { get; set; }


       

        /// <summary>
        /// 分组类型
        /// </summary>
        public string  OrderKey { get; set; }
        public bool IsOrder { get; set; }
        public bool IsOrderAsc { get; set; }

        public IList<OrderKeyMap> OrderKeyList { get { return this._OrderKeyList; } set { this.OrderKeyList = value; } }
        /// <summary>
        /// 分组类型
        /// </summary>
        public string OrderModelAlas { get; set; }
        /// <summary>
        /// 分组key类型
        /// </summary>
        public Type OrderType { get; set; }
      
        /// <summary>
        /// 分组key Name
        /// </summary>
        public string GroupKeyName { get; set; }

        public QueryParameterColletion QueryColletion { get; private set; }

        private string GetAlias()
        {
            this._aliasIndex++;
            return ("T" + this._aliasIndex);


        }
        /// <summary>
        /// SQL 语句建造
        /// </summary>
        public StringBuilder SqlBuilder
        {
            get
            {
                return this._sqlBuilder;
            }
        }
        /// <summary>
        /// 数据库字段
        /// </summary>
        public List<FieldMdel> SqlSelectFields { get; set; } = new List<FieldMdel>();

        /// <summary>
        /// Select new 嵌套new 
        /// </summary>
        public List<SelectRightNodeMdel> SelRightNodeMdels { get; set; } = new List<SelectRightNodeMdel>();

        public string QuerySQL { get; set; }
        /// <summary>
        /// 返回类型
        /// </summary>
        public Type ReturnType { get; set; }
        public ConstructorInfo Constructor { get; set; }



        
        public IVisitorBuilder VisitorBuilder { get; set; }



        public DbProvider DbProvider
        {
            get
            {
                return this._dbProvider;
            }
        }

        public string JoinSelect { get; set; }

        public JoinMode[] JoinModes { get; set; }
    }
}

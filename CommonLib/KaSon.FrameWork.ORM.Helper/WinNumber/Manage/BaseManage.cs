using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using EntityModel;
using System.Linq.Expressions;
/// <summary>
/// 没测试
/// </summary>
namespace KaSon.FrameWork.ORM.Helper.WinNumber.Manage
{
   public class BaseManage<T>: DBbase where T :class,new() 
    {
       
        public void Add(T entity)
        {
            DB.GetDal<T>().Add(entity);
        }
        /// <summary>
        /// 查询基本走势（遗漏）最新一条数据
        /// </summary>
        public T Query(Expression<Func<T,bool>> orderBy)
        {          
            return DB.CreateQuery<T>().OrderByDescending(orderBy).FirstOrDefault();
        }

        public List<T> QueryTList(int index,Expression<Func<T,bool>> Orderby)
        {
            var query = DB.CreateQuery<T>().OrderBy(Orderby).Take(index);
            return query.ToList();
        }

        /// <summary>
        /// 查询T本期是否生成
        /// </summary>
        public int QueryTIssuseNumber(string issuseNumber,Expression<Func<T,bool>> WhereLamdba)
        {           
            return DB.CreateQuery<T>().Count(WhereLamdba);
        }
    }
}

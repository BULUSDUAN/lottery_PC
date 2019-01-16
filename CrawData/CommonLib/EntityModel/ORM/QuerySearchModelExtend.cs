
using KaSon.FrameWork.Services.ORM;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace EntityModel.ORM
{
    public static class QuerySearchModelExtend
    {
        /// <summary>
        /// 构造参数
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static QueryArgs toQuery(this QuerySearchModel model)
        {

            // model==null ? new QuerySearchModel();

            QueryArgs result = new QueryArgs();

            if (model.Filters != null)
            {

                var objj = JsonConvert.DeserializeObject(model.Filters, typeof(List<WhereField>));
                result.WhereFields = objj == null ? new List<WhereField>() : (List<WhereField>)objj;
            }
            if (model.Sorts != null)
            {
                var obj = JsonConvert.DeserializeObject(model.Sorts, typeof(List<SortField>));
                result.SortFields = obj == null ? new List<SortField>() : (List<SortField>)obj;
            }


            //当前页数
            int page = 1;

            if (model.PageIndex > 0)
            {
                page = model.PageIndex / model.PageSize + 1;

            }

            result.PageSize = model.PageSize;
            result.PageIndex = page;
            return result;
        }

     

        /// <summary>
        /// DataTable 结果集合
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        //public static QuerySearchResult toResult(this QueryResult model)
        //{

        //    var result = new QuerySearchResult();

        //    result.aaData = model.Data;
        //    result.iTotalDisplayRecords = model.RowCount;
        //    result.iTotalRecords = model.RowCount;

        //    return result;

        //}

   
    }
}

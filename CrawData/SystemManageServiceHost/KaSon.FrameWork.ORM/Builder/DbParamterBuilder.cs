using KaSon.FrameWork.ORM.Dal;
using KaSon.FrameWork.ORM.IBuilder;
using KaSon.FrameWork.ORM.Visitor;
using KaSon.FrameWork.ORM.Visitor.SQLServer;
using KaSon.FrameWork.Services.Enum;
using KaSon.FrameWork.Services.ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;

namespace KaSon.FrameWork.ORM.Builder
{


    /// <summary>
    ///  Dal 参数获取
    /// </summary>
    internal class DbParamterBuilder : IDbParamterBuilder
    {
        /// <summary>
        /// 删除参数
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        public virtual DbParameterCollection GetDeleteParamters(object entity, EntityInfo info)
        {
            DbParameterCollection parameters = new DbParameterCollection();
            foreach (KeyValuePair<string, FieldMap> pair in info.Fields)
            {
                if (pair.Value.Field.IsPrimaryKey)
                {
                    parameters.Insert(pair.Key, pair.Value.Property.Get(entity) ?? DBNull.Value, System.Data.ParameterDirection.Input);
                }
            }
            return parameters;
        }

        /// <summary>
        /// 录入参数
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        public virtual DbParameterCollection GetInsertParamters(object entity, EntityInfo info)
        {
            DbParameterCollection parameters = new DbParameterCollection();
            foreach (KeyValuePair<string, FieldMap> pair in info.Fields)
            {
                if (!pair.Value.Field.IsIdenty && ! pair.Value.Field.IsDefault)
                {
                    if (pair.Value.Version != null)
                    {
                        parameters.Insert(pair.Key, pair.Value.Version.InitialValue, System.Data.ParameterDirection.Input);
                    }
                    else
                    {
                        parameters.Insert(pair.Key, pair.Value.Property.Get(entity) ?? DBNull.Value, System.Data.ParameterDirection.Input);
                    }
                }
            }
            return parameters;
        }

        /// <summary>
        /// 获取更新参数
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        public virtual DbParameterCollection GetUpdateParamters(object entity, EntityInfo info)
        {
            DbParameterCollection parameters = new DbParameterCollection();
            foreach (KeyValuePair<string, FieldMap> pair in info.Fields)
            {
               // if ((!pair.Value.Field.IsIdenty))
               // {
               //     parameters.Insert(pair.Key, pair.Value.Property.Get(entity) ?? DBNull.Value, System.Data.ParameterDirection.Input);
              //  }
                //主键 或者不是自增的 添加参数
                if (pair.Value.Field.IsPrimaryKey || !pair.Value.Field.IsIdenty)
                {
                     parameters.Insert(pair.Key, pair.Value.Property.Get(entity) ?? DBNull.Value, System.Data.ParameterDirection.Input);
                }
            }
            return parameters;
        }
    }
}

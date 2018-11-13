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



    internal class SqlServerParamterBuilder : DbParamterBuilder
    {
    //    public override DbParameterCollection GetInsertParamters(object entity, EntityInfo info)
    //    {
    //        DbParameterCollection parameters = new DbParameterCollection();
    //        foreach (KeyValuePair<string, FieldMap> pair in info.Fields)
    //        {
    //            if (pair.Value.Version != null)
    //            {
    //                parameters.Insert(pair.Key, pair.Value.Version.InitialValue, System.Data.ParameterDirection.Input);
    //            }
    //            else if (!pair.Value.)
    //            {

    //                parameters.Insert(pair.Key, pair.Value.Property.Get(entity) ?? DBNull.Value, System.Data.ParameterDirection.Input);
    //            }
    //        }
    //        return parameters;
    //    }
    }
}

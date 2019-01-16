using KaSon.FrameWork.Services.ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace KaSon.FrameWork.ORM
{
    internal interface IDbParamterBuilder
    {
        DbParameterCollection GetDeleteParamters(object entity, EntityInfo info);
        DbParameterCollection GetInsertParamters(object entity, EntityInfo info);
        DbParameterCollection GetUpdateParamters(object entity, EntityInfo info);
    }
}

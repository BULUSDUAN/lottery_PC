using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Data.SqlClient;
using System.Data.OleDb;
using Common.Utilities;

namespace Common.Database.DbAccess
{
    public static class DbFactory
    {
        public static IDbTranAccess BeginTransaction(RDatabaseType dbtype, string constr)
        {
            PreconditionAssert.IsNotEmptyString(constr, ErrorMessages.DBConnectionStringIsNullOrEmpty);
            return new DbTran(GetLHDBAccess(dbtype, constr));
        }
        public static IDbDoAccess CreateDBAccessInstance(RDatabaseType dbtype, string constr)
        {
            PreconditionAssert.IsNotEmptyString(constr, ErrorMessages.DBConnectionStringIsNullOrEmpty);
            return GetLHDBAccess(dbtype, constr);
        }
        private static DbAccessBase GetLHDBAccess(RDatabaseType dbtype, string constr)
        {
            switch (dbtype)
            {
                case RDatabaseType.MSSQL:
                    return new DbMsSql(constr);
                default:
                    throw new DbAccessException(ErrorMessages.NotSupportedDatabaseType + " - " + dbtype.ToString());
            }
        }
    }
}

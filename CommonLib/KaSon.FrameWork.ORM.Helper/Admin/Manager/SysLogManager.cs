using EntityModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace KaSon.FrameWork.ORM.Helper
{
    public class SysLogManager:DBbase
    {
        public void AddSysOperationLog(C_Sys_OperationLog entity)
        {
            DB.GetDal<C_Sys_OperationLog>().Add(entity);
        }
    }
}

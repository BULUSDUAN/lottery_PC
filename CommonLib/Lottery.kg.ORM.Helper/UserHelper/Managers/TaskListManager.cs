using EntityModel;
using EntityModel.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lottery.Kg.ORM.Helper.UserHelper
{
   public class TaskListManager:DBbase
    {
        /// <summary>
        /// 添加一条未赠送记录
        /// </summary>
        public void AddTaskList(E_TaskList entity)
        {
            DB.GetDal<E_TaskList>().Add(entity);
        }

        /// <summary>
        /// 查询某个赠送记录
        /// </summary>
        public E_TaskList QueryTaskListByCategory(string userId, TaskCategory taskCategory)
        {
           
            return DB.CreateQuery<E_TaskList>().FirstOrDefault(p => p.UserId == userId && p.TaskCategory == (int)taskCategory);
        }

        public void AddUserTaskRecord(E_UserTaskRecord entity)
        {
            DB.GetDal<E_UserTaskRecord>().Add(entity);
        }


    }
}

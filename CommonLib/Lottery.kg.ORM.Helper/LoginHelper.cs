using EntityModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using KaSon.FrameWork.Services.ORM;
using EntityModel.ORM;
using KaSon.FrameWork.Helper;
using KaSon.FrameWork.Services.Enum;

namespace Lottery.Kg.ORM.Helper
{
  public  class LoginHelper:DBbase
    {

        /// <summary>
        /// 日志记录器 
        /// </summary>
        IKgLog log = null;
        public LoginHelper() {

            log = new Log4Log();

        }
        /// <summary>
        /// 查询用户名
        /// </summary>
        public List<User> QueryUserName() {

          var list=  DB.CreateQuery<User>().ToList();
            return list;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<E_Login_Local> QueryloginUserName()
        {

            var list = DB.CreateQuery<E_Login_Local>().ToList();
            return list;
        }

        private Task<int> TTest1()
        {


            //事务增删改

            DB.Begin();
            try
            {
                for (int i = 0; i < 500; i++)
                {

                    var relate1 = new EntityModel.Relate() { Name = "联系人" + i };
                    DB.GetDal<EntityModel.Relate>().Add(relate1, true);

                    var user = new EntityModel.User() { Name = "用户" + i, Relate_ID = relate1.ID };
                    DB.GetDal<EntityModel.User>().Add(user, true);

                }
                DB.Commit();
            }
            catch (Exception ex)
            {
                DB.Rollback();
                throw ex;
            }
            return Task.FromResult(1);
        }


        private Task<List<User>> TTest2()
        {

            Console.WriteLine("当前线程：" + Thread.CurrentThread.GetHashCode());

            var uQuery = DB.CreateQuery<EntityModel.User>();
            #region 查询全部
            var allList = uQuery.ToList().Take<EntityModel.User>(2).ToList();
            #endregion
            return Task.FromResult(allList);
        }

        private List<User> TTest3()
        {

            Console.WriteLine("当前线程：" + Thread.CurrentThread.GetHashCode());

            var uQuery = DB.CreateQuery<EntityModel.User>();
            #region 查询全部
            var allList = uQuery.ToList().Take<EntityModel.User>(2).ToList();
            #endregion
            return allList;
        }

        /// <summary>
        /// 单表查询
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        private Task<int> Test1()
        {
            var uQuery = DB.CreateQuery<EntityModel.User>();
            #region 查询全部
            var allList = uQuery.ToList();
            #endregion


            #region 条件查询
            var p = uQuery.Where(b => b.ID > 0 && b.Name.Contains("用户")).ToList<EntityModel.User>();
            #endregion



            #region  条件分页查询
            QueryArgs queryAga = new QueryArgs();
            var wlist = new List<WhereField>();
            WhereField wf = new WhereField()
            {
                WhereType = WhereType.Like,
                Field = "test_name",
                Value = "用户"
            };
            wlist.Add(wf);

            queryAga.WhereFields = wlist;

            var result = DB.CreateComQuery().Query<User>(uQuery, queryAga);//.toResult();
            #endregion




            #region json 分页查询
            QuerySearchModel qsModel = new QuerySearchModel();
            string str = JsonHelper.Serialize(wlist);

            qsModel.Filters = str;
            var args = qsModel.toQuery();
            var result1 = DB.CreateComQuery().Query<User>(uQuery, args);
            #endregion


            return Task.FromResult(1);
        }
        /// <summary>
        /// 多表查询
        /// </summary>
        /// <returns></returns>
        private Task<int> Test2()
        {
            //两张表
            var uQueryUser = DB.CreateQuery<EntityModel.User>();
            var uQueryRelate = DB.CreateQuery<EntityModel.Relate>();

            #region 查询全部

            var allQuery = from b in uQueryUser
                           join c in uQueryRelate
                           on b.Relate_ID equals c.ID
                           select new EntityModel.User
                           {
                               ID = b.ID,
                               Name = b.Name,
                               Relate_ID = b.ID,
                               RelateName = c.Name

                           };
            var p1 = allQuery.ToList();

            #endregion


            #region 条件查询
            var p2 = allQuery.Where(b => b.ID > 1).ToList<EntityModel.User>();
            #endregion



            #region  条件分页查询
            QueryArgs queryAga = new QueryArgs();
            var wlist = new List<WhereField>();
            WhereField wf = new WhereField()
            {
                WhereType = WhereType.Like,
                Field = "test_name",
                Value = "用户"
            };
            wlist.Add(wf);

            queryAga.WhereFields = wlist;

            var result = DB.CreateComQuery().Query<User>(allQuery, queryAga);//.toResult();
            #endregion




            #region json 分页查询
            QuerySearchModel qsModel = new QuerySearchModel();
            string str = JsonHelper.Serialize(wlist);

            qsModel.Filters = str;
            var args = qsModel.toQuery();
            var result1 = DB.CreateComQuery().Query<User>(allQuery, args);
            #endregion


            return Task.FromResult(1);
        }

        /// <summary>
        ///  sql 语句查询
        /// </summary>
        /// <returns></returns>
        private Task<int> Test21()
        {
           // SQLXML sqlmxl = new SQLServerXml();
            //sqlmxl.LoginSQL = "";
             //两张表
             var list = DB.CreateSQLQuery("select * from test_relate where id>@p").SetInt("@p", 1).First<Relate>();
            return Task.FromResult(1);
        }


        /// <summary>
        /// 增删改与事务
        /// </summary>
        /// <returns></returns>
        private Task<int> Test3()
        {

            var relate = new EntityModel.Relate() { Name = "联系人1" };
            DB.GetDal<EntityModel.Relate>().Add(relate);

            DB.GetDal<EntityModel.Relate>().Delete(b => b.ID == 0);


            DB.GetDal<EntityModel.Relate>().Update(b => new Relate() { Name = "联系人1" }, b => b.ID == 1);




            //事务增删改

            DB.Begin();
            try
            {
                var relate1 = new EntityModel.Relate() { Name = "联系人3" };
                DB.GetDal<EntityModel.Relate>().Add(relate1, true);

                var user = new EntityModel.User() { Name = "用户1", Relate_ID = relate1.ID };
                DB.GetDal<EntityModel.User>().Add(user, true);
                DB.Commit();
            }
            catch (Exception ex)
            {
                DB.Rollback();
                throw ex;
            }




            return Task.FromResult(1);
        }



    }
}

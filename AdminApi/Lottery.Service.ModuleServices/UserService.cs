using KaSon.FrameWork.ORM.Provider;

using Kason.Sg.Core.Caching;
using Kason.Sg.Core.CPlatform;
using Kason.Sg.Core.CPlatform.EventBus.Events;
using Kason.Sg.Core.CPlatform.Filters.Implementation;
using Kason.Sg.Core.CPlatform.Ioc;
using Kason.Sg.Core.CPlatform.Routing.Implementation;
using Kason.Sg.Core.CPlatform.Runtime.Client.Address.Resolvers.Implementation.Selectors.Implementation;
using Kason.Sg.Core.CPlatform.Runtime.Server.Implementation.ServiceDiscovery.Attributes;
using Kason.Sg.Core.CPlatform.Support;
using Kason.Sg.Core.CPlatform.Support.Attributes;
using Kason.Sg.Core.CPlatform.Transport.Implementation;
using Kason.Sg.Core.ProxyGenerator;
using Kason.Sg.Core.ProxyGenerator.Implementation;
using Kason.Sg.Core.System.Intercept;

using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using EntityModel;
using Lottery.Service.Model;
using KaSon.FrameWork.Services.ORM;
using KaSon.FrameWork.Services.Enum;
using EntityModel.ORM;
using KaSon.FrameWork.Helper;

namespace Lottery.Service.ModuleServices
{
    [ModuleName("User")]
    public class UserService : ProxyServiceBase, IModuleServices.IUserService
    {
        #region Implementation of IUserService
        //private readonly UserRepository _repository;
        //public UserService(UserRepository repository)
        //{
        //    this._repository = repository;
        //}
        private DbProvider DB;
        public UserService()
        {

            DB = new DbProvider();
            //// db.Init("Default");
            DB.Init("MySql.Default");

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
            //两张表
          var  list=  DB.CreateSQLQuery("select * from test_relate where id>@p").SetInt("@p", 1).List<Relate>();
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


        public Task<int> GetUserId(string userName)
        {
            //var xid = RpcContext.GetContext().GetAttachment("xid");

            //throw new Exception("错误！");

            //var T1 = Test1();
            //var T21 = Test21();
            //var T2 = Test2();
            //var T3 = Test3();

            return Task.FromResult(1);
        }
        //获取用户
        public Task<UserModel> GetUser(UserModel user)
        {
            return Task.FromResult(new UserModel
            {
                Name = "fanly",
                Age = 18
            });
        }


        public async Task PublishThroughEventBusAsync(IntegrationEvent evt)
        {
            Publish(evt);
            await Task.CompletedTask;
        }


        #endregion Implementation of IUserService
    }
}

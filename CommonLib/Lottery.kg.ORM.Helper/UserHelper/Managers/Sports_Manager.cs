using EntityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lottery.Kg.ORM.Helper.UserHelper
{
    public class Sports_Manager:DBbase
    {
        /// <summary>
        /// 查询用户战绩
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="gameCode"></param>
        /// <param name="gameType"></param>
        /// <returns></returns>
        public C_User_Beedings QueryUserBeedings(string userId, string gameCode, string gameType)
        {
            //Session.Clear();
            return DB.CreateQuery<C_User_Beedings>().FirstOrDefault(p => p.UserId == userId && p.GameCode == gameCode && (string.Empty == gameType || p.GameType == gameType));
        }

        /// <summary>
        /// 初始化用户战绩
        /// </summary>
        /// <param name="UserBeedings"></param>
        public void AddUserBeedings(C_User_Beedings UserBeedings) {

            DB.GetDal<C_User_Beedings>().Add(UserBeedings);
        }

        /// <summary>
        /// 用户中奖概率
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="gameCode"></param>
        /// <param name="gameType"></param>
        /// <returns></returns>
        public C_User_BonusPercent QueryUserBonusPercent(string userId, string gameCode, string gameType)
        {
            //Session.Clear();
            return DB.CreateQuery<C_User_BonusPercent>().FirstOrDefault(p => p.UserId == userId && p.GameCode == gameCode && (gameType == string.Empty || p.GameType == gameType));
        }

        /// <summary>
        /// 初始化用户中奖概率
        /// </summary>
        /// <param name="UserBeedings"></param>
        public void AddUserBonusPercent(C_User_BonusPercent UserBonusPercent)
        {

            DB.GetDal<C_User_BonusPercent>().Add(UserBonusPercent);
        }

        /// <summary>
        /// 用户关注汇总
        /// </summary>
        /// <param name="c_User_Attention_Summary"></param>
        public void AddUserAttentionSummary(C_User_Attention_Summary UserAttentionSummary)
        {
            DB.GetDal<C_User_Attention_Summary>().Add(UserAttentionSummary);

        }
    }
}

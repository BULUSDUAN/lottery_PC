using System;
using System.Collections;
using GameBiz.Core;
using GameBiz.Auth.Domain.Entities;

namespace GameBiz.Domain.Entities
{
    /// <summary>
    /// �û����
    /// </summary>
    public class UserBalance
    {
        public UserBalance()
        {
            Version = 1;
        }
        public virtual string UserId { get; set; }
        public virtual int Version { get; set; }
        /// <summary>
        /// �û�
        /// </summary>
        public virtual SystemUser User { get; set; }
        /// <summary>
        /// ��ֵ�˻����
        /// </summary>
        public virtual decimal FillMoneyBalance { get; set; }
        /// <summary>
        /// �����˻����н��󷵵����˻���������
        /// </summary>
        public virtual decimal BonusBalance { get; set; }
        /// <summary>
        /// Ӷ���˻���Ϊ�����̼���Ӷ��ʱ��ת�����˻�
        /// </summary>
        public virtual decimal CommissionBalance { get; set; }
        /// <summary>
        /// �������
        /// </summary>
        public virtual decimal ExpertsBalance { get; set; }
        /// <summary>
        /// �����˻������֡�׷�š��쳣�ֹ�����
        /// </summary>
        public virtual decimal FreezeBalance { get; set; }
        /// <summary>
        /// ������
        /// </summary>
        public virtual decimal RedBagBalance { get; set; }
        /// <summary>
        /// CPS���
        /// </summary>
        public virtual decimal CPSBalance { get; set; }


        /// <summary>
        /// �ɳ�ֵ
        /// </summary>
        public virtual int UserGrowth { get; set; }
        /// <summary>
        /// ��ǰ����ֵ
        /// </summary>
        public virtual int CurrentDouDou { get; set; }


        public virtual bool IsSetPwd { get; set; }
        /// <summary>
        /// ��Ҫ�����ʽ�����ĵط�
        /// </summary>
        public virtual string NeedPwdPlace { get; set; }
        /// <summary>
        /// �ʽ�����
        /// </summary>
        public virtual string Password { get; set; }
        /// <summary>
        /// ������
        /// </summary>
        public virtual string AgentId { get; set; }

        public virtual decimal GetTotalEnableMoney()
        {
            //return this.FillMoneyBalance + this.BonusBalance + this.CommissionBalance + this.ExpertsBalance + this.RedBagBalance;
            //return this.FillMoneyBalance + this.BonusBalance + this.ExpertsBalance + this.RedBagBalance;
            return this.FillMoneyBalance + this.BonusBalance + this.CommissionBalance;
        }
    }
    /// <summary>
    /// �û��ʽ𶳽���ϸ
    /// </summary>
    public class UserBalanceFreeze
    {
        public virtual long Id { get; set; }
        public virtual string UserId { get; set; }
        public virtual string OrderId { get; set; }
        public virtual decimal FreezeMoney { get; set; }
        public virtual FrozenCategory Category { get; set; }
        public virtual string Description { get; set; }
        public virtual DateTime CreateTime { get; set; }
    }

    /// <summary>
    /// ע����Ϣ
    /// </summary>
    public class UserRegister
    {
        public virtual string UserId { get; set; }
        /// <summary>
        /// �û�
        /// </summary>
        public virtual SystemUser User { get; set; }
        /// <summary>
        /// VIP�ȼ�
        /// </summary>
        public virtual int VipLevel { get; set; }
        /// <summary>
        /// ��ʾ����
        /// </summary>
        public virtual string DisplayName { get; set; }
        /// <summary>
        /// �û�������Դ
        /// </summary>
        public virtual string ComeFrom { get; set; }
        /// <summary>
        /// ע��IP
        /// </summary>
        public virtual string RegisterIp { get; set; }
        /// <summary>
        /// ע������ҳ��
        /// </summary>
        public virtual string ReferrerUrl { get; set; }
        /// ע������
        /// </summary>
        public virtual string Referrer { get; set; }
        /// <summary>
        /// ע������
        /// </summary>
        public virtual string RegType { get; set; }
        public virtual bool IsEnable { get; set; }
        public virtual bool IsAgent { get; set; }
        /// <summary>
        /// �Ƿ�������
        /// </summary>
        public virtual bool IsExperter { get; set; }
        /// <summary>
        /// �Ƿ��ֵ
        /// </summary>
        public virtual bool IsFillMoney { get; set; }
        /// <summary>
        /// �����û�����
        /// </summary>
        public virtual int HideDisplayNameCount { get; set; }
        /// <summary>
        /// ����ʱ��
        /// </summary>
        public virtual DateTime CreateTime { get; set; }
        /// <summary>
        /// ������
        /// </summary>
        public virtual string AgentId { get; set; }
        public virtual bool? IsIgnoreReport { get; set; }
        /// <summary>
        /// ����·��
        /// </summary>
        public virtual string ParentPath { get; set; }
        /// <summary>
        /// �û����:0:��վ��ͨ�û���1���ڲ�Ա���û���
        /// </summary>
        public virtual int UserType { get; set; }
    }

    /// <summary>
    /// �û������ʷ
    /// </summary>
    public class UserBalanceHistory
    {
        public virtual long Id { get; set; }
        public virtual string SaveDateTime { get; set; }
        public virtual string UserId { get; set; }
        /// <summary>
        /// ��ֵ�˻����
        /// </summary>
        public virtual decimal FillMoneyBalance { get; set; }
        /// <summary>
        /// �����˻����н��󷵵����˻���������
        /// </summary>
        public virtual decimal BonusBalance { get; set; }
        /// <summary>
        /// Ӷ���˻���Ϊ�����̼���Ӷ��ʱ��ת�����˻�
        /// </summary>
        public virtual decimal CommissionBalance { get; set; }
        /// <summary>
        /// �������
        /// </summary>
        public virtual decimal ExpertsBalance { get; set; }
        /// <summary>
        /// �����˻������֡�׷�š��쳣�ֹ�����
        /// </summary>
        public virtual decimal FreezeBalance { get; set; }
        /// <summary>
        /// ������
        /// </summary>
        public virtual decimal RedBagBalance { get; set; }
        /// <summary>
        /// �ɳ�ֵ
        /// </summary>
        public virtual int UserGrowth { get; set; }
        /// <summary>
        /// ��ǰ����ֵ
        /// </summary>
        public virtual int CurrentDouDou { get; set; }
        /// <summary>
        /// ����ʱ��
        /// </summary>
        public virtual DateTime CreateTime { get; set; }
    }

    /// <summary>
    /// �û�����
    /// </summary>
    public class UserBalanceReport
    {
        public virtual long Id { get; set; }
        public virtual string SaveDateTime { get; set; }
        /// <summary>
        /// ��ֵ�˻����
        /// </summary>
        public virtual decimal TotalFillMoneyBalance { get; set; }
        /// <summary>
        /// �����˻����н��󷵵����˻���������
        /// </summary>
        public virtual decimal TotalBonusBalance { get; set; }
        /// <summary>
        /// Ӷ���˻���Ϊ�����̼���Ӷ��ʱ��ת�����˻�
        /// </summary>
        public virtual decimal TotalCommissionBalance { get; set; }
        /// <summary>
        /// �������
        /// </summary>
        public virtual decimal TotalExpertsBalance { get; set; }
        /// <summary>
        /// �����˻������֡�׷�š��쳣�ֹ�����
        /// </summary>
        public virtual decimal TotalFreezeBalance { get; set; }
        /// <summary>
        /// ������
        /// </summary>
        public virtual decimal TotalRedBagBalance { get; set; }
        /// <summary>
        /// �ɳ�ֵ
        /// </summary>
        public virtual int TotalUserGrowth { get; set; }
        /// <summary>
        /// ��ǰ����ֵ
        /// </summary>
        public virtual int TotalDouDou { get; set; }
        /// <summary>
        /// ����ʱ��
        /// </summary>
        public virtual DateTime CreateTime { get; set; }
    }
}

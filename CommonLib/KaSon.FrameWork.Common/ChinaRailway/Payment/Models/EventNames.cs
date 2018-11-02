using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaSon.FrameWork.Common.ChinaRailway
{
    public static class EventNames
    {
        public const string Charge = "Charge";
        public const string Charge_Query = "Charge.Query";
        public const string Charge_Refund = "Charge.Refund";
        public const string Charge_Refund_Query = "Charge.Refund.Query";
        public const string Charge_Succeeded = "Charge.Succeeded";
        public const string Charge_Refund_Succeeded = "Charge.Refund.Succeeded";

        public const string Charge_Easy_BankCard_Add = "Charge.Easy.BankCard.Add";
        public const string Charge_Easy_BankCard_Add_Confirm = "Charge.Easy.BankCard.Add.Confirm";
        public const string Charge_Easy_BankCard_Query = "Charge.Easy.BankCard.Query";
        public const string Charge_Easy_BankCard_Remove = "Charge.Easy.BankCard.Remove";

        public const string Pay = "Pay";
        public const string Pay_Query = "Pay.Query";
        public const string Pay_Refund = "Pay.Refund";
        public const string Pay_Refund_Query = "Pay.Refund.Query";
        public const string Pay_Succeeded = "Pay.Succeeded";
        public const string Pay_Refund_Succeeded = "Pay.Refund.Succeeded";

        public const string Transfer = "Transfer";
        public const string Transfer_Query = "Transfer.Query";
        public const string Transfer_Refund = "Transfer.Refund";
        public const string Transfer_Refund_Query = "Transfer.Refund.Query";
        public const string Transfer_Succeeded = "Transfer.Succeeded";
        public const string Transfer_Refund_Succeeded = "Transfer.Refund.Succeeded";
    }
}

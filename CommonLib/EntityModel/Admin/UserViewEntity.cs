
using EntityModel.CoreModel;

namespace EntityModel
{
    public class UserViewEntity
    {
        public string UserKey { get; set; }
        public string UserName { get; set; }
        public UserQueryInfo UserResult { get; set; }
        public UserLoginHistoryCollection HistoryLogin { get; set; }
        public C_BankCard Bank { get; set; }
        public string UserToken { get; set; }
        public UserBalanceFreezeCollection FreezeList { get; set; }
        public UserRealNameInfo RealNameInfo { get; set; }
        public UserMobileInfo MobileInfo { get; set; }
        public E_Authentication_Email EmailInfo { get; set; }
        public int UserCreditType { get; set; }
        public string ApliyCount { get; set; }
        public string QQNumber { get; set; }
    }
}

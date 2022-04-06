namespace Asp.netCore_MVC.ViewModels
{
    public class UserClaimsViewModel
    {
        #region Constructor

        public UserClaimsViewModel()
        {
            Claims = new List<UserClaim>();
        }
        #endregion
        #region Properties

        public string? UserId { get; set; }

        public IList<UserClaim> Claims { get; set; }
        #endregion
    }
}

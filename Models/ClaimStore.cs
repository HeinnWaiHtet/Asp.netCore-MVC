using System.Security.Claims;

namespace Asp.netCore_MVC.Models
{
    public static class ClaimStore
    {
        #region Properties

        public static List<Claim> AllClaims = new List<Claim>()
        {
            new Claim("Create Role", "Create Role"),
            new Claim("Edit Role", "Edit Role"),
            new Claim("Delete Role", "Delete Role")
        };
        #endregion
    }
}

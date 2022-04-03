using Microsoft.AspNetCore.Identity;

namespace Asp.netCore_MVC.Models
{
    public class ApplicationUser : IdentityUser
    {
        #region Properties

        /// <summary>
        /// City Name
        /// </summary>
        public string? City { get; set; }
        #endregion
    }
}

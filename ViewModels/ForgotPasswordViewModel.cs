using System.ComponentModel.DataAnnotations;

namespace Asp.netCore_MVC.ViewModels
{
    public class ForgotPasswordViewModel
    {
        #region Properties

        [Required]
        [EmailAddress]
        public string Email { get; set; }
        #endregion
    }
}

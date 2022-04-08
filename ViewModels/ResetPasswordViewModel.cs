using System.ComponentModel.DataAnnotations;

namespace Asp.netCore_MVC.ViewModels
{
    public class ResetPasswordViewModel
    {
        #region Properties

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "Password and Confirm Password must match")]
        public string ConfirmPassword { get; set; }

        public string Token { get; set; }
        #endregion
    }
}

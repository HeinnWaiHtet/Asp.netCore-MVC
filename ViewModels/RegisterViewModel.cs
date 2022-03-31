using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Asp.netCore_MVC.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress(ErrorMessage ="Enter Valid Email Address")]
        [Display(Name = "Email Address")]
        [Remote(action: "IsEmailInUse", controller: "Account")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Enter Your Password")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Enter Your Confirm Password")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password",
            ErrorMessage = "Password and confirmation password do not match.")]
        public string? ConfirmPassword { get; set; }
    }
}

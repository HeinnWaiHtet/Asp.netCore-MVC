using System.ComponentModel.DataAnnotations;

namespace Asp.netCore_MVC.ViewModels
{
    public class CreateRoleViewModel
    {
        #region Properties

        [Required(ErrorMessage = "Please Enter Role Name")]
        public string? RoleName { get; set; }
        #endregion
    }
}

using System.ComponentModel.DataAnnotations;

namespace Asp.netCore_MVC.ViewModels
{
    public class EditRoleViewModel
    {
        #region Constructor

        public EditRoleViewModel()
        {
            this.Users = new List<string>();
        }
        #endregion

        #region Properties

        public string? Id { get; set; }

        [Required(ErrorMessage = "Role Name is required")]
        public string? RoleName { get; set; }

        public IList<string> Users { get; set; }
        #endregion
    }
}

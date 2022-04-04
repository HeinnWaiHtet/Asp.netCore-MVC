using System.ComponentModel.DataAnnotations;

namespace Asp.netCore_MVC.ViewModels
{
    public class EditUserViewModel
    {

        #region Constructor

        public EditUserViewModel()
        {
            this.Claims = new List<string>();
            this.Roles = new List<string>();
        }
        #endregion

        #region Properties

        public string? Id { get; set; }

        [Required]
        public string? UserName { get; set; }

        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        public string City { get; set; }

        public List<string> Claims { get; set; }

        public List<string> Roles { get; set; }

        #endregion
    }
}

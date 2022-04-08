using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Asp.netCore_MVC.Models
{
    public class Employee
    {
        /// <summary>
        /// Employee Id
        /// </summary>
        public int Id { get; set; }

        [NotMapped]
        public string? EncryptedId { get; set; }

        /// <summary>
        /// Employee Name
        /// </summary>
        [Required]
        [MaxLength(50, ErrorMessage = "Name cannot exceed 50 characters")]
        public string Name { get; set; }

        /// <summary>
        /// Employee Email
        /// </summary>
        [Required]
        [RegularExpression(@"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z",
            ErrorMessage = "Check your email format")]
        [Display(Name = "Employee Email")]
        public string Email { get; set; }

        /// <summary>
        /// Employee Department
        /// </summary>
        [Required, Display(Name = "Select Your Department")]
        public Dept? Department { get; set; }

        /// <summary>
        /// Employee Photo Save Path
        /// </summary>
        public string PhotoPath { get; set; }
    }
}

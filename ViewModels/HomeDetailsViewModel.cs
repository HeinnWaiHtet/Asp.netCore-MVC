using Asp.netCore_MVC.Models;

namespace Asp.netCore_MVC.ViewModels
{
    public class HomeDetailsViewModel
    {
        /// <summary>
        /// Employee Data
        /// </summary>
        public Employee? Employee { get; set; }

        /// <summary>
        /// Page Title
        /// </summary>
        public string? Title { get; set; }
    }
}

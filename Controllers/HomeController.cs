using Asp.netCore_MVC.Models;
using Asp.netCore_MVC.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Asp.netCore_MVC.Controllers
{
    public class HomeController : Controller
    {
        private IEmployeeRepository _employeeRepository;

        /// <summary>
        /// HomeController Constructor
        /// </summary>
        /// <param name="employeeRepository"></param>
        public HomeController(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = new MockEmployeeRepository();
        }

        /// <summary>
        /// Default Index Method
        /// </summary>
        /// <returns></returns>
        public string Index()
        {
            return _employeeRepository.GetEmployeeById(2).Name;
        }

        /// <summary>
        /// Get Employee Details by Id
        /// </summary>
        /// <returns></returns>
        public IActionResult Details()
        {
            HomeDetailsViewModel viewModel = new HomeDetailsViewModel()
            {
                Employee = _employeeRepository.GetEmployeeById(1),
                Title = "Home Employee Details"
            };

            return this.View(viewModel);
        }
    }
}

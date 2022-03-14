using Asp.netCore_MVC.Models;
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
    }
}

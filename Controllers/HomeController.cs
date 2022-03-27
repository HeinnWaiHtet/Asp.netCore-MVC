using Asp.netCore_MVC.Models;
using Asp.netCore_MVC.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Asp.netCore_MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly IEmployeeRepository _employeeRepository;

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
        public IActionResult Index()
        {
            var employee = this._employeeRepository.GetAllEmployees();
            return this.View(employee);
        }

        /// <summary>
        /// Get Employee Details by Id
        /// </summary>
        /// <returns></returns>
        public IActionResult Details(int? id)
        {
            HomeDetailsViewModel viewModel = new HomeDetailsViewModel()
            {
                Employee = _employeeRepository.GetEmployeeById(id ?? 1),
                Title = "Home Employee Details"
            };

            return this.View(viewModel);
        }

        /// <summary>
        /// Create Employee View
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Create()
        {
            return this.View();
        }

        /// <summary>
        /// Create New Employee And Redirect to details after create
        /// </summary>
        /// <param name="employee"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Create(Employee employee)
        {
            var newEmployee = _employeeRepository.Add(employee);
            return this.RedirectToAction("details", new { id = newEmployee.Id });
        }
    }
}

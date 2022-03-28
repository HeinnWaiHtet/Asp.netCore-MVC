using Asp.netCore_MVC.Models;
using Asp.netCore_MVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;

namespace Asp.netCore_MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IWebHostEnvironment hostingEnvironment;

        /// <summary>
        /// HomeController Constructor
        /// </summary>
        /// <param name="employeeRepository"></param>
        public HomeController(
            IEmployeeRepository employeeRepository,
            IWebHostEnvironment hostingEnvironment)
        {
            _employeeRepository = employeeRepository;
            this.hostingEnvironment = hostingEnvironment;
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
        public IActionResult Create(EmployeeCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = null;
                if(model.Photo != null)
                {
                    // create uploadFolder by using webhosting directory
                    string uploadFolder = Path.Combine(hostingEnvironment.WebRootPath, "images");
                    // Get Unique Identifier using Guid
                    uniqueFileName = $"{Guid.NewGuid().ToString()}_{model.Photo.FileName}";
                    // upload local photo path
                    string filePath = Path.Combine(uploadFolder, uniqueFileName);
                    // copy image to local folder using IformFile CopyTo
                    model.Photo.CopyTo(new FileStream(filePath, FileMode.Create));
                }

                var employee = new Employee()
                {
                    Name = model.Name,
                    Email = model.Email,
                    Department = model.Department,
                    PhotoPath = uniqueFileName ?? String.Empty
                };

                var newEmployee = _employeeRepository.Add(employee);
                return this.RedirectToAction("details", new { id = newEmployee.Id });
            }

            return this.View();
        }

        /// <summary>
        /// Remove Emplyoee By Request Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult Delete(int id)
        {
            _employeeRepository.Delete(id);
            return this.RedirectToAction("index");
        }
    }
}

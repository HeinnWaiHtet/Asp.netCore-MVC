using Asp.netCore_MVC.Models;
using Asp.netCore_MVC.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Asp.netCore_MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IWebHostEnvironment hostingEnvironment;

        #region HomeConstructor

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

        #endregion

        #region EmployeeHomePage

        /// <summary>
        /// Default Index Method
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            var employee = this._employeeRepository.GetAllEmployees();
            return this.View(employee);
        }

        #endregion

        #region EmployeeDetailView

        /// <summary>
        /// Get Employee Details by Id
        /// </summary>
        /// <returns></returns>
        public IActionResult Details(int? id)
        {
            throw new Exception("Something Wrong");
            var employee = _employeeRepository.GetEmployeeById(id.Value);
            if(employee == null)
            {
                Response.StatusCode = 404;
                return this.View("EmployeeNotFound", id.Value);
            }

            HomeDetailsViewModel viewModel = new HomeDetailsViewModel()
            {
                Employee = employee,
                Title = "Home Employee Details"
            };

            return this.View(viewModel);
        }

        #endregion

        #region CreateEmployee

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
                string uniqueFileName = UploadPhoto(model);

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

        #endregion

        #region EditEmployee

        /// <summary>
        /// Edit Employee View
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var employee = _employeeRepository.GetEmployeeById(id);
            var model = new EmployeeEditViewModel()
            {
                Id = id,
                Email = employee.Email,
                Department = employee.Department,
                Name = employee.Name,
                ExistingPhotoPath = employee.PhotoPath
            };

            return this.View(model);
        }

        /// <summary>
        /// Edit Employee And Redirect to details after Update
        /// </summary>
        /// <param name="employee"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Edit(EmployeeEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var employee = _employeeRepository.GetEmployeeById(model.Id);
                employee.Name = model.Name;
                employee.Email = model.Email;
                employee.Department = model.Department;
                if(model.Photo != null)
                {
                    DeleteLocalPhoto(model.ExistingPhotoPath);
                    employee.PhotoPath = UploadPhoto(model);
                }

                var newEmployee = _employeeRepository.Update(employee);
                return this.RedirectToAction("index");
            }

            return this.View();
        }

        #endregion

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

        #region PrivateMethod

        /// <summary>
        /// Upload Photo By Given Photo Data
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private string UploadPhoto(EmployeeCreateViewModel model)
        {
            string uniqueFileName = null;
            if (model.Photo != null)
            {
                // create uploadFolder by using webhosting directory
                string uploadFolder = Path.Combine(hostingEnvironment.WebRootPath, "images");
                // Get Unique Identifier using Guid
                uniqueFileName = $"{Guid.NewGuid().ToString()}_{model.Photo.FileName}";
                // upload local photo path
                string filePath = Path.Combine(uploadFolder, uniqueFileName);
                // copy image to local folder using IformFile CopyTo
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.Photo.CopyTo(fileStream);
                }
            }

            return uniqueFileName;
        }

        /// <summary>
        /// Delete Existing Local Photo
        /// </summary>
        /// <param name="photo"></param>
        private void DeleteLocalPhoto(string photo)
        {
            if (!string.IsNullOrEmpty(photo))
            {
                string filePath = Path.Combine(
                    hostingEnvironment.WebRootPath,
                    "images",
                    photo);
                System.IO.File.Delete(filePath);
            }
        }

        #endregion
    }
}

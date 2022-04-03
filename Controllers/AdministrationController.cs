using Asp.netCore_MVC.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Asp.netCore_MVC.Controllers
{
    public class AdministrationController : Controller
    {
        #region Properties

        private readonly RoleManager<IdentityRole> roleManager;
        #endregion

        #region Constructor

        public AdministrationController(RoleManager<IdentityRole> roleManager)
        {
            this.roleManager = roleManager;
        }
        #endregion

        #region CreateRole

        /// <summary>
        /// Create Role View
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult CreateRole()
        {
            return this.View();
        }

        /// <summary>
        /// Create Role By Request ViewModel
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                /** Create IdentiyRole */
                var identityRole = new IdentityRole()
                {
                    Name = model.RoleName
                };

                /** Create Role By IdentiyRole */
                var result = await roleManager.CreateAsync(identityRole);

                /** Check Role Create Success or not */
                if (result.Succeeded)
                {
                    return this.RedirectToAction("index", "home");
                }

                /** Add Error When Role Creation Fail */
                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

            }

            return View();
        }
        #endregion

    }
}

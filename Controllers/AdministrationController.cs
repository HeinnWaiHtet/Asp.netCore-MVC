using Asp.netCore_MVC.Models;
using Asp.netCore_MVC.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Asp.netCore_MVC.Controllers
{
    public class AdministrationController : Controller
    {
        #region Properties

        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<ApplicationUser> userManager;
        #endregion

        #region Constructor

        public AdministrationController(RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
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
                    return this.RedirectToAction("ListRoles");
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

        #region GetRoleLists

        /// <summary>
        /// Get Role List From Role
        /// </summary>
        /// <returns></returns>
        public IActionResult ListRoles()
        {
            /** Get All Role Using roleManager */
            var roleLists = roleManager.Roles;
            return this.View(roleLists);
        }
        #endregion

        #region EditRole

        /// <summary>
        /// Edit Role View
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> EditRole(string id)
        {
            /** Get User role by request Roleid */
            var role = await roleManager.FindByIdAsync(id);

            /** return to error view when request role not found */
            if(role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id {id} cannot be found";
                return View("NotFound");
            }

            /** create edit role view model */
            var model = new EditRoleViewModel()
            {
                Id = role.Id,
                RoleName = role.Name
            };

            /** find users that have request role */
            var userList = await userManager.Users.ToListAsync();
            foreach (var user in userList)
            {
                if(await userManager.IsInRoleAsync(user, role.Name))
                {
                    model.Users.Add(user.UserName);
                }
            }

            return this.View(model);
        }

        /// <summary>
        /// Edit Role Action
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> EditRole(EditRoleViewModel model)
        {
            /** Get User role by request Roleid */
            var role = await roleManager.FindByIdAsync(model.Id);

            /** return to error view when request role not found */
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id {model.Id} cannot be found";
                return View("NotFound");
            }
            else
            {
                role.Name = model.RoleName;

                /** Update Role Using UpdateAsync */
                var result = await roleManager.UpdateAsync(role);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListRoles");
                }

                /** add error when update found error */
                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return this.View(model);
        }

        #endregion

    }
}

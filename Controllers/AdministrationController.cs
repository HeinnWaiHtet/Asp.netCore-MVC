using Asp.netCore_MVC.Models;
using Asp.netCore_MVC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Asp.netCore_MVC.Controllers
{
    [Authorize(Roles = "Admin")]
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

        #region DeleteRole

        /// <summary>
        ///  Delete user role by request Id
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> DeleteRole(string id)
        {
            /** Get role by request Id */
            var role = await roleManager.FindByIdAsync(id);

            /** return to error view when request role not found */
            if (role == null)
            {
                ViewBag.ErrorMessage = $"User with Id {id} cannot be found";
                return View("NotFound");
            }
            else
            {
                /** Delete role using DeleteAsync */
                var result = await roleManager.DeleteAsync(role);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListRoles");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View("ListRoles");
            }
        }

        #endregion

        #region UserProcess


        #region GetUserLists

        /// <summary>
        /// Get User List From Users
        /// </summary>
        /// <returns></returns>
        public IActionResult ListUsers()
        {
            /** Get All User Using UserManager */
            var userLists = userManager.Users;
            return this.View(userLists);
        }
        #endregion

        #region EditUser

        /// <summary>
        /// Edit User View
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> EditUser(string id)
        {
            /** Get User by request id */
            var user = await userManager.FindByIdAsync(id);

            /** return to error view when request user not found */
            if (user == null)
            {
                ViewBag.ErrorMessage = $"Role with Id {id} cannot be found";
                return View("NotFound");
            }

            /** Get User claims */
            var userClaims = await userManager.GetClaimsAsync(user);
            /** Get User Roles */
            var userRoles = await userManager.GetRolesAsync(user);

            /** create edit User view model */
            var model = new EditUserViewModel()
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                City = user.City,
                Claims = userClaims.Select(c => c.Value).ToList(),
                Roles = userRoles.ToList()
            };

            return this.View(model);
        }

        /// <summary>
        ///  Edit User Action
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            /** Get User by request id */
            var user = await userManager.FindByIdAsync(model.Id);

            /** return to error view when request user not found */
            if (user == null)
            {
                ViewBag.ErrorMessage = $"Role with Id {model.Id} cannot be found";
                return View("NotFound");
            }
            else
            {
                user.UserName = model.UserName;
                user.City = model.City;
                user.Email = model.Email;

                /** Update User Using UpdateAsync */
                var result = await userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListUsers");
                }

                /** add error when update found error */
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return this.View(model);
        }

        #endregion

        #region DeleteUser

        /// <summary>
        ///  Delete user by request Id
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            /** Get User by request id */
            var user = await userManager.FindByIdAsync(id);

            /** return to error view when request user not found */
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id {id} cannot be found";
                return View("NotFound");
            }
            else
            {
                /** Delete User using DeleteAsync */
                var result = await userManager.DeleteAsync(user);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListUsers");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View("ListUsers");
            }
        }

        #endregion

        #endregion

        #region EditUserInRole

        /// <summary>
        /// UserInRole View
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> EditUsersInRole(string id)
        {
            ViewBag.roleId = id;
            /** Get User role by request Roleid */
            var role = await roleManager.FindByIdAsync(id);

            /** return to error view when request role not found */
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id {id} cannot be found";
                return View("NotFound");
            }

            /** create UserInRole view model */
            var model = new List<UserRoleViewModel>();

            /** find users that have request role */
            var userList = await userManager.Users.ToListAsync();
            foreach (var user in userList)
            {
                var userRoleViewModel = new UserRoleViewModel()
                {
                    UserId = user.Id,
                    UserName = user.UserName
                };

                /** Check user is in role or not */
                if (await userManager.IsInRoleAsync(user, role.Name))
                {
                    userRoleViewModel.IsSelected = true;
                }
                else
                {
                    userRoleViewModel.IsSelected = false;
                }

                model.Add(userRoleViewModel);
            }

            return this.View(model);
        }

        /// <summary>
        /// Edit User Role Action
        /// </summary>
        /// <param name="model"></param>
        /// /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> EditUsersInRole(List<UserRoleViewModel> model, string id)
        {
            /** Get User role by request Roleid */
            var role = await roleManager.FindByIdAsync(id);

            /** return to error view when request role not found */
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id {id} cannot be found";
                return View("NotFound");
            }
            
            /** Loop Model Data */
            for(int index = 0; index < model.Count; index++)
            {
                /** Get User Data By User Id */
                var user = await userManager.FindByIdAsync(model[index].UserId);

                IdentityResult result = null;

                /** Check Current User is In role or not */
                if (model[index].IsSelected && !(await userManager.IsInRoleAsync(user, role.Name)))
                {
                    result = await userManager.AddToRoleAsync(user, role.Name); 
                }
                else if (!model[index].IsSelected && await userManager.IsInRoleAsync(user, role.Name))
                {
                    result = await userManager.RemoveFromRoleAsync(user, role.Name);
                }
                else
                {
                    continue;
                }

                /** Check UserRole update finish or not */
                if (result.Succeeded)
                {
                    if(index < (model.Count - 1))
                    {
                        continue;
                    }
                    else
                    {
                        return RedirectToAction("EditRole", new { Id = id });
                    }
                }
            }

            return this.RedirectToAction("EditRole", new {Id = id});
        }

        #endregion

        #region AccessDeniedPage

        /// <summary>
        /// Go To AccessDenied View
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return this.View();
        }
        #endregion
    }
}

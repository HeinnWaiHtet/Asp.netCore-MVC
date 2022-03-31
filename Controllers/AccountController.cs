﻿using Asp.netCore_MVC.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Asp.netCore_MVC.Controllers
{
    public class AccountController : Controller
    {
        #region Properties

        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;

        #endregion

        #region Constructor

        public AccountController(UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }
        #endregion

        #region RegisterUser

        /// <summary>
        /// Register View
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        /// <summary>
        /// Register User using UserManager.CreateAsync and singin using signInManager
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                /** Create IdentiyUser object using RegisterViewModel */
                var user = new IdentityUser { UserName = model.Email, Email = model.Email };
                /** Create User Using UserManager CreateAsync */
                var result = await userManager.CreateAsync(user, model.Password);

                if(result.Succeeded)
                {
                    /** Login Create User using SignInManager.SignInAsync */
                    await signInManager.SignInAsync(user, isPersistent: false);
                    return this.RedirectToAction("Index", "Home");
                }

                foreach(var errors in result.Errors)
                {
                    /** Add Model Error */
                    ModelState.AddModelError("", errors.Description);
                }
            }

            return View(model);
        }

        #endregion

    }
}

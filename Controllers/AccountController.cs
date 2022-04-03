using Asp.netCore_MVC.Models;
using Asp.netCore_MVC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Asp.netCore_MVC.Controllers
{
    public class AccountController : Controller
    {
        #region Properties

        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;

        #endregion

        #region Constructor

        public AccountController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
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
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        /// <summary>
        /// Check Register Email is in used or not
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [AcceptVerbs("Get", "Post")]
        [AllowAnonymous]
        public async Task<ActionResult> IsEmailInUse(string email)
        {
            var user = await userManager.FindByEmailAsync(email);

            return user == null
                ? Json(true)
                : Json($"Email {email} is already in use");
        }

        /// <summary>
        /// Register User using UserManager.CreateAsync and singin using signInManager
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                /** Create IdentiyUser object using RegisterViewModel */
                var user = new ApplicationUser { 
                    UserName = model.Email, 
                    Email = model.Email,
                    City = model.City,
                };
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

        #region LoginUser

        /// <summary>
        /// Login View
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        /// <summary>
        /// Login User Using Email and Password and Return To Redirect link after authenticate
        /// </summary>
        /// <param name="model"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl)
        {
            if (ModelState.IsValid)
            {

                /** Login Create User using SignInManager.SignInAsync */
                var result = await signInManager.PasswordSignInAsync(
                    model.Email,
                    model.Password,
                    model.RememberMe,
                    false);

                if (result.Succeeded)
                {
                    /** Check Whether returnUrl has or not and redirect to request URL */
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }

                    return this.RedirectToAction("Index", "Home");
                }

                /** Add Model Error */
                ModelState.AddModelError("", "Login Failed");
            }

            return View(model);
        }

        #endregion

        #region Logout

        /// <summary>
        /// Logout When Click Logout Button
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "home");
        }

        #endregion

    }
}

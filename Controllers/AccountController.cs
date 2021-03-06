using Asp.netCore_MVC.Models;
using Asp.netCore_MVC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Asp.netCore_MVC.Controllers
{
    public class AccountController : Controller
    {
        #region Properties

        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly ILogger<AccountController> logger;

        #endregion

        #region Constructor

        public AccountController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<AccountController> logger)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.logger = logger;
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
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    City = model.City,
                };
                /** Create User Using UserManager CreateAsync */
                var result = await userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    /** create Email confirmation token */
                    var token = await userManager.GenerateEmailConfirmationTokenAsync(user);

                    /** Email Confirmation Link */
                    var confirmationLink = Url.Action("confirmEmail", "Account",
                        new { userId = user.Id, token = token }, Request.Scheme);

                    logger.Log(LogLevel.Warning, confirmationLink);

                    /** Check User is login or not and login user role is admin or not */
                    if (signInManager.IsSignedIn(User) && User.IsInRole("Admin"))
                    {
                        return RedirectToAction("ListUsers", "Administration");
                    }

                    /** Show Email Confirmation Required Error */
                    ViewBag.ErrorTitle = "Registration Successful";
                    ViewBag.ErrorMessage = "Before login, Please confirm your Email, By clicking" +
                        "Confimation link";

                    return this.View("Error");
                }

                foreach (var errors in result.Errors)
                {
                    /** Add Model Error */
                    ModelState.AddModelError("", errors.Description);
                }
            }

            return View(model);
        }

        #region EmailConfrimation

        /// <summary>
        /// Add Email Confirmation by userId and token
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string token)
        {
            /** Check request userid and token */
            if(userId == null || token == null)
            {
                return this.RedirectToAction("index", "home");
            }

            var user = await userManager.FindByIdAsync(userId);
            if(user == null)
            {
                ViewBag.ErrorMessage = $"The user Id {userId} is invalid";
                return this.View("NotFound");
            }

            /** Confirm email */
            var result = await userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                return this.View();
            }

            /** Add Error When Email Confimation Failed. */
            ViewBag.ErrorTitle = "Email cannot be confirmed";
            return this.View("Error");
        }
        #endregion

        #endregion

        #region LoginUser

        /// <summary>
        /// Login View
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string? returnUrl)
        {
            var model = new LoginViewModel
            {
                ReturnUrl = returnUrl,
                ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };

            return View(model);
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
            model.ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                /** Get User By Email */
                var user = await userManager.FindByEmailAsync(model.Email);

                if (user != null && !user.EmailConfirmed &&
                    (await userManager.CheckPasswordAsync(user, model.Password)))
                {
                    ModelState.AddModelError(string.Empty, "Email not confirmed yet");
                    return this.View(model);
                }

                /** Login Create User using SignInManager.SignInAsync */
                var result = await signInManager.PasswordSignInAsync(
                    model.Email,
                    model.Password,
                    model.RememberMe,
                    true);

                if (result.Succeeded)
                {
                    /** Check Whether returnUrl has or not and redirect to request URL */
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }

                    return this.RedirectToAction("Index", "Home");
                }

                /** Check Lockedout or not */
                if (result.IsLockedOut)
                {
                    return this.View("AccountLocked");
                }

                /** Add Model Error */
                ModelState.AddModelError("", "Login Failed");
            }

            return View(model);
        }

        #endregion

        #region ExternalLogin

        /// <summary>
        /// External Login Like Google, Facebook, Twitter
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        public IActionResult ExternalLogins(string provider, string returnUrl)
        {
            /** create redirect Url after login using external service */
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account",
                new { ReturnUrl = returnUrl });

            /** Login External Services */
            var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);

            /** Return TO Local Url After External Login */
            return new ChallengeResult(provider, properties);
        }

        /// <summary>
        /// Check Error and Login after redirect login using external login
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <param name="remoteError"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(
            string returnUrl = null,
            string remoteError = null)
        {
            /** check returnUrl has or not */
            returnUrl = returnUrl ?? Url.Content("~/");

            /** Create login ViewModel */
            var loginViewModel = new LoginViewModel
            {
                ReturnUrl = returnUrl,
                ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };

            /** check remoteError has or not */
            if (remoteError != null)
            {
                ModelState.AddModelError(string.Empty,
                    $"Error From External provider : {remoteError}");
                return this.View("Login", loginViewModel);
            }

            /** Get External login information */
            var info = await signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ModelState.AddModelError(string.Empty,
                    "Error loading external login");
                return this.View("login", loginViewModel);
            }

            /** Get LoginEmail Claim Type */
            var email = info.Principal.FindFirstValue(ClaimTypes.Email);

            var user = new ApplicationUser
            {
                Email = email,
            };

            /** Check Sign In Email Confirmed or not */
            if (email != null)
            {
                user = await userManager.FindByEmailAsync(email);

                if (user != null && !user.EmailConfirmed)
                {
                    ModelState.AddModelError(string.Empty, "Email not confirmed yet");
                    return this.View("login", loginViewModel);
                }
            }

            /** Login using external login provider */
            var signInResult = await signInManager.ExternalLoginSignInAsync(info.LoginProvider,
                info.ProviderKey,
                isPersistent: false,
                bypassTwoFactor: true);

            if (signInResult.Succeeded)
            {
                return this.LocalRedirect(returnUrl);
            }
            else
            {
                /** check email register or not */
                if (email != null)
                {
                    /** check User is already register or not */
                    if (user == null)
                    {
                        user = new ApplicationUser
                        {
                            UserName = info.Principal.FindFirstValue(ClaimTypes.Email),
                            Email = info.Principal.FindFirstValue(ClaimTypes.Email),

                        };

                        await userManager.CreateAsync(user);

                        /** create Email confirmation token */
                        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);

                        /** Email Confirmation Link */
                        var confirmationLink = Url.Action("confirmEmail", "Account",
                            new { userId = user.Id, token = token }, Request.Scheme);

                        logger.Log(LogLevel.Warning, confirmationLink);

                        /** Show Email Confirmation Required Error */
                        ViewBag.ErrorTitle = "Registration Successful";
                        ViewBag.ErrorMessage = "Before login, Please confirm your Email, By clicking" +
                            "Confimation link";

                        return this.View("Error");
                    }

                    await userManager.AddLoginAsync(user, info);
                    await signInManager.SignInAsync(user, isPersistent: false);

                    return this.LocalRedirect(returnUrl);
                }

                ViewBag.ErrorTitle = $"Email Clamim not recived from : {info.LoginProvider}";
                ViewBag.ErrorMessage = "Please Contact support team";

                return this.View("Error");

            }

            return this.View("login", loginViewModel);
        }

        #endregion

        #region ForgotPassword

        /// <summary>
        /// Forgot Password View
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        /// <summary>
        /// Forgot Password Email Confirmation Send
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                /** Get User By Email */
                var user = await userManager.FindByEmailAsync(model.Email);

                /** Check Email confirm or not */
                if (user != null && await userManager.IsEmailConfirmedAsync(user))
                {
                    var token = await userManager.GeneratePasswordResetTokenAsync(user);

                    var passwordResetLink = Url.Action("ResetPassword", "Account",
                            new { email = model.Email, token = token }, Request.Scheme);

                    logger.Log(LogLevel.Warning, passwordResetLink);

                    return View("ForgotPasswordConfirmation");
                }

                return View("ForgotPasswordConfirmation");
            }

            return View(model);
        }
        #endregion

        #region ResetPassword

        /// <summary>
        /// Reset Password View
        /// </summary>
        /// <param name="token"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string token, string email)
        {
            if (token == null || email == null)
            {
                ModelState.AddModelError("", "Invalid password reset token");
            }
            return View();
        }

        /// <summary>
        /// Reset Password Action
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                /** Get User By Email */
                var user = await userManager.FindByEmailAsync(model.Email);

                if (user != null)
                {
                    /** Set Reset Password and check */
                    var result = await userManager.ResetPasswordAsync(user, model.Token, model.Password);
                    if (result.Succeeded)
                    {
                        /** check lock user or not */
                        if (await userManager.IsLockedOutAsync(user))
                        {
                            await userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow);
                        }

                        return View("ResetPasswordConfirmation");
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View(model);
                }

                return View("ResetPasswordConfirmation");
            }

            return View(model);
        }

        #endregion

        #region ChangePassword

        /// <summary>
        /// Change password
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> ChangePassword()
        {
            var user = await userManager.GetUserAsync(User);

            /** Check Current Login user password has or not */
            var userHasPassword = await userManager.HasPasswordAsync(user);

            if (!userHasPassword)
            {
                return RedirectToAction("AddPassword");
            }

            return View();
        }

        /// <summary>
        /// Change Password Process
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                /** Get User */
                var user = await userManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToAction("Login");
                }

                /** Change Login user password */
                var result = await userManager.ChangePasswordAsync(user,
                    model.CurrentPassword, model.NewPassword);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View();
                }

                await signInManager.RefreshSignInAsync(user);
                return View("ChangePasswordConfirmation");
            }

            return View(model);
        }
        #endregion

        #region AddPassword

        /// <summary>
        /// Add Password For External Login
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> AddPassword()
        {
            /** Get Current Login user */
            var user = await userManager.GetUserAsync(User);

            /** Check UserPassword has or not */
            var userHasPassword = await userManager.HasPasswordAsync(user);

            if (userHasPassword)
            {
                return RedirectToAction("ChangePassword");
            }

            return View();
        }

        /// <summary>
        /// Add Addpassword process
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddPassword(AddPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                /** Get Current Login user */
                var user = await userManager.GetUserAsync(User);

                /** Add login user local password */
                var result = await userManager.AddPasswordAsync(user, model.NewPassword);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View();
                }

                await signInManager.RefreshSignInAsync(user);

                return View("AddPasswordConfirmation");
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

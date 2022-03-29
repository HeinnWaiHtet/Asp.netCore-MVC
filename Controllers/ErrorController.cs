using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Asp.netCore_MVC.Controllers
{
    public class ErrorController : Controller
    {
        #region RequestNotFoundErrorHandling

        /// <summary>
        /// Request Url not found Error Page With Error StatusCode URL
        /// </summary>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        [Route("/Error/{statusCode}")]
        public IActionResult HttpStatusCodeHandler(int statusCode)
        {
            var statusCodeResult = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();
            switch (statusCode)
            {
                case 404:
                    ViewBag.ErrorMessage = "Sorry, the resource you requested cannot be found";
                    ViewBag.Path = statusCodeResult?.OriginalPath;
                    ViewBag.QS = statusCodeResult?.OriginalQueryString; ;
                    break;
            }
            return View("NotFound");
        }

        #endregion

        #region GlobalExceptionHandling

        /// <summary>
        /// Global Excetion Handling Error Page Like Throw Exception Error
        /// </summary>
        /// <returns></returns>
        [Route("Error")]
        [AllowAnonymous]
        public IActionResult Error()
        {
            var exceptionDetails = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            ViewBag.ExceptionPath = exceptionDetails?.Path;
            ViewBag.ExceptionMessage = exceptionDetails?.Error.Message;
            ViewBag.Stacktrace = exceptionDetails?.Error.StackTrace;
            return this.View("Error");
        }

        #endregion
    }
}

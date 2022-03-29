using Microsoft.AspNetCore.Mvc;

namespace Asp.netCore_MVC.Controllers
{
    public class ErrorController : Controller
    {
        /// <summary>
        /// Request Url not found Error Page With Error StatusCode URL
        /// </summary>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        [Route("/Error/{statusCode}")]
        public IActionResult HttpStatusCodeHandler(int statusCode)
        {
            switch (statusCode)
            {
                case 404:
                    ViewBag.ErrorMessage = "Sorry, the resource you requested cannot be found";
                    break;
            }
            return View("NotFound");
        }
    }
}

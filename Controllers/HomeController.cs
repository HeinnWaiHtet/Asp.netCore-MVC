using Microsoft.AspNetCore.Mvc;

namespace Asp.netCore_MVC.Controllers
{
    public class HomeController : Controller
    {
        public string Index()
        {
            return "This is MVC Controller";
        }
    }
}

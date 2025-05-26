using Microsoft.AspNetCore.Mvc;

namespace WebProjekt.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Impressum()
        {
            return View();
        }

        // - UserController
        //        - Registration-Action
        //        - Login Action
        //       - Beide Views im _Layout.cshtml
    }
}

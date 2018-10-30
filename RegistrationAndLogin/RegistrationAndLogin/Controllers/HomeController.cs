using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RegistrationAndLogin.Controllers
{
    public class HomeController : Controller
    {
        List<RegisterController> registerControllers = new List<RegisterController>();
        // GET: Home
        public ActionResult Index(RegisterController registerController)
        {
            registerControllers.Add(registerController);
            return View(registerControllers);
        }
    }
}
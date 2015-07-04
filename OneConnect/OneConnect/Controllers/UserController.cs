using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OneConnect.Controllers
{
    public class UserController : Controller
    {
        HomeController home = new HomeController();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Register()
        {

            ViewBag.isRegistrationCompletedSuccessfully = false;
            return View();
        }
        public ActionResult Dashboard()
        {
            if(home.IsAuthenticated())
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index","Home");
            }
        }
    }
}
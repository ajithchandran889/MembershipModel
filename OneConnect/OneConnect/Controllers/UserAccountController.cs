using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OneConnect.Controllers
{
    public class UserAccountController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Register()
        {

            ViewBag.isRegistrationCompletedSuccessfully = false;
            return View();
        }
    }
}
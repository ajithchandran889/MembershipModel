using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OneKonnect.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {

            if (HttpContext.User.Identity.IsAuthenticated)
            {

            }
            else
            {

            }
            return View();
        }
    }
}

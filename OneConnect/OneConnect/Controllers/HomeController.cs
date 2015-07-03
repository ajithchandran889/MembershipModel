using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OneConnect.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if(Request.Cookies["isAuthenticated"]!=null)
            {
                var isAuthenticated = Request.Cookies["isAuthenticated"].Value;
            }
            
            return View();
        }
    }
}

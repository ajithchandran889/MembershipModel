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
            try
            {
                if (IsAuthenticated())
                {
                        return RedirectToAction("Dashboard", "User");
                    
                }
            }
            catch(Exception e)
            {

            }
            return View();
            
        }
        public bool IsAuthenticated()
        {
            try
            {
                if (Request.Cookies["isAuthenticated"] != null)
                {
                    var isAuthenticated = Convert.ToBoolean(Request.Cookies["isAuthenticated"].Value);
                    if (isAuthenticated == true)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception e)
            {

            }
            return true;
        }
    }
}

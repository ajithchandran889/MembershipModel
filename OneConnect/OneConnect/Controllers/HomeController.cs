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
                else
                {
                    return View();
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
                if (HttpContext.Request.Cookies["isAuthenticated"] != null)
                {
                    var isAuthenticated = Convert.ToBoolean(HttpContext.Request.Cookies["isAuthenticated"].Value);
                    if (isAuthenticated == true)
                    {
                        Session["IsAuthenticated"] = true;
                        return true;
                    }
                    else
                    {
                        Session["IsAuthenticated"] = false;
                        return false;
                    }
                }
            }
            catch (Exception e)
            {
                //Session["IsAuthenticated"] = false;
               // return false;
            }
            return false;
        }
    }
}

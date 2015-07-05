using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OneConnect.Controllers
{
    public class UserController : Controller
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
        public ActionResult Dashboard()
        {
            if(IsAuthenticated())
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index","Home");
            }
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
            return true;
        }
    }
}
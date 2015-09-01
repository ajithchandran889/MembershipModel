using OneConnect.ViewModels;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using OneConnect.Entities;

namespace OneConnect.Controllers
{
    public class CreditController : Controller
    {
        // GET: Credit
        public ActionResult Index()
        {
            if (IsAuthenticated())
            {
              
                var accountInfoUrl = Url.RouteUrl(
                        "GetAccountInfo",
                        new { httproute = "", controller = "Account", action = "GetAccountInfo" },
                        Request.Url.Scheme
                    );
                var getProductsUrl = Url.RouteUrl(
                        "GetProducts",
                        new { httproute = "", controller = "Products", action = "GetProducts" },
                        Request.Url.Scheme
                    );
                var token = HttpContext.Request.Cookies["token"].Value;
                dynamic myModel = new ExpandoObject();

                myModel.accountInfo = Account.GetAccountInfo(accountInfoUrl,token);
                myModel.products = Products.GetProducts(getProductsUrl, token);
                return View(myModel);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult SelectUsers()
        {
            if (IsAuthenticated())
            {
                var accountInfoUrl = Url.RouteUrl(
                        "GetAccountInfo",
                        new { httproute = "", controller = "Account", action = "GetAccountInfo" },
                        Request.Url.Scheme
                    );
                var token = HttpContext.Request.Cookies["token"].Value;
                dynamic myModel = new ExpandoObject();

                myModel.accountInfo = Account.GetAccountInfo(accountInfoUrl, token);
                return View(myModel);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult Summary()
        {
            if (IsAuthenticated())
            {
                var accountInfoUrl = Url.RouteUrl(
                        "GetAccountInfo",
                        new { httproute = "", controller = "Account", action = "GetAccountInfo" },
                        Request.Url.Scheme
                    );
                var token = HttpContext.Request.Cookies["token"].Value;
                dynamic myModel = new ExpandoObject();

                myModel.accountInfo = Account.GetAccountInfo(accountInfoUrl, token);
                return View(myModel);
            }
            else
            {
                return RedirectToAction("Index", "Home");
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
            return false;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OneConnect.Controllers
{
    public class CartController : Controller
    {
        // GET: Cart
        public ActionResult Index()
        {
            return View();
        }
        //[HttpPost]
        //public RedirectResult Pay(string amoount)
        //{
        //    decimal totalAmount = Convert.ToDecimal(Request["totalAmount"].ToString());
        //    PayPalRedirect redirect = PayPal.ExpressCheckout(new PayPalOrder { Amount = totalAmount });

        //    Session["token"] = redirect.Token;

        //    return new RedirectResult(redirect.Url);
        //    //return View();
        //}
        public ActionResult CancelUrl()
        {
            return View();
        }
        public ActionResult ReturnUrl()
        {
            return View();
        }
        public ActionResult NotifyUrl()
        {
            return View();
        }
    }
}
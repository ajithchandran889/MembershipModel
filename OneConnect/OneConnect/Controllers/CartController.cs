using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Mvc;
using OneConnect.ViewModels;
using Newtonsoft.Json;
using System.Dynamic;
using OneConnect.Entities;

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
        
        public ActionResult Return()
        {
            var formVals = new Dictionary<string, string>();
            formVals.Add("cmd", "_notify-synch");
            formVals.Add("at", ConfigurationManager.AppSettings["paypalKey"].ToString()); 
            formVals.Add("tx", Request["tx"]);
            //set true for sandbox else false
            string response = GetPayPalResponse(formVals, Convert.ToBoolean(ConfigurationManager.AppSettings["paypalSandbox"].ToString()));

            if (response.Contains("SUCCESS"))
            {
                ProductSubscribeDetails productSubscriptionDetails = new ProductSubscribeDetails();
                productSubscriptionDetails.transactionID = GetPDTValue(response, "txn_id"); 
                string sAmountPaid = GetPDTValue(response, "mc_gross");
                productSubscriptionDetails.payerEmail = GetPDTValue(response, "payer_email"); 
                productSubscriptionDetails.item = GetPDTValue(response, "item_name");
                productSubscriptionDetails.tempItemIds = GetPDTValue(response, "custom");
                Decimal amountPaid = 0;
                Decimal.TryParse(sAmountPaid, System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out amountPaid);
                productSubscriptionDetails.sAmountPaid =Convert.ToDouble(amountPaid);
                using (var client = new HttpClient())
                {
                    //var content = new MultipartFormDataContent();
                   // content.Add(new StringContent(JsonConvert.SerializeObject(content)), "Content");
                    client.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", HttpContext.Request.Cookies["token"].Value);
                    var productsSubscribeUrl = Url.RouteUrl(
                        "Subscribe",
                        new { httproute = "", controller = "Products", action = "Subscribe" },
                        Request.Url.Scheme
                    );
                    string status="";
                    using (var result = client.PostAsJsonAsync<ProductSubscribeDetails>(productsSubscribeUrl, productSubscriptionDetails).Result)
                    {


                        if (result.IsSuccessStatusCode)
                        {

                            status = result.Content.ReadAsAsync<string>().Result;

                        }

                    }
                }
                ViewBag.Message = "Succesfully completed the payment";
            }
            else
            {
                ViewBag.Message = "Oops..Something went wrong.Please try again.";
            }
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

        string GetPayPalResponse(Dictionary<string, string> formVals, bool useSandbox)
        {

            string paypalUrl = useSandbox ? "https://www.sandbox.paypal.com/cgi-bin/webscr"
                : "https://www.paypal.com/cgi-bin/webscr";

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(paypalUrl);

            // Set values for the request back
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";

            byte[] param = Request.BinaryRead(Request.ContentLength);
            string strRequest = Encoding.ASCII.GetString(param);

            StringBuilder sb = new StringBuilder();
            sb.Append(strRequest);

            foreach (string key in formVals.Keys)
            {
                sb.AppendFormat("&{0}={1}", key, formVals[key]);
            }
            strRequest += sb.ToString();
            req.ContentLength = strRequest.Length;

            //for proxy
            //WebProxy proxy = new WebProxy(new Uri("http://urlort#");
            //req.Proxy = proxy;
            //Send the request to PayPal and get the response
            string response = "";
            using (StreamWriter streamOut = new StreamWriter(req.GetRequestStream(), System.Text.Encoding.ASCII))
            {

                streamOut.Write(strRequest);
                streamOut.Close();
                using (StreamReader streamIn = new StreamReader(req.GetResponse().GetResponseStream()))
                {
                    response = streamIn.ReadToEnd();
                }
            }

            return response;
        }
        string GetPDTValue(string pdt, string key)
        {

            string[] keys = pdt.Split('\n');
            string thisVal = "";
            string thisKey = "";
            foreach (string s in keys)
            {
                string[] bits = s.Split('=');
                if (bits.Length > 1)
                {
                    thisVal = bits[1];
                    thisKey = bits[0];
                    if (thisKey.Equals(key, StringComparison.InvariantCultureIgnoreCase))
                        break;
                }
            }
            return thisVal;

        }

    }
}
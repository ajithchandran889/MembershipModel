using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using OneConnect.ViewModels;
using System.Dynamic;
using System.Threading.Tasks;

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
        public async Task<ActionResult> Dashboard()
        {
            if(IsAuthenticated())
            {
                using (var client = new HttpClient())
                {
                    dynamic myModel = new ExpandoObject();
                    client.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer",  HttpContext.Request.Cookies["token"].Value);
                    var userDetailUrl = Url.RouteUrl(
                        "GetUsers",
                        new { httproute = "", controller = "User", action = "GetUsers" },
                        Request.Url.Scheme
                    );
                    IEnumerable<UserDetails> userDetails = null;
                    using (var response = client.GetAsync(userDetailUrl).Result)
                    {

                        if (response.IsSuccessStatusCode)
                        {

                             userDetails = response.Content.ReadAsAsync<List<UserDetails>>().Result.ToList();

                        }

                    }
                    var accountInfoUrl = Url.RouteUrl(
                        "GetAccountInfo",
                        new { httproute = "", controller = "Account", action = "GetAccountInfo" },
                        Request.Url.Scheme
                    );
                     AccountInfo accounInfo = null;
                    using (var response = client.GetAsync(accountInfoUrl).Result)
                    {

                        if (response.IsSuccessStatusCode)
                        {

                            accounInfo = response.Content.ReadAsAsync<AccountInfo>().Result;

                        }

                    }
                    myModel.userList = userDetails;
                    myModel.accountInfo = accounInfo;
                    return View(myModel);
                }
                
                //return View();
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
            return false;
        }
    }
}
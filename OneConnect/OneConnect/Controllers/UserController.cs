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



                    var subscribedProductsUrl = Url.RouteUrl(
                        "GetSubscribedProducts",
                        new { httproute = "", controller = "Products", action = "GetSubscribedProducts" },
                        Request.Url.Scheme
                    );
                    IEnumerable<SubscribedProductDetails> subscribedProductDetails = null;
                    using (var response = client.GetAsync(subscribedProductsUrl).Result)
                    {

                        if (response.IsSuccessStatusCode)
                        {

                            subscribedProductDetails = response.Content.ReadAsAsync<List<SubscribedProductDetails>>().Result.ToList();

                        }

                    }

                    myModel.userList = userDetails;
                    myModel.accountInfo = accounInfo;
                    //myModel.groupList = groupDetails;
                    myModel.subscribedProducts = subscribedProductDetails;
                    return View(myModel);
                }
                
                //return View();
            }
            else
            {
                return RedirectToAction("Index","Home");
            }
        }
        [ChildActionOnly]
        public PartialViewResult AccountInfoPartial(AccountInfo accountInfo)
        {
            return PartialView("_PartialAccountInfo",accountInfo);
        }


        public PartialViewResult GroupListPartial(bool isActiveOnly)
        {
            if (IsAuthenticated())
            {
                using (var client = new HttpClient())
                {
                    dynamic myModel = new ExpandoObject();
                    client.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", HttpContext.Request.Cookies["token"].Value);
                    var groupListUrl = Url.RouteUrl(
                                "GetGroupsWithStatus",
                                new { httproute = "", controller = "Group", action = "GetGroupsWithStatus", isActiveOnly = isActiveOnly },
                                Request.Url.Scheme
                            );
                    IEnumerable<GroupDetails> groupDetails = null;
                    using (var response = client.GetAsync(groupListUrl).Result)
                    {

                        if (response.IsSuccessStatusCode)
                        {

                            groupDetails = response.Content.ReadAsAsync<List<GroupDetails>>().Result.ToList();

                        }


                    }
                    myModel.groupList = groupDetails == null ? new List<GroupDetails>() : groupDetails;

                    return PartialView("_PartialGroupMaster", myModel);
                }

            }
            return null;
        }



        public PartialViewResult AddGroupPartial()
        {
            if (IsAuthenticated())
            {
                using (var client = new HttpClient())
                {
                    dynamic myModel = new ExpandoObject();
                    client.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", HttpContext.Request.Cookies["token"].Value);

                    var groupInfoUrl = Url.RouteUrl(
                                "GetGroupInfo",
                                new { httproute = "", controller = "Group", action = "GetGroupInfo", id = 0 },
                                Request.Url.Scheme
                            );
                    GroupInfo groupInfo = new GroupInfo();
                    using (var response = client.GetAsync(groupInfoUrl).Result)
                    {

                        if (response.IsSuccessStatusCode)
                        {

                            groupInfo = response.Content.ReadAsAsync<GroupInfo>().Result;



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
                    IEnumerable<UserDetails> userDetails = null;
                    if (accounInfo != null)
                    {
                        if (accounInfo.isOwner)
                        {

                            var userDetailUrl = Url.RouteUrl(
                               "GetUsers",
                               new { httproute = "", controller = "User", action = "GetUsers" },
                               Request.Url.Scheme
                           );

                            using (var response = client.GetAsync(userDetailUrl).Result)
                            {

                                if (response.IsSuccessStatusCode)
                                {

                                    userDetails = response.Content.ReadAsAsync<List<UserDetails>>().Result.ToList();

                                }

                            }
                            if (userDetails == null)
                            {
                                userDetails = new List<UserDetails>();
                            }

                            UserDetails userDetail = new UserDetails();
                            userDetail.userId = accounInfo.userId;
                            userDetail.customUserId = accounInfo.customUserId;
                            userDetail.emailId = accounInfo.email;
                            userDetail.status = accounInfo.status;
                            userDetail.isOwner = accounInfo.isOwner;
                            var list = userDetails.ToList();
                            list.Insert(0, userDetail);
                            userDetails = list;
                        }
                        else
                        {
                            if (userDetails == null)
                            {
                                userDetails = new List<UserDetails>();
                            }

                            UserDetails userDetail = new UserDetails();
                            userDetail.userId = accounInfo.userId;
                            userDetail.customUserId = accounInfo.customUserId;
                            userDetail.emailId = accounInfo.email;
                            userDetail.status = accounInfo.status;
                            userDetail.isOwner = accounInfo.isOwner;
                            var list = userDetails.ToList();
                            list.Insert(0, userDetail);
                            userDetails = list;
                        }
                        if (accounInfo.isOwner && groupInfo.groupDetails.groupId == 0)
                        {
                            groupInfo.groupDetails.groupAdmin = accounInfo.userId;
                        }

                    }

                    groupInfo.users = userDetails == null ? new List<UserDetails>() : userDetails.ToList();
                    groupInfo.accInfo = accounInfo == null ? new AccountInfo() : accounInfo;

                    return PartialView("_PartialCreateGroup", groupInfo);
                }
            }

            return null;
        }

        public PartialViewResult EditGroupPartial(int groupId)
        {
            if (IsAuthenticated())
            {
                using (var client = new HttpClient())
                {
                    dynamic myModel = new ExpandoObject();
                    client.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", HttpContext.Request.Cookies["token"].Value);

                    var groupInfoUrl = Url.RouteUrl(
                                "GetGroupInfo",
                                new { httproute = "", controller = "Group", action = "GetGroupInfo", id = groupId },
                                Request.Url.Scheme
                            );
                    GroupInfo groupInfo = new GroupInfo();
                    using (var response = client.GetAsync(groupInfoUrl).Result)
                    {

                        if (response.IsSuccessStatusCode)
                        {

                            groupInfo = response.Content.ReadAsAsync<GroupInfo>().Result;



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
                    IEnumerable<UserDetails> userDetails = null;
                    if (accounInfo != null)
                    {
                        if (accounInfo.isOwner)
                        {

                            var userDetailUrl = Url.RouteUrl(
                               "GetUsers",
                               new { httproute = "", controller = "User", action = "GetUsers" },
                               Request.Url.Scheme
                           );

                            using (var response = client.GetAsync(userDetailUrl).Result)
                            {

                                if (response.IsSuccessStatusCode)
                                {

                                    userDetails = response.Content.ReadAsAsync<List<UserDetails>>().Result.ToList();

                                }

                            }
                            if (userDetails == null)
                            {
                                userDetails = new List<UserDetails>();
                            }

                            UserDetails userDetail = new UserDetails();
                            userDetail.userId = accounInfo.userId;
                            userDetail.customUserId = accounInfo.customUserId;
                            userDetail.emailId = accounInfo.email;
                            userDetail.status = accounInfo.status;
                            userDetail.isOwner = accounInfo.isOwner;
                            var list = userDetails.ToList();
                            list.Insert(0, userDetail);
                            userDetails = list;
                        }
                        else
                        {
                            if (userDetails == null)
                            {
                                userDetails = new List<UserDetails>();
                            }

                            UserDetails userDetail = new UserDetails();
                            userDetail.userId = accounInfo.userId;
                            userDetail.customUserId = accounInfo.customUserId;
                            userDetail.emailId = accounInfo.email;
                            userDetail.status = accounInfo.status;
                            userDetail.isOwner = accounInfo.isOwner;
                            var list = userDetails.ToList();
                            list.Insert(0, userDetail);
                            userDetails = list;
                        }
                        if (accounInfo.isOwner && groupInfo.groupDetails.groupId == 0)
                        {
                            groupInfo.groupDetails.groupAdmin = accounInfo.userId;
                        }

                    }

                    groupInfo.users = userDetails == null ? new List<UserDetails>() : userDetails.ToList();
                    groupInfo.accInfo = accounInfo == null ? new AccountInfo() : accounInfo;

                    return PartialView("_PartialCreateGroup", groupInfo);
                }
            }

            return null;
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
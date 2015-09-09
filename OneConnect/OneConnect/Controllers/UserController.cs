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

                    var groupListUrl = Url.RouteUrl(
                        "GetGroups",
                        new { httproute = "", controller = "Group", action = "GetGroups" },
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
                    myModel.groupList = groupDetails;
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

        //public PartialViewResult AddGroupPartial()
        //{
        //    if (IsAuthenticated())
        //    {
        //        using (var client = new HttpClient())
        //        {
        //            dynamic myModel = new ExpandoObject();
        //            client.DefaultRequestHeaders.Authorization =
        //                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", HttpContext.Request.Cookies["token"].Value);
        //            var groupProductsUrl = Url.RouteUrl(
        //                "GetGroupProductDetails",
        //                new { httproute = "", controller = "Group", action = "GetGroupProductDetails", id = 0 },
        //                Request.Url.Scheme
        //            );
        //            IEnumerable<GroupProductDetails> groupProducts = new List<GroupProductDetails>();
        //            using (var response = client.GetAsync(groupProductsUrl).Result)
        //            {

        //                if (response.IsSuccessStatusCode)
        //                {

        //                    groupProducts = response.Content.ReadAsAsync<List<GroupProductDetails>>().Result.ToList();

        //                }

        //            }

        //            var groupMembersUrl = Url.RouteUrl(
        //               "GetGroupMemberDetails",
        //               new { httproute = "", controller = "Group", action = "GetGroupMemberDetails", id = 0 },
        //               Request.Url.Scheme
        //           );
        //            IEnumerable<GroupMemberDetails> groupMembers = new List<GroupMemberDetails>();
        //            using (var response = client.GetAsync(groupMembersUrl).Result)
        //            {

        //                if (response.IsSuccessStatusCode)
        //                {

        //                    groupMembers = response.Content.ReadAsAsync<List<GroupMemberDetails>>().Result.ToList();

        //                }

        //            }

        //            GroupInfo groupInfo = new GroupInfo();

        //            GroupDetails groupDetails = new GroupDetails();

        //            groupInfo.groupDetails = groupDetails;

        //            groupInfo.groupProducts = groupProducts.ToList();

        //            groupInfo.groupMembers = groupMembers.ToList();

        //            return PartialView("_PartialCreateGroup", groupInfo);
        //        }

        //    }
        //    return null;
            
        //}


        //public PartialViewResult EditGroupPartial(int groupId)
        //{
        //    if (IsAuthenticated())
        //    {
        //        using (var client = new HttpClient())
        //        {
        //            dynamic myModel = new ExpandoObject();
        //            client.DefaultRequestHeaders.Authorization =
        //                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", HttpContext.Request.Cookies["token"].Value);

        //            var groupInfoUrl = Url.RouteUrl(
        //                        "GetGroupInfo",
        //                        new { httproute = "", controller = "Group", action = "GetGroupInfo",id=groupId},
        //                        Request.Url.Scheme
        //                    );
        //            GroupDetails groupDetails = new GroupDetails();
        //            using (var response = client.GetAsync(groupInfoUrl).Result)
        //            {

        //                if (response.IsSuccessStatusCode)
        //                {

        //                    groupDetails = response.Content.ReadAsAsync<GroupDetails>().Result;

                           

        //                }

        //            }

        //            var groupProductsUrl = Url.RouteUrl(
        //                "GetGroupProductDetails",
        //                new { httproute = "", controller = "Group", action = "GetGroupProductDetails", id = groupId },
        //                Request.Url.Scheme
        //            );
        //            IEnumerable<GroupProductDetails> groupProducts = new List<GroupProductDetails>();
        //            using (var response = client.GetAsync(groupProductsUrl).Result)
        //            {

        //                if (response.IsSuccessStatusCode)
        //                {

        //                    groupProducts = response.Content.ReadAsAsync<List<GroupProductDetails>>().Result.ToList();

        //                }

        //            }

        //            var groupMembersUrl = Url.RouteUrl(
        //               "GetGroupMemberDetails",
        //               new { httproute = "", controller = "Group", action = "GetGroupMemberDetails", id = groupId },
        //               Request.Url.Scheme
        //           );
        //            IEnumerable<GroupMemberDetails> groupMembers = new List<GroupMemberDetails>();
        //            using (var response = client.GetAsync(groupMembersUrl).Result)
        //            {

        //                if (response.IsSuccessStatusCode)
        //                {

        //                    groupMembers = response.Content.ReadAsAsync<List<GroupMemberDetails>>().Result.ToList();

        //                }

        //            }

        //            GroupInfo groupInfo = new GroupInfo();

        //            groupInfo.groupDetails = groupDetails;

        //            groupInfo.groupProducts = groupProducts.ToList();

        //            groupInfo.groupMembers = groupMembers.ToList();

        //            return PartialView("_PartialCreateGroup", groupInfo);
        //        }
        //    }

        //    return null;
        //}

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
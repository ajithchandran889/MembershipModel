using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using OneConnect.ViewModels;
using OneConnect.Models;
using System.Web;
using System.Net.Mail;
using System.Data.Entity.Validation;
using System.Data.Entity;
using System.Threading.Tasks;
using Microsoft.Owin.Security.Cookies;
using System.Web.Security;
using Microsoft.AspNet.Identity;
using OneConnect.Utils;
using System.Configuration;
using Newtonsoft.Json;
namespace OneConnect.Controllers.Api
{
    [RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {
        OneKonnectEntities DBEntities = new OneKonnectEntities();
        private AuthRepository _repo = null;
        public AccountController()
        {
            _repo = new AuthRepository();
        }
        //POST api/Account/InitialRegister
        [HttpPost]
        [AllowAnonymous]
        [Route("InitialRegister")]
        public HttpResponseMessage InitialRegister(Register reg)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            else
            {
                try
                {
                    var response = reg.captchaResponse;
                    //secret that was generated in key value pair
                    string secret = ConfigurationManager.AppSettings["ReCaptchaPrivateKey"];

                    var captchaResponse = CaptchaValidation.CaptchaValidate(secret, response, "");


                    //when response is false check for the error message
                    if (!captchaResponse.Success && captchaResponse.ErrorCodes.Count > 0)
                    {


                        var error = captchaResponse.ErrorCodes[0].ToLower();
                        switch (error)
                        {
                            case ("missing-input-secret"):
                                return Request.CreateResponse<string>(HttpStatusCode.BadRequest, "The secret parameter is missing.");

                            case ("invalid-input-secret"):
                                return Request.CreateResponse<string>(HttpStatusCode.BadRequest, "The secret parameter is invalid or malformed.");


                            case ("missing-input-response"):
                                return Request.CreateResponse<string>(HttpStatusCode.BadRequest, "Please input Captcha");

                            case ("invalid-input-response"):
                                return Request.CreateResponse<string>(HttpStatusCode.BadRequest, "Captcha is invalid or malformed.");


                            default:
                                return Request.CreateResponse<string>(HttpStatusCode.BadRequest, "Error occured. Please try again");

                        }
                    }
                    else
                    {

                        //DateTime existingUserRegisteredTime = DBEntities.Registrations.Where(r => r.Email == reg.emailId && r.IsDeleted == false).Select(r => r.CreatedAt).SingleOrDefault() ?? DateTime.Now;
                        var existingUserRegisteredDetails = DBEntities.Registrations.Where(r => r.Email == reg.emailId && r.IsDeleted == false).FirstOrDefault();
                        //AspNetUser existingUser = DBEntities.AspNetUsers.Where(u => u.Email == reg.emailId).FirstOrDefault();

                        AspNetUser existingUser = (from u in DBEntities.AspNetUsers
                                                                 join ua in DBEntities.UsersAditionalInfoes on u.Id equals ua.AspNetUserId
                                                                 where ua.IsOwner == true && u.Email == reg.emailId
                                                                 select u).FirstOrDefault();
                
                        //return Request.CreateResponse(HttpStatusCode.BadRequest, "Account already exist") ;
                        DateTime existingUserRegisteredTime;
                        if (existingUserRegisteredDetails == null)
                        {
                            existingUserRegisteredTime = DateTime.Now;
                        }
                        else
                        {
                            existingUserRegisteredTime = (DateTime)existingUserRegisteredDetails.CreatedAt;
                            existingUserRegisteredTime = existingUserRegisteredTime.AddHours(24);
                        }

                        if (existingUser != null)
                        {
                            return Request.CreateResponse<string>(HttpStatusCode.BadRequest, "Account already exist");
                        }
                        else if (existingUserRegisteredDetails != null && existingUserRegisteredTime > DateTime.Now)
                        {
                            return Request.CreateResponse<string>(HttpStatusCode.BadRequest, "Already Registered,waiting for aprovel");
                        }
                        else
                        {
                            if (existingUserRegisteredDetails != null)
                            {
                                existingUserRegisteredDetails.IsDeleted = true;
                                DBEntities.Entry(existingUserRegisteredDetails).State = EntityState.Modified;
                                DBEntities.SaveChanges();
                            }
                            Registration registerRow = new Registration();
                            registerRow.Email = reg.emailId;
                            registerRow.Password = reg.password;
                            registerRow.Token = Guid.NewGuid().ToString();
                            registerRow.CreatedAt = DateTime.Now;
                            registerRow.LastModifiedAt = DateTime.Now;
                            string host = Dns.GetHostName();
                            registerRow.IpAddress = Dns.GetHostByName(host).AddressList[0].ToString();
                            registerRow.IsDeleted = false;
                            DBEntities.Registrations.Add(registerRow);
                            DBEntities.SaveChanges();
                            string registerationToken = HttpContext.Current.Server.UrlEncode(registerRow.Token);
                            var url = new Uri(Url.Link("UserActivationRoute", new { token = registerationToken }));
                            string htmlBody;
                            htmlBody = DBEntities.EmailTemplates.Where(e => e.templateType == "registration").Select(e => e.templateBody).SingleOrDefault();
                            htmlBody = htmlBody.Replace("{Name}","");
                            htmlBody = htmlBody.Replace("{Activation Link}", url.ToString());
                            htmlBody = htmlBody.Replace("{support email}", ConfigurationManager.AppSettings["supportEmail"]);
                            string subject = DBEntities.EmailTemplates.Where(e => e.templateType == "registration").Select(e => e.templateSubject).SingleOrDefault();
                            MailClient.SendMessage(ConfigurationManager.AppSettings["adminEmail"], reg.emailId, subject, true, htmlBody);
                            return Request.CreateResponse<int>(HttpStatusCode.OK, 1);
                        }
                    }

                }
                catch (Exception e)
                {
                    return Request.CreateResponse<string>(HttpStatusCode.BadRequest, "Sorry,something went wrong!");
                }
            }



        }
        //POST api/Account/Activate
        [HttpGet]
        [AllowAnonymous]
        [Route("UserActivation", Name = "UserActivationRoute")]
        //[Route("UserActivation")]
        public async Task<HttpResponseMessage> Activate(string token)
        {
            try
            {
                string registerationToken = HttpContext.Current.Server.UrlDecode(token);
                var userData = (Registration)DBEntities.Registrations.Where(u => u.Token == registerationToken).FirstOrDefault();
                DateTime existingUserRegisteredTime = (DateTime)userData.CreatedAt;
                existingUserRegisteredTime = existingUserRegisteredTime.AddHours(24);
                if ((bool)userData.IsDeleted)
                {
                    return Request.CreateResponse<string>(HttpStatusCode.OK, "Already registered");
                }
                else if (existingUserRegisteredTime < DateTime.Now)
                {
                    return Request.CreateResponse<string>(HttpStatusCode.OK, "Token expired,please register again");
                }
                else if (userData != null)
                {
                    var lastid = DBEntities.UsersAditionalInfoes.Where(u => u.CustomUserId.StartsWith("W")).OrderByDescending(u => u.Id).Select(u => u.CustomUserId).First();
                    int newIdCount = 0;
                    if (lastid == null)
                    {
                        newIdCount = 1;
                    }
                    else
                    {
                        lastid = lastid.Replace("W", "");
                        newIdCount = Convert.ToInt32(lastid);
                        newIdCount++;
                    }
                    UserModel userModel = new UserModel();
                    userModel.UserName = "W" + newIdCount.ToString("000000");
                    userModel.Email = userData.Email;
                    userModel.Password = userData.Password;
                    string userId = await _repo.RegisterUser(userModel);

                    //IdentityResult result = await _repo.RegisterUser(userModel);

                    //IHttpActionResult errorResult = GetErrorResult(result);
                    //if (errorResult != null)
                    //{
                    //    //return errorResult;
                    //}
                    UsersAditionalInfo user = new UsersAditionalInfo();
                    user.AspNetUserId = userId;
                    user.CreatedBy = userId;
                    user.LastModifiedBy = userId;
                    user.CustomUserId = "W" + newIdCount.ToString("000000");
                    user.Status = true;
                    user.CreatedAt = DateTime.Now;
                    user.LastModifiedAt = DateTime.Now;
                    user.IpAddress = userData.IpAddress;
                    user.IsDeleted = false;
                    user.IsOwner = true;
                    DBEntities.UsersAditionalInfoes.Add(user);
                    userData.IsDeleted = true;
                    DBEntities.Entry(userData).State = EntityState.Modified;
                    DBEntities.SaveChanges();
                    string htmlBody;
                    htmlBody = DBEntities.EmailTemplates.Where(e => e.templateType == "registrationSuccessful").Select(e => e.templateBody).SingleOrDefault();
                    htmlBody = htmlBody.Replace("{Name}", "");
                    htmlBody = htmlBody.Replace("{User Id}", user.CustomUserId);
                    htmlBody = htmlBody.Replace("{Password}", userModel.Password);

                    string subject = DBEntities.EmailTemplates.Where(e => e.templateType == "registrationSuccessful").Select(e => e.templateSubject).SingleOrDefault();

                    MailClient.SendMessage(ConfigurationManager.AppSettings["adminEmail"], userData.Email, subject, true, htmlBody);
                    return Request.CreateResponse<string>(HttpStatusCode.OK, "Registration successfull and user id mailed to you");
                }
                else
                {
                    return Request.CreateResponse<string>(HttpStatusCode.OK, "Record doesn't exist");
                }
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    string str = eve.Entry.Entity.GetType().Name;
                    EntityState state = eve.Entry.State;
                    foreach (var ve in eve.ValidationErrors)
                    {
                        var propertyName = ve.PropertyName;
                        var message = ve.ErrorMessage;
                    }
                }
            }
            return Request.CreateResponse<int>(HttpStatusCode.OK, 1);
        }
        [Route("Logout")]
        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage Logout()
        {
            var authentication = HttpContext.Current.GetOwinContext().Authentication;
            authentication.SignOut(DefaultAuthenticationTypes.ExternalBearer);
            return Request.CreateResponse<int>(HttpStatusCode.OK, 1);
        }
        //POST api/Account/UserRegister
        [HttpPost]
        [Authorize]
        [Route("UserRegister")]
        public async Task<HttpResponseMessage> UserRegister(Register reg)
        {

            try
            {
                var userId = GetUserIdByName(User.Identity.GetUserName());
                var existingUserRegisteredDetails = (from u in DBEntities.AspNetUsers
                                                     join ua in DBEntities.UsersAditionalInfoes on u.Id equals ua.AspNetUserId
                                                     where u.Email == reg.emailId && ua.CreatedBy == userId
                                                     select new { u.Id }).FirstOrDefault();
                if (existingUserRegisteredDetails != null)
                {
                    return Request.CreateResponse<string>(HttpStatusCode.BadRequest, "Account with email " + reg.emailId + " already exists");
                }
                else
                {

                    var lastid = DBEntities.UsersAditionalInfoes.Where(u => u.CustomUserId.StartsWith("U")).OrderByDescending(u => u.Id).Select(u => u.CustomUserId).FirstOrDefault();
                    int newIdCount = 0;
                    if (lastid == null)
                    {
                        newIdCount = 1;
                    }
                    else
                    {
                        lastid = lastid.Replace("U", "");
                        newIdCount = Convert.ToInt32(lastid);
                        newIdCount++;
                    }
                    UserModel userModel = new UserModel();
                    userModel.UserName = "U" + newIdCount.ToString("000000");
                    userModel.Email = reg.emailId;
                    userModel.Password = reg.password;
                    string newUserId = await _repo.RegisterUser(userModel);

                    UsersAditionalInfo user = new UsersAditionalInfo();
                    user.AspNetUserId = newUserId;
                    user.CreatedBy = userId;
                    user.LastModifiedBy = userId;
                    user.CustomUserId = "U" + newIdCount.ToString("000000");
                    user.Status = true;
                    user.CreatedAt = DateTime.Now;
                    user.LastModifiedAt = DateTime.Now;
                    string host = Dns.GetHostName();
                    user.IpAddress = Dns.GetHostByName(host).AddressList[0].ToString();
                    user.IsDeleted = false;
                    user.IsOwner = false;
                    DBEntities.UsersAditionalInfoes.Add(user);
                    DBEntities.SaveChanges();

                    UsersAditionalInfo parentUserAdditionalInfo = DBEntities.UsersAditionalInfoes.Where(a => a.AspNetUserId == userId).SingleOrDefault();

                    var parentUserInfo = DBEntities.AspNetUsers.Where(u => u.Id == userId).SingleOrDefault();
                    
                    string htmlBody;
                    htmlBody = DBEntities.EmailTemplates.Where(e => e.templateType == "userAddOwner").Select(e => e.templateBody).SingleOrDefault();
                    htmlBody = htmlBody.Replace("{Name}", parentUserAdditionalInfo.Name);
                    htmlBody = htmlBody.Replace("{Account Name}", parentUserAdditionalInfo.CustomUserId);
                    htmlBody = htmlBody.Replace("{User Id}", user.CustomUserId);
                    htmlBody = htmlBody.Replace("{support email}", ConfigurationManager.AppSettings["supportEmail"]);

                    string subject = DBEntities.EmailTemplates.Where(e => e.templateType == "userAddOwner").Select(e => e.templateSubject).SingleOrDefault();

                    MailClient.SendMessage(ConfigurationManager.AppSettings["adminEmail"], parentUserInfo.Email, subject, true, htmlBody);

                    
                    htmlBody = DBEntities.EmailTemplates.Where(e => e.templateType == "newUserWelcome").Select(e => e.templateBody).SingleOrDefault();
                    htmlBody = htmlBody.Replace("{Name}", user.Name);
                    htmlBody = htmlBody.Replace("{Parent Name}", parentUserAdditionalInfo.Name);
                    htmlBody = htmlBody.Replace("{Account Name}", parentUserAdditionalInfo.CustomUserId);
                    htmlBody = htmlBody.Replace("{User Id}", user.CustomUserId);
                    htmlBody = htmlBody.Replace("{Password}", userModel.Password);
                    htmlBody = htmlBody.Replace("{Login Page}", reg.hostName);

                    subject = DBEntities.EmailTemplates.Where(e => e.templateType == "newUserWelcome").Select(e => e.templateSubject).SingleOrDefault();
                    subject = subject.Replace("{Account Name}", parentUserAdditionalInfo.CustomUserId);

                    MailClient.SendMessage(ConfigurationManager.AppSettings["adminEmail"], userModel.Email, subject, true, htmlBody);

                }
                return Request.CreateResponse<int>(HttpStatusCode.OK, 1);

            }
            catch (Exception e)
            {
                return Request.CreateResponse<string>(HttpStatusCode.BadRequest, "Sorry,somehting went wrong.Please try again.");
            }



        }
        //POST api/Account/DisableEnableUser
        [HttpPost]
        [Authorize]
        [Route("DisableEnableUser")]
        public HttpResponseMessage DisableEnableUser(UserActiveStatus usAct)
        {

            try
            {
                var userInfo = DBEntities.UsersAditionalInfoes.Where(u => u.AspNetUserId == usAct.userId).SingleOrDefault();
                userInfo.Status = usAct.status;
                DBEntities.UsersAditionalInfoes.Attach(userInfo);
                var entry = DBEntities.Entry(userInfo);
                entry.Property(u => u.Status).IsModified = true;
                DBEntities.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, 1);
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Exception");
            }



        }
        //POST api/Account/ChangeEmail
        [HttpPost]
        [Authorize]
        [Route("ChangeEmail")]
        public HttpResponseMessage ChangeEmail(UserChangeEmail usEmail)
        {

            try
            {
                var userInfo = DBEntities.AspNetUsers.Where(u => u.Id == usEmail.userId).SingleOrDefault();
                userInfo.Email = usEmail.emailId;
                //userInfo.UserName = usEmail.emailId;
                DBEntities.AspNetUsers.Attach(userInfo);
                var entry = DBEntities.Entry(userInfo);
                entry.Property(u => u.Email).IsModified = true;
                //entry.Property(u => u.UserName).IsModified = true;
                DBEntities.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, 1);
            }
            catch (Exception e)
            {
                return Request.CreateResponse<string>(HttpStatusCode.BadRequest, "Sorry,something went wrong.Please try again.");
            }



        }
        //POST api/Account/ChangeUserInfo
        [HttpPost]
        [Authorize]
        [Route("ChangeUserInfo")]
        public HttpResponseMessage ChangeUserInfo(UserEditInfo usrInfo)
        {

            try
            {
                var userInfo = DBEntities.UsersAditionalInfoes.Where(u => u.AspNetUserId == usrInfo.userId).SingleOrDefault();
                userInfo.Name = usrInfo.name;
                userInfo.ContactInfo = usrInfo.contact;
                userInfo.Address = usrInfo.address;
                userInfo.Status = usrInfo.status;
                //userInfo.UserName = usEmail.emailId;
                DBEntities.UsersAditionalInfoes.Attach(userInfo);
                var entry = DBEntities.Entry(userInfo);
                entry.Property(u => u.Name).IsModified = true;
                entry.Property(u => u.ContactInfo).IsModified = true;
                entry.Property(u => u.Address).IsModified = true;
                entry.Property(u => u.Status).IsModified = true;
                //entry.Property(u => u.UserName).IsModified = true;
                DBEntities.SaveChanges();
                GroupController groupController = new GroupController();

                groupController.DeleteUserMemberships(userInfo.AspNetUserId);

                return Request.CreateResponse(HttpStatusCode.OK, 1);
            }
            catch (Exception e)
            {
                return Request.CreateResponse<string>(HttpStatusCode.BadRequest, "Sorry,something went wrong.Please try again.");
            }



        }
        public string GetUserIdByName(string name)
        {
            try
            {
                var user = DBEntities.AspNetUsers.Where(u => u.UserName == name).FirstOrDefault();
                return user.Id;
            }
            catch (Exception e)
            {

            }
            return null;
        }
        public string GetCustomUserId(string userId)
        {
            try
            {
                var user = DBEntities.UsersAditionalInfoes.Where(u => u.AspNetUserId == userId).FirstOrDefault();
                return user.CustomUserId;
            }
            catch (Exception e)
            {

            }
            return null;
        }
        //POST api/Account/GetAccountInfo
        [HttpGet]
        [Authorize]
        [Route("GetAccountInfo")]
        public AccountInfo GetAccountInfo()
        {
            AccountInfo info = null;
            try
            {
                string userId = GetUserIdByName(User.Identity.Name);

                info = (from a in DBEntities.AspNetUsers
                        join ua in DBEntities.UsersAditionalInfoes on a.Id equals ua.AspNetUserId
                        where a.Id == userId
                        select new { r = a, s = ua }).Select(t => new AccountInfo { userId = t.r.Id, customUserId = t.r.UserName, email = t.r.Email, name = t.s.Name, company = t.s.CompanyName, address = t.s.Address, contact = t.s.ContactInfo, status = t.s.Status.Value, isOwner = (bool)t.s.IsOwner }).SingleOrDefault();

            }
            catch (Exception e)
            {
            }
            return info;
        }
        // POST api/Account/ChangePassword
        [Route("ChangePassword")]
        [HttpPost]
        [Authorize]
        public async Task<IHttpActionResult> ChangePassword(ChangePasswordBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result = await _repo.ChangePassword(model, User.Identity.Name);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }
        // POST api/Account/ChangeEmailWithPassword
        [Route("ChangeEmailWithPassword")]
        [HttpPost]
        [Authorize]
        public HttpResponseMessage ChangeEmailWithPassword(ChangeEmail model)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse<string>(HttpStatusCode.OK, "Invalid data");
            }
            var user = DBEntities.AspNetUsers.Where(u => u.UserName == model.userId && u.Email == model.oldEmailId).FirstOrDefault();
            if (user == null)
            {
                return Request.CreateResponse<string>(HttpStatusCode.BadRequest, "Invalid details");
            }
            string oldHash = user.PasswordHash;
            bool result = _repo.VerifyHashedPassword(oldHash, model.password);
            if (result)
            {
                string emailToken = Guid.NewGuid().ToString();

                var userAdditionalInfo = DBEntities.UsersAditionalInfoes.Where(u => u.CustomUserId == model.userId).SingleOrDefault();
                userAdditionalInfo.emailResetToken = emailToken;
                userAdditionalInfo.newEmailRequested = model.emailId;
                DBEntities.UsersAditionalInfoes.Attach(userAdditionalInfo);
                var entry = DBEntities.Entry(userAdditionalInfo);
                entry.Property(u => u.emailResetToken).IsModified = true;
                entry.Property(u => u.newEmailRequested).IsModified = true;
                DBEntities.SaveChanges();
                emailToken = HttpContext.Current.Server.UrlEncode(emailToken);
                var url = model.hostName + "/Home/ChangeEmail?token=" + emailToken;

                string htmlBody;
                htmlBody = DBEntities.EmailTemplates.Where(e => e.templateType == "changeEmail").Select(e => e.templateBody).SingleOrDefault();
                htmlBody = htmlBody.Replace("{Name}", userAdditionalInfo.Name);
                htmlBody = htmlBody.Replace("{User Id}", userAdditionalInfo.CustomUserId);
                htmlBody = htmlBody.Replace("{Old Email}", user.Email);
                htmlBody = htmlBody.Replace("{New Email}", userAdditionalInfo.newEmailRequested);
                htmlBody = htmlBody.Replace("{confirmLink}",  url);
                htmlBody = htmlBody.Replace("{support email}", ConfigurationManager.AppSettings["supportEmail"]);

                string subject = DBEntities.EmailTemplates.Where(e => e.templateType == "changeEmail").Select(e => e.templateSubject).SingleOrDefault();

                MailClient.SendMessage(ConfigurationManager.AppSettings["adminEmail"], model.oldEmailId, subject, true, htmlBody);
                return Request.CreateResponse<string>(HttpStatusCode.OK, "Email change request received and confirmation mail send to your current mail id");
            }
            else
            {
                return Request.CreateResponse<string>(HttpStatusCode.BadRequest, "Invalid details");
            }


        }
        // POST api/Account/EditAccountInfo
        [Route("EditAccountInfo")]
        [HttpPost]
        [Authorize]
        public HttpResponseMessage EditAccountInfo(AccountInfo model)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse<string>(HttpStatusCode.OK, "Invalid data");
            }
            var response = model.captchaResponse;
            //secret that was generated in key value pair
            string secret = ConfigurationManager.AppSettings["ReCaptchaPrivateKey"];

            var captchaResponse = CaptchaValidation.CaptchaValidate(secret, response, "");


            //when response is false check for the error message
            if (!captchaResponse.Success && captchaResponse.ErrorCodes.Count > 0)
            {


                var error = captchaResponse.ErrorCodes[0].ToLower();
                switch (error)
                {
                    case ("missing-input-secret"):
                        return Request.CreateResponse<string>(HttpStatusCode.BadRequest, "The secret parameter is missing.");

                    case ("invalid-input-secret"):
                        return Request.CreateResponse<string>(HttpStatusCode.BadRequest, "The secret parameter is invalid or malformed.");


                    case ("missing-input-response"):
                        return Request.CreateResponse<string>(HttpStatusCode.BadRequest, "Please input Captcha");

                    case ("invalid-input-response"):
                        return Request.CreateResponse<string>(HttpStatusCode.BadRequest, "Captcha is invalid or malformed.");


                    default:
                        return Request.CreateResponse<string>(HttpStatusCode.BadRequest, "Error occured. Please try again");

                }
            }
            else
            {


                string userId = GetUserIdByName(User.Identity.Name);
                UsersAditionalInfo additionInfo = DBEntities.UsersAditionalInfoes.Where(u => u.AspNetUserId == userId).FirstOrDefault();
                additionInfo.Name = model.name;
                additionInfo.CompanyName = model.company;
                additionInfo.ContactInfo = model.contact;
                additionInfo.Address = model.address;
                additionInfo.Status = model.status;
                DBEntities.UsersAditionalInfoes.Attach(additionInfo);
                var entry = DBEntities.Entry(additionInfo);
                entry.Property(a => a.Name).IsModified = true;
                entry.Property(a => a.CompanyName).IsModified = true;
                entry.Property(a => a.ContactInfo).IsModified = true;
                entry.Property(a => a.Address).IsModified = true;
                entry.Property(a => a.Status).IsModified = true;
                DBEntities.SaveChanges();

                if (!model.status)
                {
                    List<UsersAditionalInfo> users = DBEntities.UsersAditionalInfoes.Where(u => u.CreatedBy == userId).ToList();
                    foreach (UsersAditionalInfo user in users)
                    {
                        user.Status = false;
                        user.LastModifiedAt = DateTime.Now;
                        user.LastModifiedBy = userId;
                        DBEntities.UsersAditionalInfoes.Attach(user);
                        var userEntry = DBEntities.Entry(user);
                        userEntry.Property(a => a.Status).IsModified = true;
                        userEntry.Property(a => a.LastModifiedAt).IsModified = true;
                        userEntry.Property(a => a.LastModifiedBy).IsModified = true;
                        DBEntities.SaveChanges();
                    }
                    GroupController groupController = new GroupController();
                    List<Group> groups = DBEntities.Groups.Where(g => g.GroupOwner == userId).ToList();
                    foreach (Group group in groups)
                    {
                        groupController.DeleteGroupWithId(group.Id);
                    }
                }

                return Request.CreateResponse<string>(HttpStatusCode.OK, "updated");
            }

        }
        // POST api/Account/ForgotUserId
        [Route("ForgotUserId")]
        [HttpPost]
        [AllowAnonymous]
        public HttpResponseMessage ForgotUserId(ForgotUserId forgotUserId)
        {
            try
            {
                var response = forgotUserId.captchaResponse;
                //secret that was generated in key value pair
                string secret = ConfigurationManager.AppSettings["ReCaptchaPrivateKey"];

                var captchaResponse = CaptchaValidation.CaptchaValidate(secret, response, "");


                //when response is false check for the error message
                if (!captchaResponse.Success && captchaResponse.ErrorCodes.Count > 0)
                {


                    var error = captchaResponse.ErrorCodes[0].ToLower();
                    switch (error)
                    {
                        case ("missing-input-secret"):
                            return Request.CreateResponse<string>(HttpStatusCode.BadRequest, "The secret parameter is missing.");

                        case ("invalid-input-secret"):
                            return Request.CreateResponse<string>(HttpStatusCode.BadRequest, "The secret parameter is invalid or malformed.");


                        case ("missing-input-response"):
                            return Request.CreateResponse<string>(HttpStatusCode.BadRequest, "Please input Captcha");

                        case ("invalid-input-response"):
                            return Request.CreateResponse<string>(HttpStatusCode.BadRequest, "Captcha is invalid or malformed.");


                        default:
                            return Request.CreateResponse<string>(HttpStatusCode.BadRequest, "Error occured. Please try again");

                    }
                }
                else
                {
                    var info = (from u in DBEntities.AspNetUsers
                                where u.Email == forgotUserId.emailId
                                select new { u.UserName }).ToList();
                    if (info.Count == 0)
                    {
                        return Request.CreateResponse<string>(HttpStatusCode.BadRequest, "Email doesn't exist");
                    }

                    string htmlBody;
                    htmlBody = DBEntities.EmailTemplates.Where(e => e.templateType == "forgotUserId").Select(e => e.templateBody).SingleOrDefault();
                    htmlBody = htmlBody.Replace("{Name}", "");
                    

                    string userIds = "";
                    foreach (var userId in info)
                    {
                        userIds += userId.UserName + "<br/>";
                    }
                    htmlBody = htmlBody.Replace("{User Ids}", userIds);
                    htmlBody = htmlBody.Replace("{Login Page}", forgotUserId.hostName);
                    htmlBody = htmlBody.Replace("{support email}", ConfigurationManager.AppSettings["supportEmail"]);

                    string subject = DBEntities.EmailTemplates.Where(e => e.templateType == "forgotUserId").Select(e => e.templateSubject).SingleOrDefault();

                    MailClient.SendMessage(ConfigurationManager.AppSettings["adminEmail"], forgotUserId.emailId, subject, true, htmlBody);

                }
            }
            catch (Exception e)
            {

            }

            return Request.CreateResponse<string>(HttpStatusCode.OK, "Ok");
        }
        // POST api/Account/ForgotPassword
        [Route("ForgotPassword")]
        [HttpPost]
        [AllowAnonymous]
        public HttpResponseMessage ForgotPassword(ForgotPassword forgotPassowrd)
        {
            try
            {
                var response = forgotPassowrd.captchaResponse;
                //secret that was generated in key value pair
                string secret = ConfigurationManager.AppSettings["ReCaptchaPrivateKey"];

                var captchaResponse = CaptchaValidation.CaptchaValidate(secret, response, "");


                //when response is false check for the error message
                if (!captchaResponse.Success && captchaResponse.ErrorCodes.Count > 0)
                {


                    var error = captchaResponse.ErrorCodes[0].ToLower();
                    switch (error)
                    {
                        case ("missing-input-secret"):
                            return Request.CreateResponse<string>(HttpStatusCode.BadRequest, "The secret parameter is missing.");

                        case ("invalid-input-secret"):
                            return Request.CreateResponse<string>(HttpStatusCode.BadRequest, "The secret parameter is invalid or malformed.");


                        case ("missing-input-response"):
                            return Request.CreateResponse<string>(HttpStatusCode.BadRequest, "Please input Captcha");

                        case ("invalid-input-response"):
                            return Request.CreateResponse<string>(HttpStatusCode.BadRequest, "Captcha is invalid or malformed.");


                        default:
                            return Request.CreateResponse<string>(HttpStatusCode.BadRequest, "Error occured. Please try again");

                    }
                }
                else
                {

                    string passwordToken = Guid.NewGuid().ToString();
                    var info = DBEntities.AspNetUsers.Where(u => u.UserName == forgotPassowrd.userId && u.Email == forgotPassowrd.emailId).SingleOrDefault();

                    if (info == null)
                    {
                        return Request.CreateResponse<string>(HttpStatusCode.BadRequest, "No data found");
                    }
                    var userAdditionalInfo = DBEntities.UsersAditionalInfoes.Where(u => u.AspNetUserId == info.Id).SingleOrDefault();
                    userAdditionalInfo.passwordRecoveryToken = passwordToken;
                    DBEntities.UsersAditionalInfoes.Attach(userAdditionalInfo);
                    var entry = DBEntities.Entry(userAdditionalInfo);
                    entry.Property(u => u.passwordRecoveryToken).IsModified = true;
                    DBEntities.SaveChanges();
                    passwordToken = HttpContext.Current.Server.UrlEncode(passwordToken);
                    var url = forgotPassowrd.hostName + "/Home/RecoverPassword?token=" + passwordToken;
                    string htmlBody;
                    htmlBody = DBEntities.EmailTemplates.Where(e => e.templateType == "forgotPassword").Select(e => e.templateBody).SingleOrDefault();
                    
                    htmlBody = htmlBody.Replace("{Name}", userAdditionalInfo.Name);
                    htmlBody = htmlBody.Replace("{User Id}", info.UserName);
                    htmlBody = htmlBody.Replace("{Change Password link}", url);
                    htmlBody = htmlBody.Replace("{support email}", ConfigurationManager.AppSettings["supportEmail"]);

                    string subject = DBEntities.EmailTemplates.Where(e => e.templateType == "forgotPassword").Select(e => e.templateSubject).SingleOrDefault();

                    MailClient.SendMessage(ConfigurationManager.AppSettings["adminEmail"], forgotPassowrd.emailId, subject, true, htmlBody);

                    return Request.CreateResponse<string>(HttpStatusCode.OK, "created");
                }
            }
            catch (Exception e)
            {

            }

            return Request.CreateResponse<string>(HttpStatusCode.OK, "Ok");
        }
        //POST api/Account/RecoverPassword
        [HttpPost]
        [AllowAnonymous]
        [Route("RecoverPassword")]
        public HttpResponseMessage RecoverPassword(RecoverPassword recoverPassword)
        {
            try
            {
                string passwordRecoveryToken = HttpContext.Current.Server.UrlDecode(recoverPassword.recoveryToken);
                var userData = DBEntities.UsersAditionalInfoes.Where(u => u.passwordRecoveryToken == passwordRecoveryToken).FirstOrDefault();
                if (userData == null)
                {
                    return Request.CreateResponse<string>(HttpStatusCode.BadRequest, "Invalid details");
                }
                else
                {
                    bool result = _repo.ChangePasswordWithoutCurrentPassword(recoverPassword.newPassword, userData.CustomUserId);

                    if (result)
                    {
                        userData.passwordRecoveryToken = "";
                        DBEntities.UsersAditionalInfoes.Attach(userData);
                        var entry = DBEntities.Entry(userData);
                        entry.Property(u => u.passwordRecoveryToken).IsModified = true;
                        DBEntities.SaveChanges();
                        return Request.CreateResponse<string>(HttpStatusCode.OK, "Ok");
                    }

                    return Request.CreateResponse<string>(HttpStatusCode.BadRequest, "Invalid details");
                    
                }

            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    string str = eve.Entry.Entity.GetType().Name;
                    EntityState state = eve.Entry.State;
                    foreach (var ve in eve.ValidationErrors)
                    {
                        var propertyName = ve.PropertyName;
                        var message = ve.ErrorMessage;
                    }
                }
                return Request.CreateResponse<string>(HttpStatusCode.BadRequest, "Invalid details");
            }
        }
        //POST api/Account/changeEmailConfirmation
        [HttpPost]
        [AllowAnonymous]
        [Route("changeEmailConfirmation")]
        public HttpResponseMessage changeEmailConfirmation(ChangeEmailConfirm changeEmailConfirm)
        {
            try
            {
                string changeEmailToken = HttpContext.Current.Server.UrlDecode(changeEmailConfirm.token);
                var userData = DBEntities.UsersAditionalInfoes.Where(u => u.emailResetToken == changeEmailToken).FirstOrDefault();
                if (userData == null)
                {
                    return Request.CreateResponse<string>(HttpStatusCode.BadRequest, "Invalid details");
                }
                else
                {
                    var user = DBEntities.AspNetUsers.Where(u => u.Id == userData.AspNetUserId).FirstOrDefault();
                    string oldHash = user.PasswordHash;
                    bool result = _repo.VerifyHashedPassword(oldHash, changeEmailConfirm.password);
                    if (result)
                    {
                        user.Email = userData.newEmailRequested;
                        DBEntities.AspNetUsers.Attach(user);
                        var entry = DBEntities.Entry(user);
                        entry.Property(u => u.Email).IsModified = true;
                        DBEntities.SaveChanges();
                        userData.emailResetToken = "";
                        DBEntities.UsersAditionalInfoes.Attach(userData);
                        var entryUser = DBEntities.Entry(userData);
                        entryUser.Property(u => u.emailResetToken).IsModified = true;
                        DBEntities.SaveChanges();

                        string htmlBody;
                        htmlBody = DBEntities.EmailTemplates.Where(e => e.templateType == "changeEmailSuccessful").Select(e => e.templateBody).SingleOrDefault();
                        htmlBody = htmlBody.Replace("{Name}", userData.Name);
                        htmlBody = htmlBody.Replace("{User Id}",userData.CustomUserId);
                        htmlBody = htmlBody.Replace("{Email Id}", user.Email);
                        htmlBody = htmlBody.Replace("{support email}", ConfigurationManager.AppSettings["supportEmail"]);
                        string subject = DBEntities.EmailTemplates.Where(e => e.templateType == "changeEmailSuccessful").Select(e => e.templateSubject).SingleOrDefault();

                        MailClient.SendMessage(ConfigurationManager.AppSettings["adminEmail"], user.Email, subject, true, htmlBody);

                        return Request.CreateResponse<string>(HttpStatusCode.OK, "Ok");
                    }
                    

                    return Request.CreateResponse<string>(HttpStatusCode.BadRequest, "Invalid details");
                }

            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    string str = eve.Entry.Entity.GetType().Name;
                    EntityState state = eve.Entry.State;
                    foreach (var ve in eve.ValidationErrors)
                    {
                        var propertyName = ve.PropertyName;
                        var message = ve.ErrorMessage;
                    }
                }

                return Request.CreateResponse<string>(HttpStatusCode.BadRequest, "Invalid details");
            }
        }
        private IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }

    }
}

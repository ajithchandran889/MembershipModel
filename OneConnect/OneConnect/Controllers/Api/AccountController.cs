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
                    //DateTime existingUserRegisteredTime = DBEntities.Registrations.Where(r => r.Email == reg.emailId && r.IsDeleted == false).Select(r => r.CreatedAt).SingleOrDefault() ?? DateTime.Now;
                    var existingUserRegisteredDetails = DBEntities.Registrations.Where(r => r.Email == reg.emailId && r.IsDeleted == false).Select(r => new { r.CreatedAt }).SingleOrDefault();
                    AspNetUser existingUser = DBEntities.AspNetUsers.Where(u => u.Email == reg.emailId).FirstOrDefault();
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
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "Account already exist");
                    }
                    else if (existingUserRegisteredTime > DateTime.Now)
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "Already Registered,waiting for aprovel");
                    }
                    else
                    {
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
                        var url = Url.Link("Account", new { Controller = "Account", Action = "Activate", token = registerationToken });
                        MailMessage mail = new MailMessage();
                        SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
                        mail.From = new MailAddress("ajithchandran1990@gmail.com");
                        mail.To.Add(reg.emailId);
                        mail.Subject = "Registration link";

                        mail.IsBodyHtml = true;
                        string htmlBody;

                        htmlBody = "Click the link to activate  thee account:<a href='" + url + "'>Click here<a/>";

                        mail.Body = htmlBody;

                        SmtpServer.Port = 587;
                        SmtpServer.Credentials = new System.Net.NetworkCredential("ajithchandran1990@gmail.com", "Ayy@pp@136252");
                        SmtpServer.EnableSsl = true;

                        SmtpServer.Send(mail);
                        return Request.CreateResponse<int>(HttpStatusCode.OK, 1);
                    }

                }
                catch (Exception e)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Exception");
                }
            }



        }
        //POST api/Account/Activate
        [HttpGet]
        [AllowAnonymous]
        //[Route("Activate")]
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
                    return Request.CreateResponse<string>(HttpStatusCode.OK, "Token expired,please try again");
                }
                else if (userData != null)
                {
                    var lastid = DBEntities.UsersAditionalInfoes.Where(u => u.CustomUserId.StartsWith("W")).OrderByDescending(u => u.Id).Select(u => u.CustomUserId).SingleOrDefault();
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
                    userModel.UserName = userData.Email;
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
                        return Request.CreateResponse<string>(HttpStatusCode.BadRequest, "Account with email "+reg.emailId+" already exists");
                    }
                    else
                    {

                        var lastid = DBEntities.UsersAditionalInfoes.Where(u => u.CustomUserId.StartsWith("U")).OrderByDescending(u => u.Id).Select(u => u.CustomUserId).SingleOrDefault();
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
                        userModel.UserName = reg.emailId;
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
                        MailMessage mail = new MailMessage();
                        SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
                        mail.From = new MailAddress("ajithchandran1990@gmail.com");
                        mail.To.Add(reg.emailId);
                        mail.To.Add(User.Identity.Name);
                        mail.Subject = "Account created";
                        mail.IsBodyHtml = true;
                        string htmlBody;
                        htmlBody = "OneKonnect account created";
                        mail.Body = htmlBody;
                        SmtpServer.Port = 587;
                        SmtpServer.Credentials = new System.Net.NetworkCredential("ajithchandran1990@gmail.com", "Ayy@pp@136252");
                        SmtpServer.EnableSsl = true;
                        SmtpServer.Send(mail);
                    }
                    return Request.CreateResponse<int>(HttpStatusCode.OK, 1);
                    
                }
                catch (Exception e)
                {
                    return Request.CreateResponse<string>(HttpStatusCode.BadRequest, "Exception");
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
                userInfo.UserName = usEmail.emailId;
                DBEntities.AspNetUsers.Attach(userInfo);
                var entry = DBEntities.Entry(userInfo);
                entry.Property(u => u.Email).IsModified = true;
                entry.Property(u => u.UserName).IsModified = true;
                DBEntities.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, 1);
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Exception");
            }



        }
        public string GetUserIdByName(string name)
        {
            try
            {
                var user = DBEntities.AspNetUsers.Where(u => u.UserName == name).FirstOrDefault();
                return user.Id;
            }
            catch(Exception e)
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
                        select new { r = a, s = ua }).Select(t => new AccountInfo { userId = t.r.Id, customUserId = t.s.CustomUserId, email = t.r.Email, name = t.s.Name, company = t.s.CompanyName, address = t.s.Address, contact = t.s.ContactInfo,status=t.s.Status.Value }).SingleOrDefault();

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
            var user = DBEntities.AspNetUsers.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();

            string oldHash = user.PasswordHash;
            bool result = _repo.VerifyHashedPassword(oldHash, model.password);
            if(result)
            {
                user.Email = model.emailId;
                user.UserName = model.emailId;
                DBEntities.AspNetUsers.Attach(user);
                var entry = DBEntities.Entry(user);
                entry.Property(u => u.UserName).IsModified = true;
                entry.Property(u => u.Email).IsModified = true;
                DBEntities.SaveChanges();
                return Request.CreateResponse<string>(HttpStatusCode.OK, "Email changed");
            }
            else
            {
                return Request.CreateResponse<string>(HttpStatusCode.OK, "Invalid password");
            }
            

        }
        // POST api/Account/EditAccountInfo
        [Route("EditAccountInfo")]
        [HttpPost]
        [Authorize]
        public HttpResponseMessage EditAccountInfo(EditAccountInfo model)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse<string>(HttpStatusCode.OK, "Invalid data");
            }
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
            return Request.CreateResponse<string>(HttpStatusCode.OK, "updated");

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

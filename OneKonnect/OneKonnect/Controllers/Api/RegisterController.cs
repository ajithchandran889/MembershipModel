using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using OneKonnect.ViewModels;
using OneKonnect.Models;
using System.Web;
using System.Net.Mail;
using System.Data.Entity.Validation;
using System.Data.Entity;
using System.Threading.Tasks;
namespace OneKonnect.Controllers.Api
{
    [RoutePrefix("api/Register")]
    public class RegisterController : ApiController
    {
        OneKonnectEntities DBEntities = new OneKonnectEntities();
        private AuthRepository _repo = null;
        public RegisterController()
        {
            _repo = new AuthRepository();
        }
        //POST api/Register/InitialRegister
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
                        var url = Url.Link("ActivateApi", new { Controller = "AccountApi", Action = "Activate", token = registerationToken });
                        MailMessage mail = new MailMessage();
                        SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
                        mail.From = new MailAddress("ajithchandran1990@gmail.com");
                        mail.To.Add("ajithchandran1990@gmail.com");
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

        //POST api/Register/Activate
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
                    var lastid = DBEntities.UsersAditionalInfoes.OrderByDescending(u => u.Id).Select(u => u.CustomUserId).SingleOrDefault();
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
    }
}
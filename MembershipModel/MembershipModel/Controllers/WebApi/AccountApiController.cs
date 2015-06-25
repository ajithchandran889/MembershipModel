using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MembershipModel.ViewModels;
using System.Web;
using System.Net.Mail;
using System.Text;
using System.Web.Http.Routing;
using System.Data.Entity;
using System.Data.Entity.Validation;
namespace MembershipModel.Controllers.WebApi
{
    public class AccountApiController : ApiController
    {
        MembershipModelEntities DBEntities = new MembershipModelEntities();
        public HttpResponseMessage Register(Register reg)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
                
            else
            {
                try
                {
                    DateTime existingUserRegisteredTime = DBEntities.Registrations.Where(r => r.email == reg.emailId && r.isDeleted==false).Select(r=>r.createdAt).SingleOrDefault() ??DateTime.Now;
                    User existingUser = DBEntities.Users.Where(u => u.email == reg.emailId).FirstOrDefault();
                   
                    existingUserRegisteredTime = existingUserRegisteredTime.AddHours(24);
                    

                    if (existingUser != null)
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "Account already exist");
                    }
                    else if (existingUserRegisteredTime>DateTime.Now)
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "Already Registered,waiting for aprovel");
                    }
                    else
                    {
                        Registration registerRow = new Registration();
                        registerRow.email = reg.emailId;
                        registerRow.password = reg.password;
                        registerRow.token = Guid.NewGuid().ToString();
                        registerRow.createdAt = DateTime.Now;
                        registerRow.lastModifiedAt = DateTime.Now;
                        string host = Dns.GetHostName();
                        registerRow.ipAddress = Dns.GetHostByName(host).AddressList[0].ToString();
                        registerRow.isDeleted = false;
                        DBEntities.Registrations.Add(registerRow);
                        DBEntities.SaveChanges();
                        string registerationToken = HttpContext.Current.Server.UrlEncode(registerRow.token);
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
                catch(Exception e)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Exception");
                }
            }
                
           
            
        }
        [HttpGet]
        public HttpResponseMessage Activate(string token)
        {
            try
            {
                string registerationToken = HttpContext.Current.Server.UrlDecode(token);
                var userData = (Registration)DBEntities.Registrations.Where(u => u.token == registerationToken).FirstOrDefault();
                DateTime existingUserRegisteredTime = (DateTime)userData.createdAt;
                existingUserRegisteredTime = existingUserRegisteredTime.AddHours(24);
                if ((bool)userData.isDeleted)
                {
                    return Request.CreateResponse<string>(HttpStatusCode.OK, "Already registered");
                }
                else if (existingUserRegisteredTime<DateTime.Now)
                {
                    return Request.CreateResponse<string>(HttpStatusCode.OK, "Token expired,please try again");
                }
                else if (userData != null)
                {
                    var lastid = DBEntities.Users.OrderByDescending(u => u.id).Select(u => u.userId).SingleOrDefault();
                    int newIdCount=0;
                    if(lastid==null)
                    {
                        newIdCount = 1;
                    }
                    else
                    {
                        lastid=lastid.Replace("W", "");
                        newIdCount = Convert.ToInt32(lastid);
                        newIdCount++;
                    }
                    User user = new User();
                    user.userId = "W" + newIdCount.ToString("000000");
                    user.email = userData.email;
                    user.password = userData.password;
                    user.status = true;
                    user.createdAt = DateTime.Now;
                    user.lastModifiedAt = DateTime.Now;
                    user.ipAddress = userData.ipAddress;
                    user.isDeleted = false;
                    user.isOwner = true;
                    DBEntities.Users.Add(user);
                    userData.isDeleted = true;
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

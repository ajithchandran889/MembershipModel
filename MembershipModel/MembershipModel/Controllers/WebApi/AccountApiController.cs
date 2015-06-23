using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MembershipModel.ViewModels;
using System.Web;

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
                    return Request.CreateResponse<int>(HttpStatusCode.OK, 1);
                    
                }
                catch(Exception e)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Exception");
                }
            }
                
           
            
        }
        
    }

}

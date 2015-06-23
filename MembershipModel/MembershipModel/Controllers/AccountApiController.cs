using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MembershipModel.ViewModels;

namespace MembershipModel.Controllers
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
                    registerRow.
                    return Request.CreateResponse<int>(HttpStatusCode.OK, 1);
                }
                catch(Exception e)
                {

                }
            }
                
           
            
        }
        
    }
}

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
        public IHttpActionResult Register(int id)
        {
            if (true)
            {
                return Ok(1);
            }
            
        }
        
    }
}

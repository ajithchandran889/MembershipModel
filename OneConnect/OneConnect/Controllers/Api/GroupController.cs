using OneConnect.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace OneConnect.Controllers.Api
{
    [RoutePrefix("api/Group")]
    public class GroupController : ApiController
    {
        OneKonnectEntities DBEntities = new OneKonnectEntities();
        //POST api/Group/GetGroupsakwdksdjfhjksdhfkjdhsjk
        [HttpGet]
        [Authorize]
        [Route("GetGroups")]
        public IEnumerable<GroupDetails> GetGroups()
        {
            IEnumerable<GroupDetails> list = null;
            try
            {
                AccountController acc = new AccountController();
                string userId = acc.GetUserIdByName(User.Identity.Name);

                list = (from g in DBEntities.Groups
                        where g.GroupOwner == userId
                        select new { r = g }).Select(t => new GroupDetails { groupId = t.r.Id, groupName = t.r.Name, description = t.r.Description, isActive = (bool)t.r.IsActive }).ToList();

            }
            catch (Exception e)
            {
            }
            return list;
        }

        //POST api/Group/SaveGroup
        [HttpPost]
        [Authorize]
        [Route("SaveGroup")]
        public HttpResponseMessage SaveGroup(GroupDetails groupDetails)
        {

            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
            else
            {

            }
            
            return null;
        }

        
    }
}
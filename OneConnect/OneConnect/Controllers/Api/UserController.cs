﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using OneConnect.ViewModels;

namespace OneConnect.Controllers.Api
{
    [RoutePrefix("api/User")]
    public class UserController : ApiController
    {
        OneKonnectEntities DBEntities = new OneKonnectEntities();
        //POST api/User/GetUsers
        [HttpGet]
        [Authorize]
        [Route("GetUsers")]
        public IEnumerable<UserDetails> GetUsers()
        {
            IEnumerable<UserDetails> list=null;
            try
            {
                AccountController acc = new AccountController();
                string userId = acc.GetUserIdByName(User.Identity.Name); 
                
               list=(from u in DBEntities.AspNetUsers
                              join  ua in DBEntities.UsersAditionalInfoes on u.Id equals ua.AspNetUserId
                              where ua.CreatedBy==userId && ua.AspNetUserId!=userId select new {r=u,s=ua}).Select(t=> new UserDetails{userId=t.r.Id,customUserId=t.s.CustomUserId,emailId=t.r.Email,status=t.s.Status.Value}).ToList();
                
            }
            catch (Exception e)
            {
            }
            return list;
        }
        
    }
}
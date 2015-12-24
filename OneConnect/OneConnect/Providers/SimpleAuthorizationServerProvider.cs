using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using OneConnect.Models;
using System.Security.Claims;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace OneConnect.Providers
{
    public class SimpleAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {

            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });
            string id = "";
            using (AuthRepository _repo = new AuthRepository())
            {
                IdentityUser user = await _repo.FindUser(context.UserName, context.Password);
                
                if (user == null)
                {
                    context.SetError("invalid_grant", "The userid or password is incorrect.");
                    return;
                }
                id = user.Id;
            }
            OneKonnectEntities DBEntities=new OneKonnectEntities();
            var userDetails=DBEntities.UsersAditionalInfoes.Where(u=>u.AspNetUserId==id).SingleOrDefault();
            userDetails.Status = userDetails.Status == null ? false : userDetails.Status;
            if((!Convert.ToBoolean(userDetails.Status)) && (!Convert.ToBoolean(userDetails.IsOwner)))
            {
                context.SetError("invalid_grant", "Please contact administrator.");
                return;
            }
            else
            {
                userDetails.Status = true;
                DBEntities.Entry(userDetails).State = EntityState.Modified;
                DBEntities.SaveChanges();
            }
            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            identity.AddClaim(new Claim("sub", context.UserName));
            identity.AddClaim(new Claim("role", "user"));
            identity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));
           // identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, context.ClientId));
            context.Validated(identity);

        }
    }
}
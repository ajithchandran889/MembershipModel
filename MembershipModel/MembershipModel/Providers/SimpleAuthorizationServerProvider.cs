using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using MembershipModel.Models;
using System.Threading;
using Microsoft.Owin.Security;
namespace MembershipModel.Providers
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

            using (AuthRepository _repo = new AuthRepository())
            {
                IdentityUser user = await _repo.FindUser(context.UserName, context.Password);

                if (user == null)
                {
                    context.SetError("invalid_grant", "The user name or password is incorrect.");
                    return;
                }
            }

            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            identity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));
            identity.AddClaim(new Claim(ClaimTypes.Role, "user"));
            identity.AddClaim(new Claim("sub", context.UserName));

            var props = new AuthenticationProperties(new Dictionary<string, string>
                {
                    { 
                        "as:client_id", (context.ClientId == null) ? string.Empty : context.ClientId
                    },
                    { 
                        "userName", context.UserName
                    }
                });

            var ticket = new AuthenticationTicket(identity, props);
            context.Validated(ticket);

        }


        //public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        //{
        //    using (IdentityManager identityManager = _identityManagerFactory.CreateStoreManager())
        //    {
        //        if (!await identityManager.Passwords.CheckPasswordAsync(context.UserName, context.Password))
        //        {
        //            context.SetError("invalid_grant", "The user name or password is incorrect.");
        //            return;
        //        }

        //        string userId = await identityManager.Logins.GetUserIdForLocalLoginAsync(context.UserName);
        //        IEnumerable<Claim> claims = await GetClaimsAsync(identityManager, userId);
        //        ClaimsIdentity oAuthIdentity = CreateIdentity(identityManager, claims,
        //            context.Options.AuthenticationType);
        //        ClaimsIdentity cookiesIdentity = CreateIdentity(identityManager, claims,
        //            _cookieOptions.AuthenticationType);
        //        AuthenticationProperties properties = await CreatePropertiesAsync(identityManager, userId);
        //        AuthenticationTicket ticket = new AuthenticationTicket(oAuthIdentity, properties);
        //        context.Validated(ticket);
        //        context.Request.Context.Authentication.SignIn(cookiesIdentity);
        //    }
        //}


    }
}
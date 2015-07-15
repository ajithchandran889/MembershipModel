using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using OneConnect.ViewModels;
using OneConnect.Controllers.Api;

namespace OneConnect.Models
{
    public class AuthRepository : IDisposable
    {
        private AuthContext _ctx;

        private UserManager<IdentityUser> _userManager;

        public AuthRepository()
        {
            _ctx = new AuthContext();
            _userManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>(_ctx));
        }

        public async Task<string> RegisterUser(UserModel userModel)
        {
            IdentityUser user = new IdentityUser
            {
                UserName = userModel.UserName,
                Email = userModel.Email
            };
            
            var result = await _userManager.CreateAsync(user, userModel.Password);
            var userId = user.Id;
            return userId;
        }
        public async Task<IdentityResult> ChangePassword(ChangePasswordBindingModel model,string username)
        {

            AccountController acc = new AccountController();
            string userId = acc.GetUserIdByName(username);
            IdentityResult result = await _userManager.ChangePasswordAsync(userId, model.OldPassword,
                model.NewPassword);

            return result;
        }
        public bool ChangePasswordWithoutCurrentPassword(string newPassword, string username)
        {

            AccountController acc = new AccountController();
            string userId = acc.GetUserIdByName(username);
            _userManager.RemovePassword(userId);
            _userManager.AddPassword(userId, newPassword);
            return true;
        }
        public async Task<IdentityUser> FindUser(string userName, string password)
        {
            IdentityUser user = await _userManager.FindAsync(userName, password);

            return user;
        }

        public void Dispose()
        {
            _ctx.Dispose();
            _userManager.Dispose();

        }
        public bool VerifyHashedPassword(string oldPassword, string password)
        {
            if (_userManager.PasswordHasher.VerifyHashedPassword(oldPassword, password) != PasswordVerificationResult.Failed)
            {
                return true;
            }
            return false;
        }
    }
}
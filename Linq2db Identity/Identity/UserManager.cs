using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Linq2db_Identity.Identity;
using Linq2db_Identity.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;

namespace Linq2db_Identity.Identity
{
    public class UserManager : UserManager<User>
    {
        public UserManager(IUserStore<User> ctx)
            : base(ctx)
        {
            this.PasswordHasher = new OldSystemPasswordHasher();
        }

        public static UserManager Create(IdentityFactoryOptions<UserManager> options, IOwinContext context)
        {
            var manager = new UserManager(new UserStore<User>());
            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<User>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };
            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = false,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider = new DataProtectorTokenProvider<User>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            manager.EmailService = new EmailService();
            return manager;
        }

        public override async System.Threading.Tasks.Task<User> FindAsync(string userName, string password)
        {
            Debug.WriteLine("UserManager: FindAsync");
            var user = await this.Store.FindByNameAsync(userName);

            if (user != null)
            {
                PasswordVerificationResult result = this.PasswordHasher.VerifyHashedPassword(user.Password, password);
                if (result == PasswordVerificationResult.Success)
                {
                    return user;
                }
            }
            return null;
        }

        public override async Task<IdentityResult> ConfirmEmailAsync(string userId, string token)
        {
            Debug.WriteLine("UserManager: ConfirmEmailAsync");
            var user = await FindByIdAsync(userId);
            var store = Store as IUserEmailStore<User>;
            if (user == null)
            {
                throw new InvalidOperationException(String.Format("UserId {0} was not found",userId));
            }
            if (user.EmailConfirmationToken != token)
            {
                return IdentityResult.Failed("Invalid Token");
            }
            await store.SetEmailConfirmedAsync(user, true);
            return await UpdateAsync(user);
        }

        public override async Task<IList<string>> GetRolesAsync(string userId)
        {
            Debug.WriteLine("UserManager: GetRolesAsync");
            var stuff = await  base.GetRolesAsync(userId);
            return stuff;
        }

        

        public override Task<IList<Claim>> GetClaimsAsync(string userId)
        {
            Debug.WriteLine("UserManager: GetClaimsAsync");
            return base.GetClaimsAsync(userId);
        }

        public override Task<IdentityResult> AddLoginAsync(string userId, UserLoginInfo login)
        {
            Debug.WriteLine("UserManager: AddLoginAsync");
            return base.AddLoginAsync(userId, login);
        }

        public override async Task<ClaimsIdentity> CreateIdentityAsync(User user, string authenticationType)
        {

            Debug.WriteLine("UserManager: CreateIdentityAsync");

            // TODO: Create claims based on the user dynamically

            return await base.CreateIdentityAsync(user, authenticationType);
        }

        protected override Task<IdentityResult> UpdatePassword(
            IUserPasswordStore<User, string> passwordStore, 
            User user, 
            string newPassword)
        {
            Debug.WriteLine("UserManager: UpdatePassword");
            return base.UpdatePassword(passwordStore, user, newPassword);
        }

        public override Task<bool> VerifyUserTokenAsync(string userId, string purpose, string token)
        {
            Debug.WriteLine("UserManager: VerifyUserTokenAsync");
            return base.VerifyUserTokenAsync(userId, purpose, token);
        }

        public override Task SendEmailAsync(string userId, string subject, string body)
        {
            Debug.WriteLine("UserManager: SendEmailAsync");
            return base.SendEmailAsync(userId, subject, body);
        }

        public async Task AddCreditAsync(string userId, int credits)
        {
            var user = await this.Store.FindByIdAsync(userId);
            user.Credits += credits;
            await this.Store.UpdateAsync(user);
        }
    }
}
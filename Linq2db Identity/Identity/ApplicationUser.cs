using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Linq2db_Identity.Identity;

namespace Linq2db_Identity.Models
{
    public partial class User :IUser
    {
        public DateTime CreateDate { get; set; }
        public DateTime BirthDate { get; set; }
        public OAuthGrant AuthGrant { get; set; }
        public User()
        {
            CreateDate = DateTime.Now;
            Id = Guid.NewGuid().ToString();
            PasswordSalt = Guid.NewGuid().ToString();
            Claims = new List<UserClaim>();
            Roles = new List<UserRole>();
            EmailConfirmationToken = Guid.NewGuid().ToString();
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<User> manager, string authenticationType)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            // Add custom user claims here
            return userIdentity;
        }

        private List<UserClaim> _claims;
        public IList<UserClaim> Claims
        {
            get
            {
                return _claims;
            }

            private set
            {
                if (_claims == null)
                {
                    _claims = new List<UserClaim>();
                }

                _claims.AddRange(value);
            }
        }
        private List<UserRole> _roles;
        public IList<UserRole> Roles
        {
            get
            {
                return _roles;
            }

            private set
            {
                if (_roles == null)
                {
                    _roles = new List<UserRole>();
                }

                _roles.AddRange(value);
            }
        }
    }
}
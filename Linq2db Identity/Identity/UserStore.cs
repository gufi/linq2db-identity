using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Linq2db_Identity.Models;
using LinqToDB;
using Microsoft.AspNet.Identity;

namespace Linq2db_Identity.Identity
{
    public class UserStore<T> :
        IUserStore<T> ,
        IUserLoginStore<T>,
        IUserClaimStore<T>,
        IUserPasswordStore<T>,
        IUserSecurityStampStore<T>,
        IQueryableUserStore<T>,
        IUserTwoFactorStore<T, string>,
        IUserLockoutStore<T, string>,
        IUserEmailStore<T>,
        IUserPhoneNumberStore<T>,
        IUserRoleStore<T>,
    IDisposable where T : User, new()
    {

        private DatabaseContext _context;
        public UserStore() 
        {
            _context = new DatabaseContext("MyDatabase");
        }
        public static UserStore<User> Create()
        {
            return new UserStore<User>(); 
        }
        
        public IQueryable<T> Users
        {
            get
            {
                var q = _context.Users.ToList() as List<T>;
                var _users = ((q != null) ? q.AsQueryable() : null);
                return _users ;
            }
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public Task CreateAsync(T user)
        {
            _context.Insert(user);
            return Task.FromResult(0);
        }

        public Task UpdateAsync(T user)
        {
            _context.Update(user);
            return Task.FromResult(0);
        }

        public Task DeleteAsync(T user)
        {
            _context.Users.Delete(user1 => user1.Id == user.Id);
            return Task.FromResult(0);
        }

        public async Task<T> FindByIdAsync(string userId) 
        {
           return (T) await _context.Users.FirstOrDefaultAsync(user => user.Id == userId);
        }

        public async Task<T> FindByNameAsync(string userName)
        {
            return (T)await _context.Users.FirstOrDefaultAsync(user => user.UserName == userName);
        }

        public Task AddLoginAsync(T user, UserLoginInfo login)
        {
            _context.Insert(new Login()
            {
                Id = Guid.NewGuid().ToString(),
                LoginProvider = login.LoginProvider,
                ProviderKey = login.ProviderKey,
                UserId = user.Id

            });
            return Task.FromResult(0);
        }

        public Task RemoveLoginAsync(T user, UserLoginInfo login)
        {
            _context.Logins.Delete(
                x => x.UserId == user.Id && x.LoginProvider == login.LoginProvider && x.ProviderKey == login.ProviderKey);
            return Task.FromResult(0);
        }

        public Task<IList<UserLoginInfo>> GetLoginsAsync(T user)
        {
            return Task.FromResult<IList<UserLoginInfo>>(
                    _context.Logins.Where(x => x.UserId == user.Id)
                    .Select( x => new UserLoginInfo(x.LoginProvider,x.ProviderKey)).ToList()
                );
        }

        public async Task<T> FindAsync(UserLoginInfo login)
        {
            return (T)await (from u in _context.Users
                          join l in _context.Logins on u.Id equals l.UserId 
                          select u)
                          .FirstOrDefaultAsync() ;
        }

        

        public Task SetPasswordHashAsync(T user, string passwordHash)
        {
            user.Password = passwordHash;
            _context.Update(user);
            return Task.FromResult(0);
        }

        public Task<string> GetPasswordHashAsync(T user)
        {
            return Task.FromResult(user.Password);
        }

        public Task<bool> HasPasswordAsync(T user)
        {
            return Task.FromResult(!string.IsNullOrEmpty(user.Password));
        }

        public Task SetSecurityStampAsync(T user, string stamp)
        {
            user.SecurityStamp = stamp;
            return Task.FromResult(0);
        }

        public Task<string> GetSecurityStampAsync(T user)
        {
            return Task.FromResult(user.SecurityStamp);
        }

        
        public Task SetTwoFactorEnabledAsync(T user, bool enabled)
        {
            throw new NotImplementedException();
        }

        public Task<bool> GetTwoFactorEnabledAsync(T user)
        {
            throw new NotImplementedException();
        }

        public Task<DateTimeOffset> GetLockoutEndDateAsync(T user)
        {
            throw new NotImplementedException();
        }

        public Task SetLockoutEndDateAsync(T user, DateTimeOffset lockoutEnd)
        {
            throw new NotImplementedException();
        }

        public async Task<int> IncrementAccessFailedCountAsync(T user)
        {
            user.LoginAttempts++;
            await this.UpdateAsync(user);
            return user.LoginAttempts;
        }

        public async Task ResetAccessFailedCountAsync(T user)
        {
            user.LoginAttempts++;
            await this.UpdateAsync(user);
        }

        public Task<int> GetAccessFailedCountAsync(T user)
        {
            return Task.FromResult(user.LoginAttempts);
        }

        public Task<bool> GetLockoutEnabledAsync(T user)
        {
            return Task.FromResult(user.LockoutEnabled);
        }

        public async Task SetLockoutEnabledAsync(T user, bool enabled)
        {
            user.LockoutEnabled = enabled;
            await this.UpdateAsync(user);
        }

        public async Task SetEmailAsync(T user, string email)
        {
            user.Email = email;
            await this.UpdateAsync(user);
        }

        public Task<string> GetEmailAsync(T user)
        {
            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(T user)
        {
            return Task.FromResult(user.EmailConfirmed);
        }

        public async Task SetEmailConfirmedAsync(T user, bool confirmed)
        {
            user.EmailConfirmed = confirmed;
            await this.UpdateAsync(user);
        }

        public async Task<T> FindByEmailAsync(string email)
        {
           return (T)await _context.Users.FirstOrDefaultAsync(x => x.Email == email);
        }

        public async Task SetPhoneNumberAsync(T user, string phoneNumber)
        {
            user.PhoneNumber = phoneNumber;
            await this.UpdateAsync(user);
        }

        public Task<string> GetPhoneNumberAsync(T user)
        {
            return Task.FromResult(user.PhoneNumber);
        }

        public Task<bool> GetPhoneNumberConfirmedAsync(T user)
        {
            throw new NotImplementedException();
        }

        public Task SetPhoneNumberConfirmedAsync(T user, bool confirmed)
        {
            throw new NotImplementedException();
        }

        public async Task AddToRoleAsync(T user, string roleName)
        {
            if (!user.Roles.Select(x => x.Name).ToList().Contains(roleName))
            {
                var role = await _context.UserRoles.FirstOrDefaultAsync(x => roleName == x.Name);
                user.Roles.Add(role);
                _context.Insert(new UserRole()
                {
                    Name = roleName,
                    UserId = user.Id
                });
            }
        }

        public Task RemoveFromRoleAsync(T user, string roleName)
        {
            var rol = user.Roles.FirstOrDefault(x => x.Name == roleName);
            if (rol != null)
            {
                user.Roles.Remove(rol);
                _context.UserRoles.Delete(x => x.Name == roleName && x.UserId == user.Id);
            }
            return Task.FromResult(0);
        }

        public async Task<IList<string>> GetRolesAsync(T user)
        {
            var roles = (user.Roles as List<UserRole>);
            roles.Clear();
                roles.AddRange(await (from aprole in _context.UserRoles
                    where aprole.UserId == user.Id
                    select aprole).ToListAsync());
            
            var blah = user.Roles.Select(x => x.Name).ToList();

            return blah;
        }

        public Task<bool> IsInRoleAsync(T user, string roleName)
        {
            return Task.FromResult(user.Roles.Select(x => x.Name).Contains(roleName));
        }

        public async Task<IList<Claim>> GetClaimsAsync(T user)
        {
            var claims = (user.Claims as List<UserClaim>);
            claims.Clear();
            claims.AddRange(await (from apcplaim in _context.UserClaims
                                                               where apcplaim.UserId == user.Id
                                                                 select apcplaim).ToListAsync());
            var blah = user.Claims.Select(x => new Claim(x.ClaimType, x.ClaimValue)).ToList();

            return blah;
        }

        public Task AddClaimAsync(T user, Claim claim)
        {
            var c = user.Claims.FirstOrDefault(x => x.ClaimType == claim.Type && x.ClaimValue == claim.Value);
            if (c == null)
            {
                var cl = new UserClaim() {ClaimType = claim.Type, ClaimValue = claim.Type, UserId = user.Id};
                user.Claims.Add(cl);
                _context.Insert(cl);
            }

            return Task.FromResult(0);
        }

        public Task RemoveClaimAsync(T user, Claim claim)
        {
            var c = user.Claims.FirstOrDefault(x => x.ClaimType == claim.Type && x.ClaimValue == claim.Value);
            if (c != null)
            {
                user.Claims.Remove(
                    user.Claims.FirstOrDefault(x => x.ClaimType == claim.Type && x.ClaimValue == claim.Value));
                _context.Delete(c);
            }
            return Task.FromResult(0);
        }
    }
}
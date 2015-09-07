using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Linq2db_Identity.Identity;
using Linq2db_Identity.Models;
using LinqToDB;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;

namespace Linq2db_Identity.IdentityContext
{
    public class ApplicationOAuthProvider : OAuthAuthorizationServerProvider
    {
        public static AuthenticationProperties CreateProperties(string userName)
        {
            IDictionary<string, string> data = new Dictionary<string, string>
            {
                { "userName", userName }
            };
            return new AuthenticationProperties(data);
        }


        public override async Task ValidateClientAuthentication(
        OAuthValidateClientAuthenticationContext context)
        {
            string clientId;
            string clientSecret;
            context.OwinContext.Response.Headers["Access-Control-Allow-Origin"] = "*";
            if (!context.TryGetBasicCredentials(out clientId, out clientSecret))
            {
                context.TryGetFormCredentials(out clientId, out clientSecret);
            }
            if (clientId != null)
            {
                
                UserManager dbContext =        context.OwinContext.Get<UserManager>();

                try
                {

                    
                    var client = await dbContext.FindAsync(clientId, clientSecret);

                    if (client != null)
                    {
                        // Client has been verified.
                        
                        client.AuthGrant = OAuthGrant.ResourceOwner;
                        context.OwinContext.Set<User>("oauth:client", client);
                        context.Validated(clientId);
                    }
                    else
                    {
                        // Client could not be validated.
                        
                        context.Rejected();
                        context.SetError("invalid_client Client credentials are invalid.");
                    }
                }
                catch
                {
                    // Could not get the client through the IClientManager implementation.
                    
                    context.Rejected();
                    context.SetError("server_error");
                }
            }
            else
            {
                //for my implementation if no client id is provided use only the user/pass 
                context.Validated(clientId);
            }
        }

        public override async Task GrantResourceOwnerCredentials(
            OAuthGrantResourceOwnerCredentialsContext context)
        {
            User user = null;
            UserManager userManager = context.OwinContext.GetUserManager<UserManager>();
            User client = context.OwinContext.Get<User>("oauth:client");
            if (client == null)
            {
                try
                {
                    user = await userManager.FindAsync(context.UserName, context.Password);
                }
                catch
                {
                    // Could not retrieve the user.
                    
                    context.Rejected();
                    context.SetError("server_error");
                    // Return here so that we don't process further. Not ideal but needed to be done here.
                    return;
                }
            }
            else if (client.AuthGrant == OAuthGrant.ResourceOwner)
            {
                // Client flow matches the requested flow. Continue...
                
                try
                {
                    user = await userManager.FindAsync(context.UserName, context.Password);
                }
                catch
                {
                    // Could not retrieve the user.
                    
                    context.Rejected();
                    context.SetError("server_error");
                    // Return here so that we don't process further. Not ideal but needed to be done here.
                    return;
                }
            }
            else
            {
                // Client is not allowed for the 'Resource Owner Password Credentials Grant'.
                context.SetError(
                    "invalid_grant",
                    "Client is not allowed for the 'Resource Owner Password Credentials Grant'");

                context.Rejected();
            }
            if (user != null && user.EmailConfirmed)
            {
                try
                {
                    // User is found. Signal this by calling context.Validated
                    ClaimsIdentity identity = await userManager.CreateIdentityAsync(
                        user,
                        DefaultAuthenticationTypes.ExternalBearer);
                    //add custom claims based on environment or something?
                    context.Validated(identity);
                }
                catch
                {
                    // The ClaimsIdentity could not be created by the UserManager.
                    
                    context.Rejected();
                    context.SetError("server_error");
                }
            }
            else
            {
                // The resource owner credentials are invalid or resource owner does not exist.
                context.Rejected();
                context.SetError(
                    "access_denied",
                    "Credentials are invalid, User could not be found or the User has not validated their email account.");

                
            }
        }
    }
}
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Owin.Security.OAuth;

namespace OAuthDemo.Providers
{

    public class DemoAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {

        /// <summary>
        /// Validates the client authentication.
        /// Responsible for validating the "Client", if the client passes this parameter. 
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
        }


        /// <summary>
        /// Grants the resource owner credentials.
        /// Responsible to validate the username and password sent to the authorization server’s token endpoint
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {

            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });

            var isValid = context.UserName == "demouser" && context.Password == "demopass";

            if (!isValid)
            {
                context.SetError("invalid_grant", "The username or password is incorrect.");
                return;
            }

            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            identity.AddClaim(new Claim("sub", context.UserName));
            identity.AddClaim(new Claim("role", "user"));
            identity.AddClaim(new Claim("timespan", DateTimeOffset.UtcNow.ToString()));

            context.Validated(identity);

        }
    }
}
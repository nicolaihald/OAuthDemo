using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Infrastructure;
using Owin;

namespace OAuthDemo.Authentication
{
    public class SimpleAuthenticationOptions : AuthenticationOptions
    {

        public SimpleAuthenticationOptions(string authenticationType) : base(authenticationType)
        {
        }
        public SimpleAuthenticationOptions() : base("Simple")
        {
        }
        public string SharedSecret { get; set; }
    }

    public class SimpleAuthentication : AuthenticationMiddleware<SimpleAuthenticationOptions>
    {

        public SimpleAuthentication(OwinMiddleware next, IAppBuilder app, SimpleAuthenticationOptions options) : base(next, options)
        {

            #region --- VERIFY OPTIONS: ---
            if (String.IsNullOrWhiteSpace(options.SharedSecret))
                throw new ArgumentException(String.Format(CultureInfo.CurrentCulture,"OptionMustBeProvided", "SharedSecret"));


            #endregion
        }

        protected override AuthenticationHandler<SimpleAuthenticationOptions> CreateHandler()
        {
            // Called for each request, to create a handler for each request.
            return new SimpleAuthenticationHandler();
        }
    }

    public class SimpleAuthenticationHandler : AuthenticationHandler<SimpleAuthenticationOptions>
    {
        public SimpleAuthenticationHandler()
        {
            //throw new NotImplementedException();
        }

        protected override async Task<AuthenticationTicket> AuthenticateCoreAsync()
        {
            AuthenticationProperties properties = new AuthenticationProperties();

            try
            {
                var hmac = "";
                var query = Request.Query;
                var values = query.GetValues("HMAC");
                if (values != null && values.Count == 1)
                {
                    hmac = values[0];
                }

                if (hmac == "42")
                {
                    var identity = new ClaimsIdentity(Options.AuthenticationType, ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
                    identity.AddClaim(new Claim(ClaimTypes.Name, "MJ"));
                    return new AuthenticationTicket(identity, properties);
                }

                

            }
            catch (Exception ex)
            {
                Debug.Write(ex.Message);
            }

           
            return new AuthenticationTicket(null, properties);
        }
    }


    public static class AppBuilderExtensions
    {
        public static void UseSimpleAuthentication(this IAppBuilder app, SimpleAuthenticationOptions options)
        {
            app.Use(typeof(SimpleAuthentication), app, options);
        }
    }
}
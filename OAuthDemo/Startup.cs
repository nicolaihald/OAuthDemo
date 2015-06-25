using System;
using System.Web.Http;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using OAuthDemo.Authentication;
using OAuthDemo.Providers;
using Owin;

[assembly: OwinStartup(typeof(OAuthDemo.Startup))]
namespace OAuthDemo
{

    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            HttpConfiguration config = new HttpConfiguration();
            WebApiConfig.Register(config);

            //ConfigureOAuth(app);
            app.UseSimpleAuthentication(new SimpleAuthenticationOptions
            {
                SharedSecret = "FOO"
            });

            // wire up ASP.NET Web API to our Owin server pipeline.
            app.UseWebApi(config);
        }


        public void ConfigureOAuth(IAppBuilder app)
        {
            OAuthAuthorizationServerOptions OAuthServerOptions = new OAuthAuthorizationServerOptions()
            {
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(1),
                Provider = new DemoAuthorizationServerProvider()
            };

            // Token Generation
            app.UseOAuthAuthorizationServer(OAuthServerOptions);
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());

        }

    }
}
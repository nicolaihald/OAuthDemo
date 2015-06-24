using System.Web.Http;
using Microsoft.Owin;
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

            // wire up ASP.NET Web API to our Owin server pipeline.
            app.UseWebApi(config);
        }

    }
}
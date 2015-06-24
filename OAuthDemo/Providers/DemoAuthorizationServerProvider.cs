using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using OAuthDemo.Models;

namespace OAuthDemo.Providers
{
    internal static class Constants
    {
        public const string ClientAllowedOrigin = "as:ClientAllowedOrigin";
        public const string ClientId = "as:ClientId";
        public const string UserName = "username";
    }

    /// <summary>
    ///     Provider used by the AuthorizationServer to communicate with the web application while processing requests.
    /// </summary>
    /// <remarks>
    /// Reference: https://katanaproject.codeplex.com/SourceControl/latest#src/Microsoft.Owin.Security.OAuth/Provider/IOAuthAuthorizationServerProvider.cs
    /// </remarks>
    public class DemoAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {

        /// <summary>
        /// Called to validate that the origin of the request is a registered "client_id", and that the correct credentials for that client are
        /// present on the request. If the web application accepts Basic authentication credentials, 
        /// context.TryGetBasicCredentials(out clientId, out clientSecret) may be called to acquire those values if present in the request header. If the web 
        /// application accepts "client_id" and "client_secret" as form encoded POST parameters, 
        /// context.TryGetFormCredentials(out clientId, out clientSecret) may be called to acquire those values if present in the request body.
        /// If context.Validated is not called the request will not proceed further. 
        /// </summary>
        /// <param name="context">The context of the event carries information in and results out.</param>
        /// <returns>Task to enable asynchronous execution</returns>
        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            string clientId;
            string clientSecret;
            Client client = null;

            if (!context.TryGetBasicCredentials(out clientId, out clientSecret))
            {
                context.TryGetFormCredentials(out clientId, out clientSecret);
            }

            // check if the client passed a clientid
            if (context.ClientId == null)
            {
                //Remove the comments from the below line context.SetError, and invalidate context 
                //if you want to force sending clientId/secrects once obtain access tokens. 
                //context.Validated();
                context.SetError("invalid_clientId", "ClientId should be sent.");
                return Task.FromResult<object>(null);
            }

            // check if it matches a valid client (in our hardcoded/fake repository)
            client = Client.FindClient(context.ClientId);

            if (client == null)
            {
                context.SetError("invalid_clientId", string.Format("Client '{0}' is not registered in the system.", context.ClientId));
                return Task.FromResult<object>(null);
            }


            if (client.ApplicationType == Models.ApplicationTypes.NativeConfidential)
            {
                if (string.IsNullOrWhiteSpace(clientSecret))
                {
                    context.SetError("invalid_clientId", "Client secret should be sent.");
                    return Task.FromResult<object>(null);
                }
                else
                {
                    
                    // her kan vi evt. beslutte at klienten skal sende de ekstra parametre, som hash'en er baseret på (username, timestamp, ip etc..)
                    if (client.Secret != Helper.GetHash(clientSecret))
                    {
                        context.SetError("invalid_clientId", "Client secret is invalid.");
                        return Task.FromResult<object>(null);
                    }
                }
            }

            if (!client.Active)
            {
                context.SetError("invalid_clientId", "Client is inactive.");
                return Task.FromResult<object>(null);
            }

            // store the allowed-origin in the context so we are able to fetch it later in the authorization pipeline 
            context.OwinContext.Set<string>(Constants.ClientAllowedOrigin, client.AllowedOrigin);            

            context.Validated();
            return Task.FromResult<object>(null);
        }


        /// <summary>
        /// Called when a request to the Token endpoint arrives with a "grant_type" of "password". This occurs when the user has provided name and password
        /// credentials directly into the client application's user interface, and the client application is using those to acquire an "access_token" and 
        /// optional "refresh_token". If the web application supports the
        /// resource owner credentials grant type it must validate the context.Username and context.Password as appropriate. To issue an
        /// access token the context.Validated must be called with a new ticket containing the claims about the resource owner which should be associated
        /// with the access token. The application should take appropriate measures to ensure that the endpoint isn�t abused by malicious callers.  . 
        /// The default behavior is to reject this grant type.
        /// See also http://tools.ietf.org/html/rfc6749#section-4.3.2
        /// </summary>
        /// <param name="context">The context of the event carries information in and results out.</param>
        /// <returns>Task to enable asynchronous execution</returns>
        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            // 
            var allowedOrigin = context.OwinContext.Get<string>(Constants.ClientAllowedOrigin) ?? "*";
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { allowedOrigin });

            // hardcoded user - only this specific user will be able to request a token:
            var isValid = context.UserName == "demouser" && context.Password == "demopass";


            if (!isValid)
            {
                context.SetError("invalid_grant", "The username or password is incorrect.");
                return;
            }

            var identity = new ClaimsIdentity(context.Options.AuthenticationType);            
            identity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));
            identity.AddClaim(new Claim(ClaimTypes.Role, "user"));
            identity.AddClaim(new Claim("issued", DateTimeOffset.UtcNow.ToString()));

            // pass a few additional parameters back to the client, along with the token:
            var props = new AuthenticationProperties(new Dictionary<string, string>
                {
                    {
                        Constants.ClientId, context.ClientId ?? string.Empty
                    },
                    {
                        Constants.UserName, context.UserName
                    }
                });

            // finally, issue the token 
            var ticket = new AuthenticationTicket(identity, props);
            context.Validated(ticket);
            

        }


        /// <summary>
        /// Called at the final stage of a successful Token endpoint request. An application may implement this call in order to do any final 
        /// modification of the claims being used to issue access or refresh tokens. This call may also be used in order to add additional 
        /// response parameters to the Token endpoint's json response body.
        /// </summary>
        /// <param name="context">The context of the event carries information in and results out.</param>
        /// <returns>Task to enable asynchronous execution</returns>
        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            // Add all entries that we've added to the validated ticket (using the AuthenticationProperties):
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return Task.FromResult<object>(null);
        }

        
        /// <summary>
        /// Called when a request to the Token andpoint arrives with a "grant_type" of any other value. If the application supports custom grant types
        /// it is entirely responsible for determining if the request should result in an access_token. If context.Validated is called with ticket
        /// information the response body is produced in the same way as the other standard grant types. If additional response parameters must be
        /// included they may be added in the final TokenEndpoint call.
        /// See also http://tools.ietf.org/html/rfc6749#section-4.5
        /// </summary>
        /// <param name="context">The context of the event carries information in and results out.</param>
        /// <returns>Task to enable asynchronous execution</returns>
        public override Task GrantCustomExtension(OAuthGrantCustomExtensionContext context)
        {
            return base.GrantCustomExtension(context);
        }




    }

    public class Clients
    {
        public class Client1
        {
            public static string Id { get; set; }
            public static string RedirectUrl { get; set; }
        }

        public class Client2
        {
            public static string Id { get; set; }
            public static string RedirectUrl { get; set; }
        }
    }
}
using System.Linq;
using System.Security.Claims;
using System.Web.Http;

namespace OAuthDemo.Controllers
{
    [RoutePrefix("api/free")]
    public class FreeController : ApiController
    {
        //[Authorize]
        [Route("")]
        public IHttpActionResult Get()
        {
            var indentity = User.Identity as ClaimsIdentity;
            if (indentity != null)
            {
                return Ok(indentity.Claims.Select(x => new {  x.Type, x.Value}).ToList());
            }
            return Ok<object>(null);
        }



    }
}
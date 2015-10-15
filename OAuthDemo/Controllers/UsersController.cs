using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace OAuthDemo.Controllers
{
    /// <summary>
    ///     Test Controller.
    /// </summary>
    [RoutePrefix("api/users")]
    public class UsersController : ApiController
    {
        /// <summary>
        ///     Returns a list of users.
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [Route("")]
        public IHttpActionResult Get()
        {
            return Ok(DemoUser.CreateUsers());
        }

    }

    #region Helpers

    public class DemoUser
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string City { get; set; }

        public static List<DemoUser> CreateUsers()
        {
            List<DemoUser> list = new List<DemoUser>
            {
                new DemoUser {UserId = 42, UserName = "Thomas Schou-Moldt", City = "Søllerrød"},
                new DemoUser {UserId = 43, UserName = "Ulrik Knudsen", City = "Lorte Øen"    },
                new DemoUser {UserId = 44,UserName = "Nicolai Hald", City = "Solrød"         },
                new DemoUser {UserId = 45,UserName = "Philip Hoppe", City = "Smørum"        },
                new DemoUser {UserId = 46,UserName = "Henrik Runge", City = "Vanløse"       }
            };

            return list;
        }
    }

    #endregion
}

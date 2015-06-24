using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace OAuthDemo.Controllers
{
    [RoutePrefix("api/Orders")]
    public class OrdersController : ApiController
    {
        //[Authorize]
        [Route("")]
        public IHttpActionResult Get()
        {
            return Ok(Order.CreateOrders());
        }

    }

    #region Helpers

    public class Order
    {
        public int OrderID { get; set; }
        public string CustomerName { get; set; }
        public string City { get; set; }

        public static List<Order> CreateOrders()
        {
            List<Order> OrderList = new List<Order>
            {
                new Order {OrderID = 42, CustomerName = "Thomas Schou-Moldt", City = "Søllerrød"},
                new Order {OrderID = 43, CustomerName = "Ulrik Knudsen", City = "Lorte Øen"    },
                new Order {OrderID = 44,CustomerName = "Nicolai Hald", City = "Solrød"         },
                new Order {OrderID = 45,CustomerName = "Philip Hoppe", City = "Smørum"        },
                new Order {OrderID = 46,CustomerName = "Henrik Runge", City = "Vanløse"       }
            };

            return OrderList;
        }
    }

    #endregion
}

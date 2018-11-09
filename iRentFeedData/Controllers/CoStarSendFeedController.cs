using iRentFeedData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace iRentFeedData.Controllers
{
    
    [Route("api/sendingData")]
    public class CoStarSendFeedController : ApiController
    {
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        public bool Post(FeedDataModel data)
        {
            GetCoStarFeedData result = new GetCoStarFeedData(data);

            return result.success;
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpGet]
        public bool Get(int id, string commaSeparatedValues)
        {
            var data = new FeedDataModel { PropertyID = id, Units = commaSeparatedValues.Split(',').Select(x => Int32.Parse(x)).ToList() };
            GetCoStarFeedData result = new GetCoStarFeedData(data);

            return result.success;
        }
    }
}

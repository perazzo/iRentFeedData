using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace iRentFeedData.Models
{
    public class FeedDataModel
    {
        public int PropertyID { get; set; }
        public List<int> Units { get; set; } = null;
    }
}
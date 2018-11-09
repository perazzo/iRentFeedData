using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace iRentFeedData.Models.ModelView
{
    public class ModelParking
    {
        public string ParkingType { get; set; }
        public bool Assigned { get; set; } = false;
        public decimal AssignedFee { get; set; }
        public decimal SpaceFee { get; set; }
    }
}
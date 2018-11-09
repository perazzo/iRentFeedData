using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace iRentFeedData.Models.ModelView
{
    public class ModelUnit
    {
        public int UnitID { get; set; }
        public int FloorPlanID { get; set; }
        public string MarketingName { get; set; }
        public int UnitBedrooms { get; set; }
        public float UnitBathrooms { get; set; }
        public int MinSquareFeet { get; set; }
        public int MaxSquareFeet { get; set; }
        public Decimal UnitRent { get; set; }
        public string UnitLeasedStatus { get; set; } = "available";
        public string FloorplanName { get; set; }
        public decimal Deposit { get; set; }
        public decimal EffectiveRent { get; set; }
        public string AvailabilityMonth { get; set; }
        public string AvailabilityDay { get; set; }
        public string AvailabilityYear { get; set; }
    }
}
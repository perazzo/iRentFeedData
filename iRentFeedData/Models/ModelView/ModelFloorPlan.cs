using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace iRentFeedData.Models.ModelView
{
    public class ModelFloorPlan
    {
        public int FloorPlanID { get; set; }
        public string Name { get; set; }
        public int UnitCount { get; set; }
        public int UnitsAvailable { get; set; }
        public int Bedrooms { get; set; }
        public float Bathrooms { get; set; }
        public int Sqft { get; set; }
        public decimal Rent { get; set; }
        public List<ModelFile> Images { get; set; }
    }
}
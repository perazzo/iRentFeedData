using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace iRentFeedData.Models.ModelView
{
    public class ModelProperty
    {
        public ModelCompany Company { get; set; }
        public int PropertyID { get; set; }
        public string Name { get; set; }
        public string WebSite { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string ILS_Identification { get; set; }
        public int TotalUnits { get; set; }
        public List<ModelOfficeHour> OfficeHours { get; set; }
        public string LongDescription { get; set; }
        public int LeaseLength { get; set; }
        public List<ModelParking> ParkingList { get; set; }
        public Decimal AdminFee { get; set; }
        public Decimal ApplicationFee { get; set; }
        public Decimal NonRefundableHoldFee { get; set; }
        public List<ModelPets> Pets { get; set; }
        public List<ModelFloorPlan> FloorPlans { get; set; }
        public List<ModelUnit> Units { get; set; }
        public ModelUtilities Utilities { get; set; }
        public List<ModelFile> PropertyImages { get; set; }
        public string EmailSendXML { get; set; }
    }
}
//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace iRentFeedData.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class property_officehours
    {
        public int PropertyOfficeHoursID { get; set; }
        public Nullable<int> MondayFridayOpenTime { get; set; }
        public Nullable<int> MondayFridayCloseTime { get; set; }
        public Nullable<int> SaturdayOpenTime { get; set; }
        public Nullable<int> SaturdayCloseTime { get; set; }
        public Nullable<int> SundayOpenTime { get; set; }
        public Nullable<int> SundayCloseTime { get; set; }
        public Nullable<int> PropertyID { get; set; }
    }
}

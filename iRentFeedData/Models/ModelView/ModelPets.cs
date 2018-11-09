using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace iRentFeedData.Models.ModelView
{
    public class ModelPets
    {
        public bool Allowed { get; set; }
        public string PetType { get; set; }
        public int Count { get; set; }
        public decimal Deposit { get; set; }
        public decimal Rent { get; set; }
        public string Restriction { get; set; }
    }
}
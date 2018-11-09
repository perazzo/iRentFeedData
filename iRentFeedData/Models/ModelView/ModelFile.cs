using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace iRentFeedData.Models.ModelView
{
    public class ModelFile
    {
        public int FileID { get; set; }
        public bool Active { get; set; } = true;
        public string FileType { get; set; }
        public string Caption { get; set; }
        public string Format { get; set; }
        public string SRC { get; set; }
        public int Rank { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FP.Cloud.OnlineRateTable.Models
{
    public class GetRateTableRequest
    {
        public string Variant { get; set; }
        public string Version { get; set; }
        public int? Carrier { get; set; }
        public DateTime ValidFrom { get; set; }
        public string Culture { get; set; }
        public int StartValue { get; set; }
        public int ItemCount { get; set; }
        public bool IncludeFileData{ get; set; }
    }
}
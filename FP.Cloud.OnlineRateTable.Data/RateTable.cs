using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FP.Cloud.OnlineRateTable.Data
{
    public class RateTable
    {
        [Key]
        public int Id { get; set; }
        public string CountryIso3Code { get; set; }
        public int CarrierId { get; set; }
        public DateTime ValidFrom { get; set; }
        public string Culture { get; set; }
    }
}

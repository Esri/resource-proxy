using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FP.Cloud.OnlineRateTable.Common.RateTable
{
    public class RateTableInfo
    {
        #region properties
        public int Id { get; set; }
        [Required(ErrorMessage = "Please enter a variant description")]
        public string Variant { get; set; }

        [Display(Name = "Version number")]
        [Required(ErrorMessage = "Please specify a valid version number")]
        public string VersionNumber { get; set; }

        [Display(Name = "Carrier id")]
        [Required(ErrorMessage = "Please specify a valid carrier id")]
        public int CarrierId { get; set; }

        [Display(Name = "Carrier details")]
        [Required(ErrorMessage = "Please specify a valid carrier detail id")]
        public int CarrierDetails { get; set; }

        [DataType(DataType.Date, ErrorMessage ="This is not a valid date")]
        [Display(Name = "Valid From")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage ="Please specify a validity start date for the rate table")]
        public DateTime ValidFrom { get; set; }

        [Display(Name = "Language")]
        [Required(ErrorMessage = "Please specify a valid language description")]
        public string Culture { get; set; }

        [Display(Name = "Package files")]
        public List<RateTableFileInfo> PackageFiles { get; set; }

        [IgnoreDataMember]
        public string FormattedVersion
        {
            get { return GetFormattedVersion(); }
        }
        #endregion

        #region constructor
        public RateTableInfo()
        {
            PackageFiles = new List<RateTableFileInfo>();
        }
        #endregion

        #region private
        private string GetFormattedVersion()
        {
            string hexValue = string.Format("{0:X8}", NumberConverter.TryParse<int>(VersionNumber, int.TryParse));
            //read revision (last 4 digits)
            ushort revision = NumberConverter.Parse<ushort>(hexValue.Substring(hexValue.Length - 4), System.Globalization.NumberStyles.HexNumber, ushort.Parse);
            //read version
            ushort version = NumberConverter.Parse<ushort>(hexValue.Substring(hexValue.Length - 8, 4), System.Globalization.NumberStyles.HexNumber, ushort.Parse);
            return string.Format("{0:#0}.{1:#00}", version, revision);
        }
        #endregion
    }
}

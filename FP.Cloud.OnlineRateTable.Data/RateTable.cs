using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FP.Cloud.OnlineRateTable.Common.RateTable;

namespace FP.Cloud.OnlineRateTable.Data
{
    public class RateTable
    {
        #region properties
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Variant { get; set; }
        public string VersionNumber { get; set; }
        public int CarrierId { get; set; }
        public int CarrierDetails { get; set; }
        public DateTime ValidFrom { get; set; }
        public string Culture { get; set; }
        public virtual List<RateTableFile> PackageFiles { get; set; }
        #endregion

        #region public
        public RateTableInfo ToRateTableInfo()
        {
            return new RateTableInfo()
            {
                Id = Id,
                Variant = Variant,
                VersionNumber = VersionNumber,
                CarrierId = CarrierId,
                CarrierDetails = CarrierDetails,
                ValidFrom = ValidFrom,
                Culture = Culture,
                PackageFiles = PackageFiles.Select(p => p.ToRateTableFileInfo()).ToList()
            };
        }

        public void UpdateRateTable(RateTableInfo info)
        {
            Variant = info.Variant;
            VersionNumber = info.VersionNumber;
            CarrierId = info.CarrierId;
            CarrierDetails = info.CarrierDetails;
            ValidFrom = info.ValidFrom;
            Culture = info.Culture;
            //associated package files can't be updated by now
        }

        public static RateTable New(RateTableInfo info)
        {
            RateTable r = new RateTable();
            List<RateTableFile> fileList = new List<RateTableFile>();
            r.UpdateRateTable(info);
            foreach (RateTableFileInfo fileInfo in info.PackageFiles)
            {
                fileList.Add(RateTableFile.New(fileInfo));
            }
            r.PackageFiles = fileList;
            return r;
        }

        #endregion
    }
}

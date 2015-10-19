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
    public class RateTableFile
    {
        #region properties
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [ForeignKey("RateTable")]
        [Required]
        public int RateTableId { get; set; }
        public string FileName { get; set; }
        public int FileType { get; set; }
        public byte[] FileData { get; set; }
        public long Checksum { get; set; }
        public virtual RateTable RateTable { get; set; }
        #endregion

        #region public
        public RateTableFileInfo ToRateTableFileInfo()
        {
            return new RateTableFileInfo()
            {
                Id = Id,
                FileName = FileName,
                FileType = Enum.IsDefined(typeof(EFileType), FileType) ? (EFileType)FileType : EFileType.Undefined,
                FileData = new List<byte>(FileData),
                Checksum = Checksum
            };
        }

        public static RateTableFile New(RateTableFileInfo info)
        {
            return new RateTableFile()
            {
                FileName = info.FileName,
                FileType = (int) info.FileType,
                FileData = info.FileData.ToArray(),
                Checksum = info.Checksum
            };
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace FP.Cloud.OnlineRateTable.Common.ProductCalculation.ApiRequests
{
    [Serializable]
    [DataContract]
    public class GetRateTableRequest
    {
        /// <summary>
        /// the variant of the rate tables - if set to null all variants are returnedG
        /// </summary>
        /// <value>
        /// The variant.
        /// </value>
        [DataMember]
        public string Variant { get; set; }

        /// <summary>
        /// the specific version of the rate tables - if set to null all versions are returned
        /// </summary>
        /// <value>
        /// The version.
        /// </value>
        [DataMember]
        public string Version { get; set; }

        /// <summary>
        /// the carrier of the rate tables - if set to null all carriers are returned.
        /// </summary>
        /// <value>
        /// The carrier.
        /// </value>
        [DataMember]
        public int? Carrier { get; set; }

        /// <summary>
        /// minimum valid from date - if set to null the lower boundary of the date range is ignored
        /// </summary>
        /// <value>
        /// The minimum valid from.
        /// </value>
        [DataMember]
        public DateTime MinValidFrom { get; set; }

        /// <summary>
        /// maximum valid from date - if set to null the upper boundary of the date range is ignored
        /// </summary>
        /// <value>
        /// The maximum valid from.
        /// </value>
        [DataMember]
        public DateTime MaxValidFrom { get; set; }

        /// <summary>
        /// the culture of the rate tables - if set to null all cultures are returned.
        /// </summary>
        /// <value>
        /// The culture.
        /// </value>
        [DataMember]
        public string Culture { get; set; }

        /// <summary>
        /// the paging start index.
        /// </summary>
        /// <value>
        /// The start value.
        /// </value>
        [DataMember]
        public int StartValue { get; set; }

        /// <summary>
        /// the number of entries to be returned - if set to 0 all entries are being returned.
        /// </summary>
        /// <value>
        /// The item count.
        /// </value>
        [DataMember]
        public int ItemCount { get; set; }

        /// <summary>
        /// if set to true the Package file data is appended (long transmission time)
        /// </summary>
        /// <value>
        ///   <c>true</c> if [include file data]; otherwise, <c>false</c>.
        /// </value>
        [DataMember]
        public bool IncludeFileData{ get; set; }
    }
}
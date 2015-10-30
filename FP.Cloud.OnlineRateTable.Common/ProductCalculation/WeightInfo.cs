using FP.Cloud.OnlineRateTable.Common.Properties;
using System;
using System.Runtime.Serialization;
using System.Text;

namespace FP.Cloud.OnlineRateTable.Common.ProductCalculation
{
    [Serializable]
    [DataContract]
    public class WeightInfo
    {
        #region Constants
        private const decimal OUNCE_TO_GRAMS = 28.349523125M;
        private const decimal POUND_TO_GRAMS = 453.59237M;

        //conversion factor pound to oune
        private const int POUND_TO_OUNCE = 16;
        private const int KG_TO_GRAM = 1000;
        #endregion

        #region properties
        [DataMember]
        public int WeightValue { get; set; }
        [DataMember]
        public EWeightUnit WeightUnit { get; set; }

        [IgnoreDataMember]
        public decimal WeightInGram
        {
            get { return GetWeightInGram(); }
        }

        [IgnoreDataMember]
        public decimal WeightInOunces
        {
            get { return GetWeightInOunces(); }
        }

        [IgnoreDataMember]
        public string FormattedWeight
        {
            get { return GetFormattedtWeight(); }
        }

        [IgnoreDataMember]
        public string FormattedWeightImperial
        {
            get { return GetFormattedtWeightImperial(); }
        }
        #endregion

        #region private
        private decimal GetWeightInGram()
        {
            switch (WeightUnit)
            {
                case EWeightUnit.Gram:
                    return WeightValue;
                case EWeightUnit.TenthGram:
                    return (decimal)WeightValue / 10;
                case EWeightUnit.TenthOunce:
                    return OuncesToGram((decimal)WeightValue / 10);
                case EWeightUnit.HoundrethOunce:
                    return OuncesToGram((decimal)WeightValue / 100);
            }
            return 0;
        }

        private decimal GetWeightInOunces()
        {
            switch (WeightUnit)
            {
                case EWeightUnit.Gram:
                    return GramToOunces(WeightValue);
                case EWeightUnit.TenthGram:
                    return GramToOunces((decimal)WeightValue / 10);
                case EWeightUnit.TenthOunce:
                    return (decimal)WeightValue / 10;
                case EWeightUnit.HoundrethOunce:
                    return (decimal)WeightValue / 100;
            }
            return 0;
        }

        private string GetFormattedtWeight()
        {
            return GetMetricWeightInfo(GetWeightInGram());
        }

        private string GetFormattedtWeightImperial()
        {
            return GetImperialWeightInfo(GetWeightInOunces());
        }

        private string GetMetricWeightInfo(decimal weightInGram)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0}{1}", (int)(weightInGram / KG_TO_GRAM), Resources.LabelKilo);
            sb.AppendFormat(" {0}{1}", Math.Round((weightInGram % KG_TO_GRAM), 1), Resources.LabelGram);
            return sb.ToString();
        }

        private string GetImperialWeightInfo(decimal weightInOunces)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0}{1}", (int)(weightInOunces / POUND_TO_OUNCE), Resources.LabelPound);
            sb.AppendFormat(" {0}{1}", Math.Round((weightInOunces % POUND_TO_OUNCE), 1), Resources.LabelOunce);
            return sb.ToString();
        }


        /// <summary>
        /// Grams to ounces.
        /// </summary>
        /// <param name="grams">The grams.</param>
        /// <returns></returns>
        private decimal GramToOunces(decimal grams)
        {
            return Math.Round(grams / OUNCE_TO_GRAMS, 1);
        }

        /// <summary>
        /// Ounceses to gram.
        /// </summary>
        /// <param name="ounces">The ounces.</param>
        /// <returns></returns>
        private decimal OuncesToGram(decimal ounces)
        {
            return ounces * OUNCE_TO_GRAMS;
        }
        #endregion
    }
}

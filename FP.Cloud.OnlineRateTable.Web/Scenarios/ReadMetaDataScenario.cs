using FP.Cloud.OnlineRateTable.Common;
using FP.Cloud.OnlineRateTable.Common.RateTable;
using FP.Cloud.OnlineRateTable.Common.ScenarioRunner;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.XPath;

namespace FP.Cloud.OnlineRateTable.Web.Scenarios
{
    public class ReadMetaDataScenario
    {
        #region constants
        private const string GENERAL_QUERY = "/Package/General";
        private const string FILE_QUERY = "/Package/Content/File";
        private const string PRODUCT_TABLE_QUERY = "/Package/ProductTableSpecific";
        private const string VARIANT = "Variant";
        private const string VERSION = "Version";
        private const string FILE_NAME = "Name";
        private const string CRC = "Crc";
        private const string CARRIER = "Carrier";
        private const string CARRIER_DETAILS = "CarrierDetails";
        #endregion

        #region members
        private ScenarioRunner m_ScenarioRunner;
        #endregion

        #region constructor
        public ReadMetaDataScenario(ScenarioRunner scenarioRunner)
        {
            m_ScenarioRunner = scenarioRunner;
        }
        #endregion

        #region public
        public ScenarioResult Execute(RateTableInfo rateTable)
        {
            return m_ScenarioRunner.Run(() => ExtractMetaData(rateTable));
        }
        #endregion

        #region private
        private void ExtractMetaData(RateTableInfo rateTable)
        {
            //find the meta info file
            RateTableFileInfo metaFile = rateTable.PackageFiles.FirstOrDefault(f => f.FileType == EFileType.Meta);
            if (null != metaFile)
            {
                using (MemoryStream stream = new MemoryStream(metaFile.FileData.ToArray()))
                {
                    XPathDocument doc = new XPathDocument(stream);
                    XPathNavigator nav = doc.CreateNavigator();

                    XPathNodeIterator iter = CreateIterator(GENERAL_QUERY, nav);
                    if (iter.Count == 1)
                    {
                        iter.MoveNext();
                        rateTable.Variant = ReadStringAttribute(VARIANT, iter);
                        rateTable.VersionNumber = ReadStringAttribute(VERSION, iter);
                    }

                    iter = CreateIterator(FILE_QUERY, nav);
                    while (iter.MoveNext())
                    {
                        string fileName = ReadStringAttribute(FILE_NAME, iter);
                        long crc = NumberConverter.TryParse<long>(ReadStringAttribute(CRC, iter), long.TryParse);
                        RateTableFileInfo fileInfo = rateTable.PackageFiles.FirstOrDefault(f => f.FileName == fileName);
                        if(null != fileInfo)
                        {
                            //set crc information
                            fileInfo.Checksum = crc;
                        }
                    }

                    iter = CreateIterator(PRODUCT_TABLE_QUERY, nav);
                    if (iter.Count == 1)
                    {
                        iter.MoveNext();
                        rateTable.CarrierId = NumberConverter.TryParse<int>(ReadStringAttribute(CARRIER, iter), int.TryParse);
                        rateTable.CarrierDetails = NumberConverter.TryParse<int>(ReadStringAttribute(CARRIER_DETAILS, iter), int.TryParse);
                    }
                }
            }
        }

        private XPathNodeIterator CreateIterator(string queryString, XPathNavigator nav)
        {
            XPathExpression query = nav.Compile(queryString);
            XPathNodeIterator iter = nav.Select(query);
            return iter;

        }

        /// <summary>
        /// Reads a string value from xml.
        /// </summary>
        /// <param name="element">The child element to be read, if this is empty or null the value of
        /// the current node will be read.</param>
        /// <param name="iter">The XPathNodeIterator.</param>
        /// <returns>the string value or an empty string if the element was not found</returns>
        private string ReadStringValue(string element, XPathNodeIterator iter)
        {
            string ret = string.Empty;
            if (string.IsNullOrEmpty(element))
            {
                ret = iter.Current.Value.Trim();
            }
            else if (iter.Current.MoveToChild(element, string.Empty))
            {
                ret = iter.Current.Value.Trim();
                iter.Current.MoveToParent();
            }
            return ret;
        }

        /// <summary>
        /// Reads a string attribute from xml.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="iter">The XPathNodeIterator.</param>
        /// <returns>the string value or an empty string if the element was not found</returns>
        private string ReadStringAttribute(string attribute, XPathNodeIterator iter)
        {
            string ret = string.Empty;
            if (iter.Current.MoveToAttribute(attribute, string.Empty))
            {
                ret = iter.Current.Value.Trim();
                iter.Current.MoveToParent();
            }
            return ret;
        }
        #endregion
    }
}
using System;

namespace FP.Cloud.OnlineRateTable.Web.Formatter
{
    public class FormatToken
    {
        #region c'tors
        public FormatToken(string token, bool isFormatSpec, UInt32 width, UInt32 precision, char charType)
        {
            m_strToken = token;
            m_isFormatSpec = isFormatSpec;
            m_width = width;
            m_precision = precision;
            m_charType = charType;
        }
        #endregion

        #region members
        private string m_strToken;
        private char m_charType;
        private bool m_isFormatSpec;
        private UInt32 m_width;
        private UInt32 m_precision;
        #endregion

        #region properties
        public bool isFormatSpec
        {
            get
            {
                return m_isFormatSpec;
            }
        }
        public char charType
        {
            get
            {
                return m_charType;
            }
        }

        public UInt32 width
        {
            get
            {
                return m_width;
            }
        }

        public UInt32 precision
        {
            get
            {
                return m_precision;
            }
        }

        public string strToken
        {
            get
            {
                return m_strToken;
            }
        }
        #endregion
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FP.Cloud.OnlineRateTable.Web.Formatter
{
    public class FormatStringAdapter
    {
        #region constants
        const string FORMAT_TYPES = "duxXfsS";
        const char FORMAT_FILL_CHAR = '_';
        #endregion

        #region c'tors
        public FormatStringAdapter(char decimalSeperator)
        {
            m_DigitNumber = 0;
            m_ValueNumber = 0;
            m_DecimalSeperator = decimalSeperator;
            m_FormatTokens = new List<FormatToken>();
        }
        #endregion

        #region members
        protected string m_FormatString;
        protected List<FormatToken> m_FormatTokens;
        protected UInt32 m_DigitNumber;
        protected UInt32 m_ValueNumber;
        protected char m_DecimalSeperator;
        #endregion

        #region public methods
        /// <summary>
        /// returns CursorPosition of string from paramList
        /// </summary>
        /// <returns></returns>
        public virtual UInt32 Format(out string outString, string inString)
        {
            outString = string.Empty;
            UInt32 cursorpos = 0;
            UInt32 processed = 0;

            if (m_DigitNumber < inString.Length)
            {
                return 0;
            }
            UInt32 delta = m_DigitNumber - Convert.ToUInt32(inString.Length);

            //create an input string in the length of total expected digits, pad with fill char
            //std::wstring input(delta, FORMAT_FILL_CHAR);
            string input = new String(FORMAT_FILL_CHAR, Convert.ToInt32(delta));
            //input.append(inString);
            input += inString;

            //for(vector<FormatToken>::iterator it = m_FormatTokens.begin(); it < m_FormatTokens.end(); ++it)
            foreach (FormatToken it in m_FormatTokens)
            {
                //if(it->isFormatSpec)
                if (it.isFormatSpec)
                {
                    //out.append(input.substr(processed, it->width));
                    //new handling for uppercase strings -> wysiwyg experience for the user
                    if (it.charType == 'S')
                    {
                        outString += input.Substring(Convert.ToInt32(processed), Convert.ToInt32(it.width)).ToUpper();
                    }
                    else
                    {
                        outString += input.Substring(Convert.ToInt32(processed), Convert.ToInt32(it.width));
                    }
                    //processed += it->width;
                    processed += it.width;
                    //if(it->precision > 0)
                    if (it.precision > 0)
                    {
                        //out.append(1, m_DecimalSeperator);
                        outString += m_DecimalSeperator;
                        //out.append(input.substr(processed, it->precision));
                        outString += input.Substring(Convert.ToInt32(processed), Convert.ToInt32(it.precision));
                        //processed += it->precision;
                        processed += it.precision;
                    }
                    //cursorpos = out.length();
                    cursorpos = Convert.ToUInt32(outString.Length);
                }
                else
                {
                    //copy constant text
                    //out.append(it->strToken);
                    outString += it.strToken;
                }
            }
            return cursorpos;
        }

        /// <summary>
        /// returns the maximum number of digits
        /// </summary>
        public virtual UInt32 GetMaxDigits()
        {
            return m_DigitNumber;
        }

        /// <summary>
        /// returns the number of values in the format specification, typically 1
        /// but maybe 2 (e.g. lb + oz)
        /// </summary>
        /// <returns></returns>
        public virtual UInt32 GetValueNumber()
        {
            return m_ValueNumber;
        }

        public object GetValue(string inputText, UInt32 index)
        {
            UInt32 idxFormat = 0;
            UInt32 start = 0;
            object empty = null;

            if (GetFormatSpec(index, ref idxFormat, ref start))
            {
                UInt32 len = CalcLength(m_FormatTokens[Convert.ToInt32(idxFormat)]);
                if (inputText.Length >= (start + len))
                {
                    while ((len > 0) && (inputText[Convert.ToInt32(start)] == FORMAT_FILL_CHAR))
                    {
                        ++start;
                        --len;
                    }
                    string s = inputText.Substring(Convert.ToInt32(start), Convert.ToInt32(len));

                    // we need some more logic for an empty string, so that the following switch statement will succeed
                    if (string.IsNullOrEmpty(s))
                    {
                        s = "0";
                    }

                    UInt32 uval = 0;
                    Int32 val = 0;
                    //float fval =0;
                    double fval = 0;
                    switch (m_FormatTokens[Convert.ToInt32(idxFormat)].charType)
                    {
                        case 'x':
                        case 'X':
                            uval = Convert.ToUInt32(s, 16); // we need to convert the text expression from base-16 to an integer 
                            return uval;
                        case 'u':
                            uval = Convert.ToUInt32(s);
                            return uval;
                        case 'd':
                            val = Convert.ToInt32(s);
                            return val;
                        case 'f':
                            fval = Convert.ToDouble(s);
                            return fval;
                        case 's':
                            //return string without any changes
                            return s;
                        case 'S':
                            //return uppercase string
                            return s.ToUpper();
                    }
                }
            }
            return empty;
        }

        public void SetFormatString(string formatString)
        {
            m_DigitNumber = 0;
            m_ValueNumber = 0;
            m_FormatTokens.Clear();
            m_FormatString = formatString;
            ParseFormatString();
        }

        public string GetFormatString()
        {
            return m_FormatString;
        }

        #endregion

        #region private methods
        private void ParseFormatString()
        {
            EParsingState state = EParsingState.NoValue;
            string part = string.Empty;
            char c;
            UInt32 width = 0;
            UInt32 prec = 0;

            for (UInt32 i = 0; i < m_FormatString.Length; ++i)
            {
                c = m_FormatString[(int)i];
                switch (state)
                {
                    case EParsingState.NoValue:
                        if (c == '%')
                        {
                            state = EParsingState.ValStarted;
                            AppendToken(ref part, false, 0, 0, ' ');
                        }
                        else
                        {
                            part += c;
                        }
                        break;
                    case EParsingState.ValStarted:
                        //we expect a width specification
                        if (c == '%')
                        {
                            //2 %'s in a row
                            state = EParsingState.NoValue;
                            //m_FormatString.erase(i, 1);
                        }
                        else
                        {
                            if ((c >= '0') && (c <= '9'))
                            {
                                //a width field begins
                                part += c;
                                state = EParsingState.WidthStarted;
                            }
                            else
                            {
                                throw new Exception("FormatAdapterException::FORMAT_MISSING_WIDTH");
                            }
                        }
                        break;
                    case EParsingState.WidthStarted:
                        if ((c >= '0') && (c <= '9'))
                        {
                            //the width specifier has 2 or more digits 
                            part += c;
                        }
                        else
                        {
                            if (c == '.')
                            {
                                state = EParsingState.PrecisionStarted;
                                string s = part;
                                //s >> width;
                                width = Convert.ToUInt32(s);
                                part += c;
                            }
                            else
                            {
                                if (IsTypeChar(c))
                                {
                                    string s = part;
                                    //s >> width;
                                    width = Convert.ToUInt32(s);
                                    part += c;
                                    AppendToken(ref part, true, width, prec, c);
                                    state = EParsingState.NoValue;
                                }
                                else
                                {
                                    throw new Exception("FormatAdapterException::FORMAT_UNKNOWN_TYPE");
                                }
                            }
                        }
                        break;
                    case EParsingState.PrecisionStarted:
                        if ((c >= '0') && (c <= '9'))
                        {
                            part += c;
                            //wstringstream s(wstring(1, c));
                            string s = string.Format("{0}", c);
                            //s >> prec;
                            prec = Convert.ToUInt32(s);
                            state = EParsingState.PrecisionFinished;
                        }
                        else
                        {
                            throw new Exception("FormatAdapterException::FORMAT_INCOMPLETE_PRECISION");
                        }
                        break;
                    case EParsingState.PrecisionFinished:

                        if (IsTypeChar(c))
                        {
                            part += c;
                            //finish format value
                            AppendToken(ref part, true, width, prec, c);
                            state = EParsingState.NoValue;
                        }
                        else
                        {
                            throw new Exception("FormatAdapterException::FORMAT_UNKNOWN_TYPE");
                        }
                        break;
                }//switch
            }//for

            //append tailing characters
            AppendToken(ref part, false, 0, 0, ' ');
        }

        private void AppendToken(ref string token, bool isFormatSpec, UInt32 width, UInt32 precision, char charType)
        {
            if (!string.IsNullOrEmpty(token))
            {
                FormatToken formatToken = new FormatToken(token, isFormatSpec, width, precision, charType);
                m_FormatTokens.Add(formatToken); // Appends the formatToken at the end of the FormatToken List
                m_DigitNumber += width + precision;
                if (isFormatSpec)
                {
                    ++m_ValueNumber;
                }
                token = string.Empty;
            }
        }

        private bool IsTypeChar(char c)
        {
            for (UInt32 i = 0; i < FORMAT_TYPES.Length; ++i)
            {
                if (c == FORMAT_TYPES[Convert.ToInt32(i)])
                {
                    return true;
                }
            }
            return false;
        }

        private UInt32 CalcLength(FormatToken token)
        {
            if (token.isFormatSpec)
            {
                if (token.charType == 'f')
                {
                    return token.width + token.precision + 1; //one char for decimal point
                }
                else
                {
                    return token.width;
                }
            }
            else
            {
                return (uint)token.strToken.Length;
            }
        }

        private bool GetFormatSpec(UInt32 index, ref UInt32 tokenIndex, ref UInt32 strPos)
        {
            UInt32 fspecs = 0;
            UInt32 pos = 0;
            //find the format specifier token for the requested value
            for (UInt32 i = 0; i < m_FormatTokens.Count; ++i)
            {
                if (m_FormatTokens[Convert.ToInt32(i)].isFormatSpec)
                {
                    if (index == fspecs)
                    {
                        tokenIndex = i;
                        strPos = pos;
                        return true;
                    }
                    else
                    {
                        pos += CalcLength(m_FormatTokens[Convert.ToInt32(i)]);
                        ++fspecs;
                    }
                }
                else
                {
                    pos += (UInt32)m_FormatTokens[Convert.ToInt32(i)].strToken.Length;
                }
            }
            return false;
        }
        #endregion
    }
}
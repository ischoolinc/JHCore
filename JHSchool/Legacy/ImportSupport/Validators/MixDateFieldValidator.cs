using System;
using System.Collections.Generic;
using System.Text;
using DocValidate;
using System.Text.RegularExpressions;
using System.Xml;
using System.Globalization;

namespace JHSchool.Legacy.ImportSupport.Validators
{
    internal class MixDateFieldValidator : IFieldValidator
    {
        private const string ErrorSource = "MixDateFieldValidator";
        private const short ErrNumXmlLoadError = 1;
        private const string ErrorXmlLoadError = "無法載入 Xml 字串。";

        #region IFieldValidator 成員

        private string _date_format;
        private List<DateMatcher> _matchs;
        private DateMatcher _valid_pattern;

        public void InitFromXMLString(string XmlString)
        {
            XmlDocument xmldoc = new XmlDocument();

            xmldoc.LoadXml(XmlString);

            InitFromXMLNode(xmldoc.DocumentElement);
        }

        public void InitFromXMLNode(System.Xml.XmlElement XmlNode)
        {
            _date_format = XmlNode.SelectSingleNode("Matchs/@CorrectTo").InnerText;
            _valid_pattern = new DateMatcher(XmlNode.SelectSingleNode("ValidPattern") as XmlElement);

            _matchs = new List<DateMatcher>();
            foreach (XmlElement each in XmlNode.SelectNodes("Matchs/Match"))
                _matchs.Add(new DateMatcher(each));

        }

        public bool Validate(string Value)
        {
            if (_valid_pattern.IsMatch(Value))
            {
                Nullable<DateTime> result = _valid_pattern.Parse(Value);
                return result.HasValue;
            }
            else
                return false;
        }

        public string Correct(string Value)
        {
            DateMatcher parser = null;

            foreach (DateMatcher each in _matchs)
            {
                if (each.IsMatch(Value))
                {
                    parser = each;
                    break;
                }
            }

            if (parser != null)
            {
                Nullable<DateTime> result = parser.Parse(Value);

                if (!result.HasValue)
                    return string.Empty;
                else
                    return string.Format("<Correct>{0}</Correct>", result.Value.ToString(_date_format, DateTimeFormatInfo.InvariantInfo));
            }
            else
                return string.Empty;
        }

        public string ToString(string Description)
        {
            return Description;
        }

        #endregion

        private class DateMatcher
        {
            private string _dateType;
            private Regex _regex_match;

            public DateMatcher(XmlElement matchInfo)
            {
                _dateType = matchInfo.GetAttribute("DateType");
                _regex_match = new Regex(matchInfo.InnerText.Trim(), RegexOptions.Singleline);
            }

            private string DateType
            {
                get { return _dateType; }
            }

            public Regex CurrentRegex
            {
                get { return _regex_match; }
            }

            public bool IsMatch(string value)
            {
                return CurrentRegex.Match(value).Success;
            }

            public Nullable<DateTime> Parse(string value)
            {
                Match m = CurrentRegex.Match(value);

                if (!m.Success) return null;

                Group year = m.Groups["Year"];
                Group month = m.Groups["Month"];
                Group day = m.Groups["Day"];

                if (DateType.ToUpper() == "Gregorian".ToUpper())
                    return ParseGEDate(string.Format("{0}/{1}/{2}", year.Value, month.Value, day.Value));
                else if (DateType.ToUpper() == "Taiwan".ToUpper())
                {
                    int pYear;

                    if (int.TryParse(year.Value, out pYear))
                        pYear += 1911;
                    else
                        return null;

                    return ParseGEDate(string.Format("{0}/{1}/{2}", pYear, month.Value, day.Value));
                }
                else
                    return null;
            }

            private Nullable<DateTime> ParseGEDate(string value)
            {
                DateTime result;

                if (DateTime.TryParse(value, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.None, out result))
                    return result;
                else
                    return null;
            }
        }
    }
}

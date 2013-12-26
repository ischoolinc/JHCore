using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace JHSchool.StudentExtendControls.Ribbon.StudentImportWizardControls
{
    /// <summary>
    /// 代表處理 Xml 過程中發生錯誤。
    /// </summary>
    public class XmlParseException : Exception
    {
        private XmlElement _error_source;

        public XmlParseException(string message, XmlElement errorSource)
            : base(message)
        {
            _error_source = errorSource;
        }

        public XmlParseException(string message, XmlElement errorSource, Exception innerException)
            : base(message, innerException)
        {
            _error_source = errorSource;
        }

        public XmlElement ErrorElement
        {
            get { return _error_source; }
        }
    }
}

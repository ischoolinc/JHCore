using System;
using System.Collections.Generic;
using System.Text;
using JHSchool.Legacy.Export.RequestHandler;
using System.Xml;
using JHSchool.Legacy.Export.ResponseHandler.DataType;
using JHSchool.Legacy.Export.ResponseHandler.Converter;

namespace JHSchool.Legacy.Export.ResponseHandler
{
    public class ExportField : Field
    {
        private XmlElement _element;

        public XmlElement Element
        {
            get { return _element; }
            set { _element = value; }
        }

        private int _columnIndex;

        public int ColumnIndex
        {
            get { return _columnIndex; }
            set { _columnIndex = value; }
        }

        private string _XPath;

        public string XPath
        {
            get { return _XPath; }
            set { _XPath = value; }
        }

        private string _requestName;

        public string RequestName
        {
            get { return _requestName; }
            set { _requestName = value; }
        }

        private string _converter;

        public string Converter
        {
            get { return _converter; }
            set { _converter = value; }
        }

        private string _dataType;
        public string DataType
        {
            get { return _dataType; }
            set { _dataType = value; }
        }        
    }
}

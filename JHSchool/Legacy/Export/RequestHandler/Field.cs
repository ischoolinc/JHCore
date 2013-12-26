using System;
using System.Collections.Generic;
using System.Text;

namespace JHSchool.Legacy.Export.RequestHandler
{
    public class Field
    {
        private string _fieldName;

        public string FieldName
        {
            get { return _fieldName; }
            set { _fieldName = value; }
        }

        private string _displayText;

        public string DisplayText
        {
            get { return _displayText; }
            set { _displayText = value; }
        }

        private object _tag;

        public object Tag
        {
            get { return _tag; }
            set { _tag = value; }
        }
    }
}

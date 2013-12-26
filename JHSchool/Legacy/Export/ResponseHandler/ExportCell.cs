using System;
using System.Collections.Generic;
using System.Text;

namespace JHSchool.Legacy.Export.ResponseHandler
{
    public class ExportCell
    {
        public ExportCell()
        {

        }

        private string _value;

        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }
        private int _rowIndex;

        public int RowIndex
        {
            get { return _rowIndex; }
            set { _rowIndex = value; }
        }
        private int _columnIndex;

        public int ColumnIndex
        {
            get { return _columnIndex; }
            set { _columnIndex = value; }
        }
        private object _tag;

        public object Tag
        {
            get { return _tag; }
            set { _tag = value; }
        }
    }
}

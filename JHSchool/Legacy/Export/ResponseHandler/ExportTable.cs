using System;
using System.Collections.Generic;
using System.Text;

namespace JHSchool.Legacy.Export.ResponseHandler
{
    public class ExportTable
    {
        private ExportFieldCollection _columns;

        public ExportFieldCollection Columns
        {
            get { return _columns; }     
        }

        private ExportRowCollection _rows;

        public ExportRowCollection Rows
        {
            get { return _rows; }      
        }

        public ExportTable()
        {
            _columns = new ExportFieldCollection();
            _rows = new ExportRowCollection();
        }

        public int AddColumn(ExportField column)
        {
            int columnIndex = _columns.Add(column);
            column.ColumnIndex = columnIndex;
            return columnIndex;
        }

        public ExportRow AddRow()
        {
            return _rows.Add(this);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace JHSchool.Legacy.Export.ResponseHandler
{
    public class ExportRow
    {
        private List<ExportCell> _cells;

        public List<ExportCell> Cells
        {
            get { return _cells; }           
        }

        private int _index;
        private ExportTable _parentTable;

        public ExportTable ParentTable
        {
            get { return _parentTable; }       
        }

        public ExportRow(ExportTable parentTable, int rowIndex)
        {
            _cells = new List<ExportCell>();
            int columnIndex = 0;
            foreach (ExportField field in parentTable.Columns)
            {
                ExportCell cell = new ExportCell();
                cell.ColumnIndex = columnIndex;
                cell.RowIndex = rowIndex;
                _cells.Add(new ExportCell());
                columnIndex++;
                _index = rowIndex;
                _parentTable = parentTable;
            }
        }

        public int Index
        {
            get { return _index; }
        }

        
    }
}

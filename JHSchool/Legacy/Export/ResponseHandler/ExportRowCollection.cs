using System;
using System.Collections.Generic;
using System.Text;

namespace JHSchool.Legacy.Export.ResponseHandler
{
    public class ExportRowCollection: IEnumerable<ExportRow>
    {
        private ExportTable _parentTable;
        private List<ExportRow> _rows;
        public ExportRowCollection()
        {
            _rows = new List<ExportRow>();
        }

        public ExportRow Add(ExportTable parentTable)
        {
            ExportRow row = new ExportRow(parentTable, _rows.Count);
            _rows.Add(row);
            return row;
        }

        public int Length
        {
            get { return _rows.Count; }
        }

        #region IEnumerable<ExportRow> 成員

        public IEnumerator<ExportRow> GetEnumerator()
        {
            return _rows.GetEnumerator();
        }

        #endregion

        #region IEnumerable 成員

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _rows.GetEnumerator();
        }

        #endregion
    }
}

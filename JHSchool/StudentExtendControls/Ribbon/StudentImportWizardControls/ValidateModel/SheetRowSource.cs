using System;
using System.Collections.Generic;
using System.Text;
using DocValidate;
using System.Collections;
using JHSchool.StudentExtendControls.Ribbon.StudentImportWizardControls.SheetModel;

namespace JHSchool.StudentExtendControls.Ribbon.StudentImportWizardControls.ValidateModel
{
    public class SheetRowSource : IRowSource
    {
        private SheetReader _reader;
        private RowCellEnumerator _cell_enumerator;
        private ValidateColumnCollection _columns;

        public SheetRowSource(SheetReader reader, ValidateColumnCollection columns)
        {
            _reader = reader;
            _cell_enumerator = new RowCellEnumerator(_reader, columns);
            _columns = columns;

            foreach (ValidateColumn each in columns.Values)
            {
                if (!_reader.Columns.ContainsKey(each.Name))
                    throw new ArgumentException("來源資料中不包含此欄位。(" + each.Name + ")");
            }
        }

        public void Reset()
        {
            _reader.Reset();
        }

        public bool NextRow()
        {
            return _reader.MoveNext();
        }

        public RowMessage CreateRowMessage()
        {
            return new RowMessage(_reader.RelativelyIndex);
        }

        public void SetStringValue(string fieldName, string value)
        {
            _reader.GetCell(fieldName).PutValue(value);
        }

        #region IRowSource Members

        public bool ContainsField(string FieldName)
        {
            return _columns.ContainsKey(FieldName);
        }

        public string GetFieldData(string FieldName)
        {
            if (_columns.ContainsKey(FieldName))
                return _reader.GetValue(FieldName).Trim();
            else
                throw new ArgumentException("指定的欄位不存在，或不允許讀取。(" + FieldName + ")");
        }

        #endregion

        #region IEnumerable<string> Members

        public IEnumerator<string> GetEnumerator()
        {
            return _cell_enumerator;
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _cell_enumerator;
        }

        #endregion

        #region RowCellEnumerator
        private class RowCellEnumerator : IEnumerator<string>
        {
            private SheetReader _reader;
            private List<string> _columns;
            private int _current_index;

            public RowCellEnumerator(SheetReader reader, ValidateColumnCollection columns)
            {
                _reader = reader;
                _columns = new List<string>();
                _current_index = -1;

                foreach (ValidateColumn each in columns.Values)
                    _columns.Add(each.Name);
            }

            #region IEnumerator<string> Members

            public string Current
            {
                get { return _reader.GetValue(_columns[_current_index]); }
            }

            #endregion

            #region IDisposable Members

            public void Dispose()
            {
                _reader = null;
                _columns = null;
            }

            #endregion

            #region IEnumerator Members

            object IEnumerator.Current
            {
                get { return _reader.GetValue(_columns[_current_index]); }
            }

            public bool MoveNext()
            {
                if (_current_index >= (_columns.Count - 1))
                    return false;

                _current_index++;
                return true;
            }

            public void Reset()
            {
                _current_index = -1;
            }

            #endregion
        }
        #endregion
    }
}

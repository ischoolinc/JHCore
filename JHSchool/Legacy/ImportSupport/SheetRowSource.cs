using System;
using System.Collections.Generic;
using System.Text;
using DocValidate;

namespace JHSchool.Legacy.ImportSupport
{
    public class SheetRowSource : IRowSource
    {
        private SheetHelper _sheet;
        private WizardContext _context;
        private int _current_row_index;

        public SheetRowSource(SheetHelper sheet, WizardContext context)
        {
            _sheet = sheet;
            _context = context;
            _current_row_index = -1;
        }

        public SheetHelper Sheet { get { return _sheet; } }

        private WizardContext Context { get { return _context; } }

        public int CurrentRowIndex { get { return _current_row_index; } }

        public void BindRow(int index)
        {
            _current_row_index = index;
        }

        #region IRowSource 成員

        public bool ContainsField(string FieldName)
        {
            return Context.SelectedFields.Contains(FieldName);
        }

        public string GetFieldData(string FieldName)
        {
            return Sheet.GetValue(CurrentRowIndex, Sheet.GetFieldIndex(FieldName));
        }

        #endregion

        #region IEnumerable<string> 成員

        public IEnumerator<string> GetEnumerator()
        {
            return Context.SelectedFields.GetEnumerator();
        }

        #endregion

        #region IEnumerable 成員

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return Context.SelectedFields.GetEnumerator();
        }

        #endregion
    }
}

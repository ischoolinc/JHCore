using System;
using System.Collections.Generic;
using System.Text;

namespace JHSchool.StudentExtendControls.Ribbon.StudentImportWizardControls.ValidateModel
{
    public class RowMessage
    {
        private int _row_index;
        private Dictionary<string, CellMessage> _messages;

        public RowMessage(int rowIndex)
        {
            _row_index = rowIndex;
            _messages = new Dictionary<string, CellMessage>();
        }

        public int RowIndex
        {
            get { return _row_index; }
        }

        public bool HasMessage
        {
            get { return _messages.Count > 0; }
        }

        public void ReportMessage(string column, MessageType infoType, string message)
        {
            CellMessage result;

            if (_messages.ContainsKey(column))
                result = _messages[column];
            else
            {
                result = new CellMessage(column);
                _messages.Add(column, result);
            }

            result.ReportMessage(infoType, message);
        }

        public List<CellMessage> GetMessages()
        {
            return new List<CellMessage>(_messages.Values);
        }
    }
}

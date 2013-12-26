using System;
using System.Collections.Generic;
using System.Text;

namespace JHSchool.StudentExtendControls.Ribbon.StudentImportWizardControls.ValidateModel
{
    public class CellMessage
    {
        private string _column;
        private StringBuilder _infos;
        private MessageType _msg_type;

        public CellMessage(string column)
        {
            _column = column;
            _infos = new StringBuilder();
            _msg_type = MessageType.Unknow;
        }

        public string Column
        {
            get { return _column; }
        }

        public void ReportMessage(MessageType msgType, string message)
        {
            _msg_type = msgType;

            switch (msgType)
            {
                case MessageType.Correct:
                    _infos.AppendLine("­×¥¿\n" + message);
                    break;
                case MessageType.Warning:
                    _infos.AppendLine("´£¥Ü\n" + message);
                    break;
                case MessageType.Error:
                    _infos.AppendLine("¿ù»~\n" + message);
                    break;
            }
        }

        public string Message
        {
            get { return _infos.ToString(); }
        }

        public MessageType MessageType
        {
            get { return _msg_type; }
        }
    }
}

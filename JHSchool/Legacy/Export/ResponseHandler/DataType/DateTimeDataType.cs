using System;
using System.Collections.Generic;
using System.Text;

namespace JHSchool.Legacy.Export.ResponseHandler.DataType
{
    public class DateTimeDataType : IDataType
    {
        private string _value;
        private DateTime _resultValue;

        public void SetValue(string value)
        {
            _value = value;
        }

        public bool IsValidDataType
        {
            get
            {
                return DateTime.TryParse(_value, out _resultValue);
            }
        }

        public object GetTypeValue()
        {
            return _resultValue;
        }
    }
}

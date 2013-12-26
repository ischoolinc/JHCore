using System;
using System.Collections.Generic;
using System.Text;

namespace JHSchool.Legacy.Export.ResponseHandler.DataType
{
    public class BooleanDataType : IDataType
    {
        #region IDataType<bool> жин√

        private string _value;
        private bool _resultValue;

        public void SetValue(string value)
        {
            _value = value;
        }

        public bool IsValidDataType
        {
            get
            {
                return bool.TryParse(_value, out _resultValue);
            }
        }

        public object GetTypeValue()
        {
            return _resultValue;
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace JHSchool.Legacy.Export.ResponseHandler.DataType
{
    public class IntegerDataType:IDataType
    {
        #region IDataType<int> жин√
        private string _value;
        private int _returnValue;

        public void SetValue(string value)
        {
            _value = value;
        }

        public bool IsValidDataType
        {
            get { return int.TryParse(_value, out _returnValue); }
        }

        public object GetTypeValue()
        {
            return _returnValue;
        }

        #endregion
    }
}

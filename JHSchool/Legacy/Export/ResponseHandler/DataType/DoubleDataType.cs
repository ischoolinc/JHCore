using System;
using System.Collections.Generic;
using System.Text;

namespace JHSchool.Legacy.Export.ResponseHandler.DataType
{
    public class DoubleDataType : IDataType
    {
        #region IDataType<double> жин√
        private string _value;
        private double _returnValue;

        public void SetValue(string value)
        {
            _value = value;
        }

        public bool IsValidDataType
        {
            get { return double.TryParse(_value, out _returnValue); }
        }
        
        public object GetTypeValue()
        {
            return _returnValue;
        }

        #endregion
    }
}

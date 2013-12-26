using System;
using System.Collections.Generic;
using System.Text;

namespace JHSchool.Legacy.Export.ResponseHandler.DataType
{
    public class StringDataType:IDataType
    {

        #region IDataType<string> жин√

        private string _value;        

        public void SetValue(string value)
        {
            _value = value;
        }

        public bool IsValidDataType
        {
            get
            {
                return true;
            }
        }
        
        public object GetTypeValue()
        {
            return _value;
        }

        #endregion
    }
}

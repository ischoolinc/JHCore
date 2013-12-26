using System;
using System.Collections.Generic;
using System.Text;

namespace JHSchool.Legacy.Export.ResponseHandler.DataType
{
    public interface IDataType
    {
        void SetValue(string value);
        bool IsValidDataType { get;}  
        object GetTypeValue();
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace JHSchool.Legacy.Export.ResponseHandler.DataType
{
    public class DataTypeFactory
    {
        private static Dictionary<string, IDataType> _dataTypes;

        public static IDataType GetInstance(string typeName)
        {
            typeName = typeName.ToLower();
            if (_dataTypes != null)
            {
                if(_dataTypes.ContainsKey(typeName))
                    return _dataTypes[typeName];
            }

            _dataTypes = new Dictionary<string, IDataType>();
            _dataTypes.Add("integer", new IntegerDataType());
            _dataTypes.Add("boolean", new BooleanDataType());
            _dataTypes.Add("double", new DoubleDataType());
            _dataTypes.Add("datetime", new DateTimeDataType());
            _dataTypes.Add("object", new ObjectDataType());
            _dataTypes.Add("string", new StringDataType());
            _dataTypes.Add("", new StringDataType());
            return _dataTypes[typeName];
        }        
    }
}

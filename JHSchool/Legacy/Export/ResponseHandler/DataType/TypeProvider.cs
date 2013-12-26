using System;
using System.Collections.Generic;
using System.Text;

namespace JHSchool.Legacy.Export.ResponseHandler.DataType
{
    public class TypeProvider
    {
        public static Type GetType(string typeName)
        {
            switch (typeName.ToUpper())
            {
                case "INTEGER":
                    return typeof(int);
                case "STRING":
                    return typeof(string);
                case "DOUBLE":
                    return typeof(double);
                case "DATETIME":
                    return typeof(DateTime);
                case "BOOLEAN":
                    return typeof(bool);
                default:
                    return typeof(object);
            }
        }
    }
}

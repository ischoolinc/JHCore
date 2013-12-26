using System;
using System.Collections.Generic;
using System.Text;
using JHSchool.Legacy.Export.RequestHandler.Generator;

namespace JHSchool.Legacy.Export.ResponseHandler.Formater
{
    public class ResponseFormaterFactory
    {
        public static IResponseFormater CreateInstance(ExportType type)
        {
            switch (type)
            {
                case ExportType.ExportStudent:
                    return new ResponseFormater();
                default:
                    return new ResponseFormater();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using JHSchool.Legacy.Export.RequestHandler.Generator;

namespace JHSchool.Legacy.Export.RequestHandler.Formater
{
    public class FieldFormaterFactory
    {
        public static IFieldFormater CreateInstance(ExportType type)
        {
            switch (type)
            {
                case ExportType.ExportStudent:
                    return new BaseFieldFormater();
                default:
                    return new BaseFieldFormater();
            }
        }
    }
}

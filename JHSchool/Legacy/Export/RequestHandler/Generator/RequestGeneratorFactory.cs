using System;
using System.Collections.Generic;
using System.Text;

namespace JHSchool.Legacy.Export.RequestHandler.Generator
{
    public class RequestGeneratorFactory
    {
        public static IRequestGenerator CreateInstance(ExportType style)
        {            
            switch (style)
            {
                case ExportType.ExportStudent:
                    return new ExportStudentRequestGenerator();
                default:
                    return new ExportStudentRequestGenerator();
            }
        }
    }
}

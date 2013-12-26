using System;
using System.Collections.Generic;
using System.Text;
using JHSchool.Legacy.Export.RequestHandler;
using System.Xml;

namespace JHSchool.Legacy.Export.ResponseHandler.Formater
{
    public interface IResponseFormater
    {
        ExportFieldCollection Format(XmlElement element);
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace JHSchool.Legacy.Export.RequestHandler.Generator.Condition
{
    public interface ICondition
    {        
        XmlElement GetConditionElement();
    }
}

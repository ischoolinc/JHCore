using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace JHSchool.Legacy.Export.RequestHandler.Generator.Condition
{
    public class BaseCondition : ICondition
    {
        private XmlElement _conditionElement;
        private string _elementName;
        private string _elementValue;

        public BaseCondition(string elementName, string elementValue)
        {
            XmlDocument doc = new XmlDocument();
            XmlElement element = doc.CreateElement(elementName);
            element.InnerText = elementValue;
            _conditionElement = (XmlElement)element.Clone();
        }

        #region ICondition жин√

        public XmlElement GetConditionElement()
        {
            return _conditionElement;
        }

        #endregion
    }   
}

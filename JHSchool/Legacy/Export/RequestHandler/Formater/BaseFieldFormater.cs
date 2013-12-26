using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace JHSchool.Legacy.Export.RequestHandler.Formater
{
    public class BaseFieldFormater:IFieldFormater
    {
        #region IFieldFormater жин√

        public FieldCollection Format(XmlElement source)
        {
            FieldCollection _collection = new FieldCollection();
            foreach (XmlNode node in source.ChildNodes)
            {
                if (node.NodeType != XmlNodeType.Element) continue;
                if (node.Name != "Field" && node.Name != "XmlField") continue;
                XmlElement element = (XmlElement)node;
                string name = element.GetAttribute("Name");
                string displaytext = element.GetAttribute("DisplayText");

                Field field = new Field();
                field.FieldName = name;
                field.DisplayText = displaytext;
                _collection.Add(field);
            }
            return _collection;
        }

        #endregion        
    }
}

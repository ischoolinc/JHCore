using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace JHSchool
{
    public class StudentTagRecord : GeneralTagRecord
    {
        protected override string GetEntityID(XmlElement data)
        {
            return data.SelectSingleNode("StudentID").InnerText;
        }

        public StudentRecord Student { get { return JHSchool.Student.Instance[RefEntityID]; } }
    }
}

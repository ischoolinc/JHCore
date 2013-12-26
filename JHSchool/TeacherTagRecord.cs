using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JHSchool
{
    public class TeacherTagRecord : GeneralTagRecord
    {
        protected override string GetEntityID(System.Xml.XmlElement data)
        {
            return data.SelectSingleNode("TeacherID").InnerText;
        }

        public TeacherRecord Teacher{ get { return JHSchool.Teacher.Instance[RefEntityID]; } }
    }
}

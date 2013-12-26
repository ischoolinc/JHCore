using System;
using System.Collections.Generic;
using System.Text;
using FISCA.DSAUtil;
using System.Xml;

namespace JHSchool
{
    public class DepartmentRecord
    {
        public string Name { get; private set; }
        public string FullName { get; private set; }
        public string SubDepartment { get; private set; }
        public string ID { get; private set; }
        public string Code { get; private set; }
        internal DepartmentRecord(XmlElement element)
        {
            ID = element.GetAttribute("ID");
            DSXmlHelper helper = new DSXmlHelper(element);
            var name = helper.GetText("Name").Replace("：", ":");
            var hasSubDepartment = name.Split(":".ToCharArray()).Length > 1;
            if ( hasSubDepartment )
            {
                Name = name.Split(":".ToCharArray())[0];
                SubDepartment = name.Substring(name.IndexOf(":"));
                FullName = Name + ":" + SubDepartment;
            }
            else
            {
                Name = name;
                FullName = name;
                SubDepartment = "";
            }

            Code = helper.GetText("Code");
        }

    }
}

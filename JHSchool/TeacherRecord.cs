using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using FISCA.DSAUtil;

namespace JHSchool
{
    public class TeacherRecord : IComparable<TeacherRecord>
    {
        public string ID { get; private set; }
        public string Name { get; private set; }
        public string Nickname { get; private set; }
        public string Status { get; private set; }
        public string Gender { get; private set; }
        public string IDNumber { get; private set; }
        //public string UniqName { get; private set; }
        public string ContactPhone { get; private set; }
        public string TALoginName { get; private set; }
        public string Category { get; private set; }
        //public string SuperviseByClassID { get; private set; }
        /// <summary>
        /// 完整名稱，暱稱+()
        /// </summary>
        public string FullName
        {
            get
            {
                if (string.IsNullOrEmpty(Nickname))
                    return Name;
                else
                    return Name + "(" + Nickname + ")";
            }
        }

        internal TeacherRecord(XmlElement element)
        {
            ID = element.GetAttribute("ID");
            
            foreach (XmlNode node in element.ChildNodes)
            {
                if (node is XmlElement)
                {
                    string text = node.InnerText;
                    switch (node.Name)
                    {
                        case "TeacherName": Name = text; break;
                        case "Nickname": Nickname = text; break;
                        case "Status": Status = text; break;
                        case "Gender": Gender = text; break;
                        case "IDNumber": IDNumber = text; break;
                        case "ContactPhone": ContactPhone = text; break;
                        case "Category": Category = text; break;
                        case "TALoginName": TALoginName = text; break;
                        //case "SupervisedByClassID": SuperviseByClassID = text; break;
                    }
                }
            }
        }

        #region IComparable<TeacherRecord> 成員

        public static event EventHandler<CompareTeacherRecordEventArgs> CompareTeacherRecord;

        public int CompareTo(TeacherRecord other)
        {
            if ( CompareTeacherRecord != null )
            {
                CompareTeacherRecordEventArgs args = new CompareTeacherRecordEventArgs(this, other);
                CompareTeacherRecord(null, args);
                return args.Result;
            }
            else
            {
                if ( this.ID.Length == other.ID.Length )
                    return this.ID.CompareTo(other.ID);
                else
                    return this.ID.Length.CompareTo(other.ID.Length);
            }
        }

        #endregion
    }

    public class CompareTeacherRecordEventArgs : EventArgs
    {
        internal CompareTeacherRecordEventArgs(TeacherRecord v1, TeacherRecord v2)
        {
            Value1 = v1;
            Value2 = v2;
        }
        public TeacherRecord Value1 { get; private set; }
        public TeacherRecord Value2 { get; private set; }
        public int Result { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using FISCA.DSAUtil;

namespace JHSchool
{
    public class CourseRecord : IComparable<CourseRecord>
    {
        public string ID { get; private set; }
        public string Name { get; private set; }
        public int SchoolYear { get; private set; }
        public int Semester { get; private set; }
        public string Subject { get; private set; }
        public string Domain { get; private set; }
        public string Period { get; private set; }
        public string Credit { get; private set; }
        public string RefClassID { get; private set; }
        public string RefAssessmentSetupID { get; private set; }
        /// <summary>
        /// 1:列入學期成績，2:不列入學期成績。
        /// </summary>
        public string CalculationFlag { get; private set; }
        //public string Tags { get; private set; }

        public ClassRecord Class { get { return JHSchool.Class.Instance[RefClassID]; } }

        internal CourseRecord(XmlElement element)
        {
            ID = element.GetAttribute("ID");
            
            foreach (XmlNode node in element.ChildNodes)
            {
                if (node is XmlElement)
                {
                    string text = node.InnerText;
                    switch (node.Name)
                    {
                        case "CourseName": Name = text; break;
                        case "SchoolYear": 
                            int sy = 0;
                            int.TryParse(text, out sy);
                            SchoolYear = sy;
                            break;
                        case "Semester": 
                            int sem = 0;
                            int.TryParse(text, out sem);
                            Semester = sem;
                            break;
                        case "Subject": Subject = text; break;
                        case "Domain": Domain = text; break;
                        case "Period": Period = text; break;
                        case "Credit": Credit = text; break;
                        case "RefClassID": RefClassID = text; break;
                        case "RefExamTemplateID": RefAssessmentSetupID = text; break;
                        case "ScoreCalcFlag": CalculationFlag = text; break;
                    }
                }
            }
        }
        #region IComparable<CourseRecord> 成員

        public static event EventHandler<CompareCourseRecordEventArgs> CompareCourseRecord;

        public int CompareTo(CourseRecord other)
        {
            if (CompareCourseRecord != null)
            {
                CompareCourseRecordEventArgs args = new CompareCourseRecordEventArgs(this, other);
                CompareCourseRecord(null, args);
                return args.Result;
            }
            else
            {
                if (this.SchoolYear == other.SchoolYear)
                {
                    if (this.Semester == other.Semester)
                    {
                        return Framework.StringComparer.Comparer(this.Name, other.Name);
                    }
                    else
                        return this.Semester.CompareTo(other.Semester);
                }
                else return this.SchoolYear.CompareTo(other.SchoolYear);
            }
        }

        #endregion

        public object GetGetInstructTeachers()
        {
            throw new NotImplementedException();
        }
    }
    public class CompareCourseRecordEventArgs : EventArgs
    {
        internal CompareCourseRecordEventArgs(CourseRecord v1, CourseRecord v2)
        {
            Value1 = v1;
            Value2 = v2;
        }
        public CourseRecord Value1 { get; private set; }
        public CourseRecord Value2 { get; private set; }
        public int Result { get; set; }
    }
}

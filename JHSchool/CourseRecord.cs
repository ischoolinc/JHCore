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
            DSXmlHelper helper = new DSXmlHelper(element);
            //RequiredBy = helper.GetText("RequiredBy"); //高中
            //Required = helper.GetText("IsRequired") == "必"; //高中
            Name = helper.GetText("CourseName");
            int i = 0;
            int.TryParse(helper.GetText("SchoolYear"), out i);
            SchoolYear = i;
            i = 0;
            int.TryParse(helper.GetText("Semester"), out i);
            Semester = i;
            Subject = helper.GetText("Subject");
            Domain = helper.GetText("Domain");
            Period = helper.GetText("Period");
            Credit = helper.GetText("Credit");
            RefClassID = helper.GetText("RefClassID");
            RefAssessmentSetupID = helper.GetText("RefExamTemplateID");
            CalculationFlag = helper.GetText("ScoreCalcFlag");
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

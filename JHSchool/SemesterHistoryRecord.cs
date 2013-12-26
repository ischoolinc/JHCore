using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace JHSchool
{
    public class SemesterHistoryRecord
    {
        internal string RefStudentID { get; private set; }
        public int SchoolYear { get; private set; }
        public int Semester { get; private set; }
        public int GradeYear { get; private set; }
        public string ClassName { get; private set; }
        public string SeatNo { get; private set; }
        public string Teacher { get; private set; }
        public string  SchoolDayCount { get; private set; }
        public StudentRecord Student
        {
            get { return JHSchool.Student.Instance[RefStudentID]; }
            //set 
            //{
            //    if ( RefStudentID != "" && RefStudentID != value.ID )
            //    {
            //        throw new Exception("不允許變更關聯之學生。");
            //    }
            //    else
            //    {
            //        RefStudentID = value.ID;
            //    }
            //}
        }
        internal SemesterHistoryRecord(string refStudentID,XmlElement element)
        {
            RefStudentID = refStudentID;
            int i = 0;
            SchoolYear=int.TryParse(element.GetAttribute("SchoolYear"), out i)?i:0;
            Semester=int.TryParse(element.GetAttribute("Semester"), out i)?i:0;
            GradeYear = int.TryParse(element.GetAttribute("GradeYear"), out i) ? i : 0;
            ClassName = element.GetAttribute("ClassName");
            SeatNo = element.GetAttribute("SeatNo");
            Teacher = element.GetAttribute("Teacher");
            SchoolDayCount = element.GetAttribute("SchoolDayCount");
        }
    }
}

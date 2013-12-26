using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JHSchool
{

    /// <summary>
    /// 由 Class 類別所提供的 Extend Method
    /// </summary>
    public static class Class_ExtendMethod
    {

        /// <summary>
        /// 根據班級名稱取得班級物件。
        /// </summary>
        public static ClassRecord GetClassByName(this Class classentity,string classname)
        {
            foreach (ClassRecord cr in Class.Instance.Items)
                if (cr.Name.Equals(classname))
                    return cr;
            return null;
        }


        /// <summary>
        /// 取得班級空座號。
        /// </summary>
        public static List<string> GetUnUsedSeatNo(this ClassRecord classrecord)
        {
            List<int> UsedSeatNo = new List<int>();
            List<string> UnUsedSeatNo = new List<string>();
            int SeatNo;

            if (classrecord == null)
                return UnUsedSeatNo;

            foreach (StudentRecord studRec in classrecord.Students)
            {
                int.TryParse(studRec.SeatNo, out SeatNo);
                UsedSeatNo.Add(SeatNo);
            }

            UsedSeatNo.Sort();

            for (int i = 1; i <= classrecord.Students.Count; i++)
                if (!UsedSeatNo.Contains(i))
                    UnUsedSeatNo.Add(i.ToString());

            if (UsedSeatNo.Count > 0)
                UnUsedSeatNo.Add((UsedSeatNo[UsedSeatNo.Count - 1] + 1).ToString());

            return UnUsedSeatNo;
        }

        /// <summary>
        /// 根據年級名稱取得班級列表。
        /// </summary>
         public static List<ClassRecord> GetClassesByGradeYear(this Class classentity,string vGradeYear)
         {
             List<ClassRecord> classes = new List<ClassRecord>();

             foreach (ClassRecord classrecord in Class.Instance.Items)
                 if (classrecord.GradeYear.Equals(vGradeYear))
                     classes.Add(classrecord);

             return classes;
         }

        /// <summary>
        /// 根據年級名稱取得班級列表。
        /// </summary>
         public static List<string> GetClassName(this IEnumerable<ClassRecord> classes)
         {
             List<string> ClassName=new List<string>();

             foreach (ClassRecord classrecord in classes)
                 ClassName.Add(classrecord.Name);
             return ClassName;
         }

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JHSchool
{
    public struct SemesterInfo:IComparable<SemesterInfo>
    {
        public int SchoolYear { get; set; }
        public int Semester { get; set; }
        public static bool operator ==(SemesterInfo c1, SemesterInfo c2)
        {
            return c1.SchoolYear == c2.SchoolYear && c1.Semester == c2.Semester;
        }
        public static bool operator !=(SemesterInfo c1, SemesterInfo c2)
        {
            return c1.SchoolYear != c2.SchoolYear || c1.Semester != c2.Semester;
        }
        public static bool operator <(SemesterInfo c1, SemesterInfo c2)
        {
            if ( c1.SchoolYear == c2.SchoolYear ) 
                return c1.Semester < c2.Semester;
            else 
                return c1.SchoolYear < c2.SchoolYear;
        }
        public static bool operator >(SemesterInfo c1, SemesterInfo c2)
        {
            if ( c1.SchoolYear == c2.SchoolYear )
                return c1.Semester > c2.Semester;
            else
                return c1.SchoolYear > c2.SchoolYear;
        }
        public static bool operator <=(SemesterInfo c1, SemesterInfo c2)
        {
            if ( c1.SchoolYear == c2.SchoolYear )
                return c1.Semester <= c2.Semester;
            else
                return c1.SchoolYear < c2.SchoolYear;
        }
        public static bool operator >=(SemesterInfo c1, SemesterInfo c2)
        {
            if ( c1.SchoolYear == c2.SchoolYear )
                return c1.Semester >=c2.Semester;
            else
                return c1.SchoolYear > c2.SchoolYear;
        }

        public override bool Equals(object obj)
        {
            if ( obj is SemesterInfo )
                return this == (SemesterInfo)obj;
            else
                return false;
        }
        public override int GetHashCode()
        {
            return ( SchoolYear << 5 ) & Semester;
        }
        public override string ToString()
        {
            return "" + SchoolYear + "學年度 第"+Semester+"學期";
        }

        #region IComparable<SemesterInfo> 成員

        public int CompareTo(SemesterInfo other)
        {
            return this.SchoolYear == other.SchoolYear ? this.Semester.CompareTo(other.Semester) : this.SchoolYear.CompareTo(other.SchoolYear);
        }

        #endregion
    }
}

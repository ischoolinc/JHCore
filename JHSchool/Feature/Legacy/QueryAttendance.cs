using System;
using System.Collections.Generic;
using System.Text;
using FISCA.DSAUtil;
using Framework.Security;
using Framework;
using FISCA.Authentication;

namespace JHSchool.Feature.Legacy
{
    //[FeatureCode("F009")]
    [AutoRetryOnWebException()]
    public class QueryAttendance
    {
        public static DSResponse GetAttendance(DSRequest request)
        {
            return DSAServices.CallService("SmartSchool.Student.Attendance.GetAttendance", request);
        }

        public static DSResponse GetAttendanceStatistic(DSRequest request)
        {
            return DSAServices.CallService("SmartSchool.Student.Attendance.GetAbsenceStatistic", request);
        }

        public static DSResponse GetNoAbsenceStatistic(DSRequest request)
        {
            return DSAServices.CallService("SmartSchool.Student.Attendance.GetNoAbsenceStatistic", request);
        }
    }
}

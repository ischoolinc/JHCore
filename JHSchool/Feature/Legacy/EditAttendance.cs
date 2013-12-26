using System;
using System.Collections.Generic;
using System.Text;
using FISCA.DSAUtil;
using Framework;
using FISCA.Authentication;

namespace JHSchool.Feature.Legacy
{
    public class EditAttendance
    {
        [AutoRetryOnWebException()]
        public static void Delete(DSRequest dSRequest)
        {
            DSAServices.CallService("SmartSchool.Student.Attendance.Delete", dSRequest);
        }

        [AutoRetryOnWebException()]
        public static void Update(DSRequest dSRequest)
        {
            DSAServices.CallService("SmartSchool.Student.Attendance.Update", dSRequest);
        }

        public static void Insert(DSRequest dSRequest)
        {
            DSAServices.CallService("SmartSchool.Student.Attendance.Insert", dSRequest);
        }
    }
}

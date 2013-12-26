using System;
using System.Collections.Generic;
using System.Text;
using Framework;
using FISCA.DSAUtil;
using FISCA.Authentication;

namespace JHSchool.Feature.Legacy
{
    [AutoRetryOnWebException()]
    public class EditDiscipline
    {
        public static void Delete(DSRequest dSRequest)
        {
            DSAServices.CallService("SmartSchool.Student.Discipline.Delete", dSRequest);
        }

        public static void Update(DSRequest dSRequest)
        {
            DSAServices.CallService("SmartSchool.Student.Discipline.Update", dSRequest);
        }

        public static void Insert(DSRequest dSRequest)
        {
            DSAServices.CallService("SmartSchool.Student.Discipline.Insert", dSRequest);
        }
    }
}

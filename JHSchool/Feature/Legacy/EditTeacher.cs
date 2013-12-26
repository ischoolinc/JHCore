using System;
using System.Collections.Generic;
using System.Text;
using FISCA.DSAUtil;
using Framework;
using FISCA.Authentication;

namespace JHSchool.Feature.Legacy
{
    [AutoRetryOnWebException()]
    public class EditTeacher
    {
        public static DSResponse Update(DSRequest dsreq)
        {
            return DSAServices.CallService("SmartSchool.Teacher.Update", dsreq);
        }
    }
}

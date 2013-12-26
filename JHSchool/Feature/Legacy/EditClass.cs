using System;
using System.Collections.Generic;
using System.Text;
using FISCA.DSAUtil;
using FISCA.Authentication;

namespace JHSchool.Feature.Legacy
{
    [AutoRetryOnWebException()]
    class EditClass
    {
        public static void Update(DSRequest request)
        {
            DSAServices.CallService("SmartSchool.Class.Update", request);
        }
    }
}

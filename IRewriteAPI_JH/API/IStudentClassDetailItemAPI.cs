using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IRewriteAPI_JH
{
    /// <summary>
    /// 國中-學生>班級資訊
    /// </summary>
    public interface IStudentClassDetailItemAPI
    {
        FISCA.Presentation.IDetailBulider CreateBasicInfo();
    }
}

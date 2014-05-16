using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IRewriteAPI_JH
{
    /// <summary>
    /// 國中系統 - 學生基本資料項目
    /// </summary>
    public interface IStudentDetailItemAPI
    {
        FISCA.Presentation.IDetailBulider CreateBasicInfo();
    }
}

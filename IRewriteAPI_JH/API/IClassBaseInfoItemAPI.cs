using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IRewriteAPI_JH
{
    /// <summary>
    /// 國中-班級 基本資訊
    /// </summary>
    public interface IClassBaseInfoItemAPI
    {
        FISCA.Presentation.IDetailBulider CreateBasicInfo();
    }
}

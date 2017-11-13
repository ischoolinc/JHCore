using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IRewriteAPI_JH
{

    //// 2017/11/9 穎驊註解，因應高雄小組項目 [06-01][03] 修改學生狀態沒有上傳局端 而新增的Code

    /// <summary>
    /// 國中系統 - 學生-刪除學生
    /// </summary>
    public interface IStudentDeleteStudentAPI
    {
        FISCA.Presentation.Controls.BaseForm CreateForm();
    }
}

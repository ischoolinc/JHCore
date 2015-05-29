using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IRewriteAPI_JH
{
    /// <summary>
    /// 匯出學生基本資料
    /// </summary>
    public interface IStudentExportWizardAPI
    {
        FISCA.Presentation.Controls.BaseForm CreateForm();
    }
}

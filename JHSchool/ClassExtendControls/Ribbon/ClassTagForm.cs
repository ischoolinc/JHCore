using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using JHSchool.InternalExtendControls.Tagging;
//using Framework;
//using JHSchool.Editor;
using JHSchool.Data;

namespace JHSchool.ClassExtendControls.Ribbon
{
    internal partial class ClassTagForm : TagForm
    {
        public ClassTagForm()
        {
            InitializeComponent();
        }

        protected override K12.Data.TagCategory Category
        {
            get
            {
                return K12.Data.TagCategory.Class;
            }
        }

        protected override void DoDelete(JHTagConfigRecord  record)
        {
            int use_count = 0;

            foreach (JHClassTagRecord rec in JHClassTag.SelectAll())
            {
                if (rec.RefTagID == record.ID)
                    use_count++;
            }


            string msg;
            if (use_count > 0)
                msg = string.Format("目前有「{0}」個班級使用此類別，您確定要刪除此類別嗎？", use_count);
            else
                msg = "您確定要刪除此類別嗎？";

            if (FISCA.Presentation.Controls.MsgBox.Show(msg, MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                PermRecLogProcess prlp = new PermRecLogProcess();
                prlp.SaveLog("學籍.類別管理", "類別管理刪除類別", "刪除 " + record.Category  + " 類別,名稱:" + record.FullName);
                JHTagConfig.Delete(record);
            }
        }
    }
}

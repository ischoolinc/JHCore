using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using DevComponents.DotNetBar;
using FISCA.Presentation.Controls;
using JHSchool.Legacy.Export.RequestHandler;
using JHSchool.Legacy.Export.RequestHandler.Formater;
using Framework;
using JHSchool.Legacy.Export.ResponseHandler.Connector;
using JHSchool.Legacy.Export.ResponseHandler;
using JHSchool.Legacy.Export.ResponseHandler.Output;

namespace JHSchool.TeacherExtendControls.Ribbon
{
    public partial class TeacherExportWizard : BaseForm
    {
        public TeacherExportWizard()
        {
            InitializeComponent();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ExportTeacher_Load(object sender, EventArgs e)
        {
            XmlElement element = SmartSchool.Feature.Teacher.TeacherBulkProcess.GetExportDescription();
            BaseFieldFormater formater = new BaseFieldFormater();
            FieldCollection collection = formater.Format(element);

            List<string> list = new List<string>(new string[] { "教師系統編號", "教師姓名", "暱稱" });

            //需遮蔽的欄位
            List<string> avoids = new List<string>(new string[] { "帳號類型" });

            foreach (Field field in collection)
            {
                //遮蔽欄位
                if (avoids.Contains(field.DisplayText)) continue;

                ListViewItem item = listView.Items.Add(field.DisplayText);
                if (list.Contains(field.DisplayText))
                {
                    item.ForeColor = Color.Red;
                }
                item.Tag = field;
                item.Checked = true;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (GetSelectedFields().Count == 0)
            {
                FISCA.Presentation.Controls.MsgBox.Show("必須至少選擇一項匯出欄位!", "欄位空白", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            saveFileDialog1.Filter = "Excel (*.xls)|*.xls|所有檔案 (*.*)|*.*";
            saveFileDialog1.FileName = "匯出教師基本資料";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                IExportConnector ec = new ExportTeacherConnector();
                foreach (TeacherRecord teacher in Teacher.Instance.SelectedList)
                {
                    ec.AddCondition(teacher.ID);
                }
                ec.SetSelectedFields(GetSelectedFields());
                ExportTable table = ec.Export();

                ExportOutput output = new ExportOutput();
                output.SetSource(table);
                output.Save(saveFileDialog1.FileName);

                if (FISCA.Presentation.Controls.MsgBox.Show("檔案存檔完成，是否開啟該檔案", "是否開啟", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    try
                    {
                        Process.Start(saveFileDialog1.FileName);
                    }
                    catch (Exception ex)
                    {
                        FISCA.Presentation.Controls.MsgBox.Show("開啟檔案發生失敗:" + ex.Message, "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                this.Close();
            }
        }

        private FieldCollection GetSelectedFields()
        {
            FieldCollection collection = new FieldCollection();
            foreach (ListViewItem item in listView.CheckedItems)
            {
                Field field = item.Tag as Field;
                collection.Add(field);
            }
            return collection;
        }

        private void saveFileDialog2_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void chkSelect_CheckedChanged(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listView.Items)
            {
                item.Checked = chkSelect.Checked;
            }
        }
    }
}
using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.Xml;
using FISCA.Presentation.Controls;
using JHSchool.Legacy.Export.RequestHandler;
using Framework;
using JHSchool.Legacy.Export.RequestHandler.Formater;
using JHSchool.Legacy.Export.ResponseHandler.Output;
using JHSchool.Legacy.Export.ResponseHandler.Connector;
using JHSchool.Legacy.Export.ResponseHandler;
using System.Drawing;
using System.Collections.Generic;
using IRewriteAPI_JH;

namespace JHSchool.ClassExtendControls.Ribbon
{
    public partial class ClassExportWizard : BaseForm
    {
        public ClassExportWizard()
        {
            InitializeComponent();
        }

        //關閉畫面
        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        private void ExportClass_Load(object sender, EventArgs e)
        {
            //XmlElement element = SmartSchool.Feature.Class.ClassBulkProcess.GetExportDescription();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(Properties.Resources.JH_C_ExportDescription);
            XmlElement element = doc.DocumentElement;
            #region XML內容
            /*	<FieldDescription>
					<Field DisplayText="班級系統編號" Name="ID" ReadOnly="True" />
					<Field DisplayText="班級名稱" Name="ClassName" ShiftCheckable="True" />
					<Field DisplayText="班導師" Name="TeacherName" ShiftCheckable="True" />
					<Field DisplayText="年級" Name="GradeYear" ShiftCheckable="True" />
					<Field DisplayText="科別" Name="DepartmentName" ShiftCheckable="True" />
					<Field DisplayText="課程規劃" Name="GraduationPlan" ShiftCheckable="True" />
					<Field DisplayText="計算規則" Name="CalculationRule" ShiftCheckable="True" />
					<Field DisplayText="排列序號" Name="DisplayOrder" ShiftCheckable="True" />
					<Field DisplayText="班級名稱規則" Name="NamingRule" ShiftCheckable="True" />
				</FieldDescription>
             */



            #endregion

            BaseFieldFormater formater = new BaseFieldFormater();
            //將資料格式化,並組成集合
            FieldCollection collection = formater.Format(element);

            // 2018/12/21 穎驊 因應高雄項目 [10-03][??] 局端夠查詢學校班級有調整”導師”的功能
            // 讓匯出的說明，班導師也納入 必填欄位
            // 但蠻無言的， 原來的必填欄位居然只是這樣填寫List...
            // 有載入高雄自動編班模組的 ， 其匯入規則 載Local 的設定(班導師必填)
            // 其餘的學校 依然為舊做法，自Service 載匯入規則資料

            List<string> list;

            IClassBaseInfoItemAPI ClassBaseInfoItem = FISCA.InteractionService.DiscoverAPI<IClassBaseInfoItemAPI>();
            if (ClassBaseInfoItem != null)
            {
                list = new List<string>(new string[] { "班級系統編號", "班級名稱","班導師" });
            }
            else
            {
                list = new List<string>(new string[] { "班級系統編號", "班級名稱" });
            }
            
            //將集合內容,逐一填入使用者勾選清單中(Tag放置一份field),預設為(true)
            foreach (Field field in collection)
            {
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
            //判斷勾選個數
            if (GetSelectedFields().Count == 0)
            {
                FISCA.Presentation.Controls.MsgBox.Show("必須至少選擇一項匯出欄位!", "欄位空白", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2017/8/22 穎驊依據高雄小組專案 [03-05][04+] EXCEL匯入格式可否修正為xlsx也可匯入？ 更改為新版 Aspose.Cells_201402 寫法，支援.xlsx 匯出
            saveFileDialog1.Filter = "Excel (*.xlsx)|*.xlsx|Excel (*.xls)|*.xls|所有檔案 (*.*)|*.*";
            saveFileDialog1.FileName = "匯出班級基本資料";

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                //??
                IExportConnector ec = new ExportClassConnector();
                foreach (ClassRecord info in Class.Instance.SelectedList)
                {
                    ec.AddCondition(info.ID);
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
            //把勾選項目的集合
            foreach (ListViewItem item in listView.CheckedItems)
            {
                Field field = item.Tag as Field;
                collection.Add(field);
            }
            return collection;
        }

        //全部選取
        private void chkSelect_CheckedChanged(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listView.Items)
            {
                item.Checked = chkSelect.Checked;
            }
        }
    }
}
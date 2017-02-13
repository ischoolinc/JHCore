using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevComponents.DotNetBar;
using JHSchool.Legacy.Export.ResponseHandler.Connector;
using System.Xml;
using JHSchool.Legacy.Export.RequestHandler.Formater;
using JHSchool.Legacy.Export.RequestHandler;
using JHSchool.Legacy.Export.ResponseHandler;
using JHSchool.Legacy.Export.ResponseHandler.Output;
using System.Diagnostics;
using DevComponents.DotNetBar.Rendering;
using FISCA.Presentation.Controls;
using Framework;
using JHSchool.Feature.Legacy;

namespace JHSchool.StudentExtendControls.Ribbon
{
    public partial class StudentExportWizard : BaseForm
    {
        //private ButtonX advButton;
        //private DevComponents.DotNetBar.ControlContainerItem advContainer;
        //private LinkLabel helpButton;

        //public event EventHandler HelpButtonClick;

        public StudentExportWizard()
        {
            InitializeComponent();

            //#region 加入進階跟HELP按鈕
            //advContainer = new ControlContainerItem();
            //advContainer.AllowItemResize = false;
            //advContainer.GlobalItem = false;
            //advContainer.MenuVisibility = eMenuVisibility.VisibleAlways;

            //ItemContainer itemContainer2 = new ItemContainer();
            //itemContainer2.LayoutOrientation = DevComponents.DotNetBar.eOrientation.Vertical;
            //itemContainer2.MinimumSize = new System.Drawing.Size(0, 0);
            //itemContainer2.SubItems.AddRange(new DevComponents.DotNetBar.BaseItem[] {
            //advContainer});

            //advButton = new ButtonX();
            //advButton.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            //advButton.Text = "    進階";
            //advButton.Top = this.wizard1.Controls[1].Controls[0].Top;
            //advButton.Left = 5;
            //advButton.Size = this.wizard1.Controls[1].Controls[0].Size;
            //advButton.Visible = true;
            //advButton.SubItems.Add(itemContainer2);
            //advButton.PopupSide = ePopupSide.Top;
            //advButton.SplitButton = true;
            //advButton.Enabled = false;
            //advButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            //advButton.AutoExpandOnClick = true;
            //advButton.SubItemsExpandWidth = 16;
            //advButton.FadeEffect = false;
            //advButton.FocusCuesEnabled = false;
            //this.wizard1.Controls[1].Controls.Add(advButton);

            //helpButton = new LinkLabel();
            //helpButton.AutoSize = true;
            //helpButton.BackColor = System.Drawing.Color.Transparent;
            //helpButton.Location = new System.Drawing.Point(81, 10);
            //helpButton.Size = new System.Drawing.Size(69, 17);
            //helpButton.TabStop = true;
            //helpButton.Text = "Help";
            ////helpButton.Top = this.wizard1.Controls[1].Controls[0].Top + this.wizard1.Controls[1].Controls[0].Height - helpButton.Height;
            ////helpButton.Left = 150;
            //helpButton.Visible = false;
            //helpButton.Click += delegate { if (HelpButtonClick != null)HelpButtonClick(this, new EventArgs()); };
            //helpButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            //this.wizard1.Controls[1].Controls.Add(helpButton);
            //#endregion

            #region 設定Wizard會跟著Style跑
            this.wizard1.HeaderStyle.ApplyStyle((GlobalManager.Renderer as Office2007Renderer).ColorTable.GetClass(ElementStyleClassKeys.RibbonFileMenuBottomContainerKey));
            this.wizard1.FooterStyle.BackColorGradientAngle = -90;
            this.wizard1.FooterStyle.BackColorGradientType = eGradientType.Linear;
            this.wizard1.FooterStyle.BackColor = (GlobalManager.Renderer as Office2007Renderer).ColorTable.RibbonBar.Default.TopBackground.Start;
            this.wizard1.FooterStyle.BackColor2 = (GlobalManager.Renderer as Office2007Renderer).ColorTable.RibbonBar.Default.TopBackground.End;
            this.wizard1.BackColor = (GlobalManager.Renderer as Office2007Renderer).ColorTable.RibbonBar.Default.TopBackground.Start;
            this.wizard1.BackgroundImage = null;
            for (int i = 0; i < 5; i++)
            {
                (this.wizard1.Controls[1].Controls[i] as ButtonX).ColorTable = eButtonColor.OrangeWithBackground;
            }
            (this.wizard1.Controls[0].Controls[1] as System.Windows.Forms.Label).ForeColor = (GlobalManager.Renderer as Office2007Renderer).ColorTable.RibbonBar.MouseOver.TitleText;
            (this.wizard1.Controls[0].Controls[2] as System.Windows.Forms.Label).ForeColor = (GlobalManager.Renderer as Office2007Renderer).ColorTable.RibbonBar.Default.TitleText;
            #endregion
        }

        private void wizard1_CancelButtonClick(object sender, CancelEventArgs e)
        {
            this.Close();
        }

        private void wizard1_FinishButtonClick(object sender, CancelEventArgs e)
        {
            if (GetSelectedFields().Count == 0)
            {
                FISCA.Presentation.Controls.MsgBox.Show("必須至少選擇一項匯出欄位!", "欄位空白", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string reason = "";

            Forms.StudentInformationExportWarningForm warningform = new Forms.StudentInformationExportWarningForm();

            if (warningform.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                reason = warningform.reason;
            }
            else 
            {
                // 若使用者 沒有鍵入事由、點選取消，則中止匯出學生基本程序。
                return;                        
            }

            // 取得欄位名稱
            List<string> field_name_list = new List<string>();

            foreach (var field in GetSelectedFields())
            {
                field_name_list.Add(field.DisplayText);                                        
            }

            string field_name = "";

            field_name = string.Join(",",field_name_list);

            StringBuilder sb = new StringBuilder();


            saveFileDialog1.Filter = "Excel (*.xls)|*.xls|所有檔案 (*.*)|*.*";
            saveFileDialog1.FileName = "匯出學生基本資料";

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                ExportStudentConnector ec = new ExportStudentConnector();
                foreach (StudentRecord student in Student.Instance.SelectedList)
                {
                    ec.AddCondition(student.ID);

                    string student_log = "";

                    student_log = (student.Class!=null ? student.Class.Name : "") +","+student.SeatNo+","+student.StudentNumber+","+student.Name;

                    sb.AppendLine(student_log);


                }
                ec.SetSelectedFields(GetSelectedFields());
                ExportTable table = ec.Export();

                ExportOutput output = new ExportOutput();
                output.SetSource(table);
                try
                {
                    output.Save(saveFileDialog1.FileName);
                    PermRecLogProcess prlp = new PermRecLogProcess();

                    //prlp.SaveLog("學生.匯出學生基本資料", "批次匯出", "匯出" + Student.Instance.SelectedKeys.Count + "筆學生資料，匯出欄位:"+ s+ "，匯出事由:"+reason);

                    prlp.SaveLog("學生.匯出學生基本資料", "批次匯出", "匯出學生(班級,座號,學號,姓名)" + "\r\n" + "\r\n" + sb +  "\r\n" + "匯出欄位:" + field_name + "\r\n" +  "\r\n" + "匯出用途:" + reason);
                }
                catch (Exception)
                {
                    FISCA.Presentation.Controls.MsgBox.Show("檔案儲存失敗, 檔案目前可能已經開啟。", "儲存失敗", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (FISCA.Presentation.Controls.MsgBox.Show("檔案存檔完成，是否開啟該檔案", "是否開啟", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    try
                    {
                        System.Diagnostics.Process.Start(saveFileDialog1.FileName);
                    }
                    catch (Exception ex)
                    {
                        FISCA.Presentation.Controls.MsgBox.Show("開啟檔案發生失敗:" + ex.Message, "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                this.Close();
            }
        }

        private void ExportWizard_Load(object sender, EventArgs e)
        {
            XmlElement element = StudentBulkProcess.GetExportDescription();
            BaseFieldFormater formater = new BaseFieldFormater();
            FieldCollection collection = formater.Format(element);

            //// 加入可以匯出學生狀態
            //Field fld1 = new Field();
            //fld1.DisplayText = "狀態";
            //fld1.FieldName = "StudentStatus";
            //collection.Add(fld1);
            List<string> list = new List<string>(new string[] { "學生系統編號", "姓名", "學號", "身分證號", "狀態" });
            
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

        private void chkSelect_CheckedChanged(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listView.Items)
            {
                item.Checked = chkSelect.Checked;
            }
        }
    }
}
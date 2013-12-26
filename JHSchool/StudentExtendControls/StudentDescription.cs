using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DevComponents.DotNetBar;
using Framework;

namespace JHSchool.StudentExtendControls
{
    /// <summary>
    /// 程式碼很多...。
    /// </summary>
    public partial class StudentDescription : TaggingDescription
    {
        private LabelX DescriptionLabel;
        private PanelEx StatusPanel;
        private ContextMenuBar StatusMenuBar;
        private ButtonItem Status1;
        private ButtonItem Status2;
        private ButtonItem Status3;
        private ButtonItem Status4;
        private ButtonItem Status5;
        private ButtonItem Status6;

        public StudentDescription()
        {
            InitializeComponent();

            #region InitializeComponent Manual
            this.StatusMenuBar = new DevComponents.DotNetBar.ContextMenuBar();
            this.Status1 = new DevComponents.DotNetBar.ButtonItem();
            this.Status2 = new DevComponents.DotNetBar.ButtonItem();
            this.Status3 = new DevComponents.DotNetBar.ButtonItem();
            this.Status4 = new DevComponents.DotNetBar.ButtonItem();
            this.Status5 = new DevComponents.DotNetBar.ButtonItem();
            this.Status6 = new DevComponents.DotNetBar.ButtonItem();
            this.StatusPanel = new DevComponents.DotNetBar.PanelEx();
            ((System.ComponentModel.ISupportInitialize)(this.StatusMenuBar)).BeginInit();
            this.SuspendLayout();
            // 
            // contextMenuBar1
            // 
            this.StatusMenuBar.Items.AddRange(new DevComponents.DotNetBar.BaseItem[] {
            this.Status1});
            this.StatusMenuBar.Location = new System.Drawing.Point(36, 12);
            this.StatusMenuBar.Size = new System.Drawing.Size(123, 25);
            this.StatusMenuBar.Stretch = true;
            this.StatusMenuBar.Style = DevComponents.DotNetBar.eDotNetBarStyle.Office2007;
            this.StatusMenuBar.TabIndex = 184;
            this.StatusMenuBar.TabStop = false;
            this.StatusMenuBar.Text = "StatusMenuBar";
            // 
            // buttonItem1
            // 
            this.Status1.AutoExpandOnClick = true;
            this.Status1.ImageListSizeSelection = DevComponents.DotNetBar.eButtonImageListSelection.NotSet;
            this.Status1.ImagePaddingHorizontal = 8;
            this.Status1.SubItems.AddRange(new DevComponents.DotNetBar.BaseItem[] {
            this.Status2,
            this.Status3,
            this.Status4,
            this.Status5,
            this.Status6});
            this.Status1.Text = "statusMenu";
            // 
            // buttonItem2
            // 
            this.Status2.AutoCheckOnClick = true;
            this.Status2.ImageListSizeSelection = DevComponents.DotNetBar.eButtonImageListSelection.NotSet;
            this.Status2.ImagePaddingHorizontal = 8;
            this.Status2.OptionGroup = "status";
            this.Status2.Text = "一般";
            this.Status2.CheckedChanged += new System.EventHandler(this.buttonItem2_CheckedChanged);
            // 
            // buttonItem3
            // 
            this.Status3.AutoCheckOnClick = true;
            this.Status3.ImageListSizeSelection = DevComponents.DotNetBar.eButtonImageListSelection.NotSet;
            this.Status3.ImagePaddingHorizontal = 8;
            this.Status3.OptionGroup = "status";
            this.Status3.Text = "畢業或離校";
            this.Status3.CheckedChanged += new System.EventHandler(this.buttonItem2_CheckedChanged);
            // 
            // buttonItem4
            // 
            this.Status4.AutoCheckOnClick = true;
            this.Status4.ImageListSizeSelection = DevComponents.DotNetBar.eButtonImageListSelection.NotSet;
            this.Status4.ImagePaddingHorizontal = 8;
            this.Status4.OptionGroup = "status";
            this.Status4.Text = "休學";
            this.Status4.CheckedChanged += new System.EventHandler(this.buttonItem2_CheckedChanged);
            // 
            // buttonItem5
            // 
            this.Status5.AutoCheckOnClick = true;
            this.Status5.ImageListSizeSelection = DevComponents.DotNetBar.eButtonImageListSelection.NotSet;
            this.Status5.ImagePaddingHorizontal = 8;
            this.Status5.OptionGroup = "status";
            this.Status5.Text = "延修";
            this.Status5.Visible = false;
            this.Status5.CheckedChanged += new System.EventHandler(this.buttonItem2_CheckedChanged);
            // 
            // buttonItem6
            // 
            this.Status6.AutoCheckOnClick = true;
            this.Status6.ImageListSizeSelection = DevComponents.DotNetBar.eButtonImageListSelection.NotSet;
            this.Status6.ImagePaddingHorizontal = 8;
            this.Status6.OptionGroup = "status";
            this.Status6.Text = "輟學";
            this.Status6.CheckedChanged += new System.EventHandler(this.buttonItem2_CheckedChanged);



            // 
            // panelEx1
            // 
            this.StatusPanel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.StatusPanel.CanvasColor = System.Drawing.SystemColors.Control;
            this.StatusPanel.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007;
            this.StatusPanel.Location = new System.Drawing.Point(66, 3);
            this.StatusPanel.Margin = new System.Windows.Forms.Padding(0);
            this.StatusPanel.Size = new System.Drawing.Size(95, 20);
            this.StatusPanel.Style.Alignment = System.Drawing.StringAlignment.Center;
            this.StatusPanel.Style.BackColor1.Color = System.Drawing.Color.LightBlue;
            this.StatusPanel.Style.BackColor2.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.BarBackground2;
            this.StatusPanel.Style.BorderColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.BarDockedBorder;
            this.StatusPanel.Style.BorderSide = DevComponents.DotNetBar.eBorderSide.None;
            this.StatusPanel.Style.BorderWidth = 0;
            this.StatusPanel.Style.CornerType = DevComponents.DotNetBar.eCornerType.Rounded;
            this.StatusPanel.Style.ForeColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.ItemText;
            this.StatusPanel.Style.GradientAngle = 90;
            this.StatusPanel.Style.TextTrimming = System.Drawing.StringTrimming.Word;
            this.StatusPanel.TabIndex = 184;
            this.StatusPanel.Text = "一般";
            this.StatusPanel.Click += new System.EventHandler(this.StatusPanel_Click);
            //
            // DescriptionLabel
            //
            DescriptionLabel = new LabelX();
            DescriptionLabel.Text = string.Empty;
            DescriptionLabel.Dock = DockStyle.Left;
            DescriptionLabel.AutoSize = true;
            DescriptionLabel.Font = new Font(Font.FontFamily, 13);
            // 
            // StudentDescription
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.AutoSize = true;
            this.Controls.Add(this.StatusMenuBar);
            this.Name = "StudentDescription";
            this.Size = new System.Drawing.Size(375, 55);
            ((System.ComponentModel.ISupportInitialize)(this.StatusMenuBar)).EndInit();
            this.ResumeLayout(false);

            DescriptionPanel.Controls.Add(DescriptionLabel);
            DescriptionPanel.Controls.Add(StatusPanel);
            #endregion

            Student.Instance.ItemUpdated += delegate(object sender, ItemUpdatedEventArgs e)
            {
                if (e.PrimaryKeys.Contains(PrimaryKey))
                    OnPrimaryKeyChanged(e);
            };
        }

        protected override void OnPrimaryKeyChanged(EventArgs arg)
        {
            base.OnPrimaryKeyChanged(arg);

            if (string.IsNullOrEmpty(PrimaryKey)) return;

            DisplayInformation<StudentTag, List<StudentTagRecord>, StudentTagRecord>(StudentTag.Instance);

            var stu = Student.Instance.Items[PrimaryKey];

            if (stu == null)
                DescriptionLabel.Text = string.Empty;
            else
            {
                if (stu.Class == null)
                    DescriptionLabel.Text = string.Format("{0} {1}", stu.Name, stu.StudentNumber);
                else
                {
                    if (string.IsNullOrEmpty(stu.SeatNo))
                        DescriptionLabel.Text = string.Format("{0} {1} {2}", stu.Class.Name, stu.Name, stu.StudentNumber);
                    else
                        DescriptionLabel.Text = string.Format("{0}({1}) {2} {3}", stu.Class.Name, stu.SeatNo, stu.Name, stu.StudentNumber);
                }

                Color s;
                switch (stu.Status)
                {
                    case "一般":
                        s = Color.FromArgb(255, 255, 255);
                        break;
                    case "畢業或離校":
                        s = Color.FromArgb(156, 220, 128);
                        break;
                    case "休學":
                        s = Color.FromArgb(254, 244, 128);
                        break;
                    case "延修":
                        s = Color.FromArgb(224, 254, 210);
                        break;
                    case "輟學":
                        s = Color.FromArgb(254, 244, 128);
                        break;
                    case "刪除":
                        s = Color.FromArgb(254, 128, 155);
                        break;
                    default:
                        s = Color.Transparent;
                        break;
                }

                StatusPanel.Text = stu.Status;
                StatusPanel.Style.BackColor1.Color = s;
                StatusPanel.Style.BackColor2.Color = s;

                foreach (var item in new DevComponents.DotNetBar.ButtonItem[] { Status2, Status3, Status4, Status5 })
                {
                    if (item.Text == stu.Status)
                        item.Checked = true;
                    else
                        item.Checked = false;

                }
            }
        }

        private void buttonItem2_CheckedChanged(object sender, EventArgs e)
        {
            var button = (DevComponents.DotNetBar.ButtonItem)sender;
            if (button.Checked)
            {
                var studentRec = Student.Instance.Items[PrimaryKey];
                if (studentRec != null)
                {
                    if (button.Text != studentRec.Status)
                    {
                        //if (button.Text == "刪除")
                        //    throw new ArgumentException();

                        try
                        {
                            PermRecLogProcess prlp = new PermRecLogProcess();

                            JHSchool.Data.JHStudentRecord stu = JHSchool.Data.JHStudent.SelectByID(PrimaryKey);
                            prlp.SetBeforeSaveText("學生狀態", stu.Status.ToString());


                            if (K12.Data.StudentRecord.StudentStatus.一般.ToString() == button.Text)
                                stu.Status = K12.Data.StudentRecord.StudentStatus.一般;

                            if (K12.Data.StudentRecord.StudentStatus.休學.ToString() == button.Text)
                                stu.Status = K12.Data.StudentRecord.StudentStatus.休學;

                            if (K12.Data.StudentRecord.StudentStatus.畢業或離校.ToString() == button.Text)
                                stu.Status = K12.Data.StudentRecord.StudentStatus.畢業或離校;

                            if (K12.Data.StudentRecord.StudentStatus.輟學.ToString() == button.Text)
                                stu.Status = K12.Data.StudentRecord.StudentStatus.輟學;

                            if (K12.Data.StudentRecord.StudentStatus.刪除.ToString() == button.Text)
                                stu.Status = K12.Data.StudentRecord.StudentStatus.刪除;

                            // 檢查同狀態要身分證或學號相同時，無法變更

                            List<string> checkIDNumber = new List<string>();
                            List<string> checkSnum = new List<string>();

                            foreach (JHSchool.Data.JHStudentRecord studRec in JHSchool.Data.JHStudent.SelectAll())
                            {
                                if (studRec.Status == stu.Status)
                                {
                                    if (!string.IsNullOrEmpty(studRec.StudentNumber))
                                        checkSnum.Add(studRec.StudentNumber.Trim());
                                    if (!string.IsNullOrEmpty(studRec.IDNumber))
                                        checkIDNumber.Add(studRec.IDNumber.Trim());
                                }
                            }

                            if (checkSnum.Contains(stu.StudentNumber.Trim()))
                            {
                                MsgBox.Show("在" + stu.Status.ToString() + "狀態學號有重複無法變更.");
                                return;
                            }

                            if (checkIDNumber.Contains(stu.IDNumber.Trim()))
                            {
                                MsgBox.Show("在" + stu.Status.ToString() + "狀態身分證號有重複無法變更.");
                                return;
                            }

                            JHSchool.Data.JHStudent.Update(stu);
                            prlp.SetAfterSaveText("學生狀態", stu.Status.ToString());
                            prlp.SetActionBy("學籍", "學生學生狀態");
                            prlp.SetAction("修改學生學生狀態");
                            prlp.SaveLog("", "", "student", PrimaryKey);


                            //Feature.Legacy.EditStudent.ChangeStudentStatus(studentRec.ID, button.Text);
                        }
                        catch (ArgumentException aa)
                        {
                            MessageBox.Show("目前無法移到刪除");
                        }

                        catch
                        {
                            OnPrimaryKeyChanged(new EventArgs());
                            FISCA.Presentation.MotherForm.SetStatusBarMessage("變更狀態失敗，可能發生原因為學號或身分證號在" + button.Text + "學生中已經存在，請檢查學生資料。");
                            return;
                        }
                        Student.Instance.SyncDataBackground(studentRec.ID);
                        FISCA.Presentation.MotherForm.SetStatusBarMessage("已變更學生狀態。" + DescriptionLabel.Text);
                    }
                }
            }
        }

        private void StatusPanel_Click(object sender, EventArgs e)
        {
            Status1.Popup(StatusPanel.PointToScreen(new Point(0, StatusPanel.Height)));
        }
    }
}

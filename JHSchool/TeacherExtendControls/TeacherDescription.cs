using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Framework;
using DevComponents.DotNetBar;

namespace JHSchool.TeacherExtendControls
{
    public partial class TeacherDescription : TaggingDescription
    {
        LabelX DescriptionLabel;

        private PanelEx StatusPanel;
        private ContextMenuBar StatusMenuBar;
        private ButtonItem Status1;
        private ButtonItem Status2;
        private ButtonItem Status3;


        public TeacherDescription()
        {
            InitializeComponent();

                        #region InitializeComponent Manual
            this.StatusMenuBar = new DevComponents.DotNetBar.ContextMenuBar();
            this.Status1 = new DevComponents.DotNetBar.ButtonItem();
            this.Status2 = new DevComponents.DotNetBar.ButtonItem();
            this.Status3 = new DevComponents.DotNetBar.ButtonItem();
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
            this.Status3
            });
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
            this.Status3.Text = "刪除";
            this.Status3.CheckedChanged += new System.EventHandler(this.buttonItem2_CheckedChanged);



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
            // TeacherDescription
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.AutoSize = true;
            this.Controls.Add(this.StatusMenuBar);
            this.Name = "TeacherDescription";
            this.Size = new System.Drawing.Size(375, 55);
            ((System.ComponentModel.ISupportInitialize)(this.StatusMenuBar)).EndInit();
            this.ResumeLayout(false);

            DescriptionPanel.Controls.Add(DescriptionLabel);
            DescriptionPanel.Controls.Add(StatusPanel);
            #endregion


            //DescriptionLabel = new LabelX();
            //DescriptionLabel.Text = string.Empty;
            //DescriptionLabel.Dock = DockStyle.Left;
            //DescriptionLabel.AutoSize = true;
            //DescriptionLabel.Font = new Font(Font.FontFamily, 13);
            //DescriptionPanel.Controls.Add(DescriptionLabel);

            Teacher.Instance.ItemUpdated += delegate(object sender, ItemUpdatedEventArgs e)
            {
                if (e.PrimaryKeys.Contains(PrimaryKey))
                    OnPrimaryKeyChanged(e);
            };
        }

        protected override void OnPrimaryKeyChanged(EventArgs arg)
        {
            base.OnPrimaryKeyChanged(arg);

            if (string.IsNullOrEmpty(PrimaryKey)) return;

            DescriptionLabel.Text = Teacher.Instance[PrimaryKey].Name;
            DisplayInformation<TeacherTag, List<TeacherTagRecord>, TeacherTagRecord>(TeacherTag.Instance);

            var Teach = Teacher.Instance.Items[PrimaryKey];

            Color s;            
            
            switch (Teach.Status)
            {
                case "一般":
                    s = Color.FromArgb(255, 255, 255);
                    break;
                case "刪除":
                    s = Color.FromArgb(254, 128, 155);
                    break;
                default:
                    s = Color.Transparent;
                    break;
            }
            StatusPanel.Text = Teach.Status;
            StatusPanel.Style.BackColor1.Color = s;
            StatusPanel.Style.BackColor2.Color = s;

            foreach (var item in new DevComponents.DotNetBar.ButtonItem[] { Status2, Status3 })
            {
                if (item.Text == Teach.Status)
                    item.Checked = true;
                else
                    item.Checked = false;

            }

        }

        private void buttonItem2_CheckedChanged(object sender, EventArgs e)
        {
            var button = (DevComponents.DotNetBar.ButtonItem)sender;
            if (button.Checked)
            {
                var teacherRec = Teacher.Instance.Items[PrimaryKey];
                if (teacherRec != null)
                {
                    if (button.Text != teacherRec.Status)
                    {
                        try
                        {
                            JHSchool.Data.JHTeacherRecord teach = JHSchool.Data.JHTeacher.SelectByID(PrimaryKey);


                            if (K12.Data.TeacherRecord.TeacherStatus.一般.ToString() == button.Text)
                                teach.Status = K12.Data.TeacherRecord.TeacherStatus.一般;

                            if (K12.Data.TeacherRecord.TeacherStatus.刪除.ToString ()== button.Text)
                                teach.Status = K12.Data.TeacherRecord.TeacherStatus.刪除;

                            JHSchool.Data.JHTeacher.Update(teach);

                            PermRecLogProcess prlp = new PermRecLogProcess();
                            prlp.SaveLog("教師", "修改", "修改教師狀態");

                            //Feature.Legacy.EditStudent.ChangeStudentStatus(studentRec.ID, button.Text);
                        }
                        catch (ArgumentException aa)
                        {
                            MessageBox.Show("目前無法移到刪除");
                        }

                        catch
                        {
                            OnPrimaryKeyChanged(new EventArgs());
                            FISCA.Presentation.MotherForm.SetStatusBarMessage("變更狀態失敗，可能發生原因為學號或身分證號在" + button.Text + "敎師中已經存在，請檢查教師資料。");
                            return;
                        }
                        Teacher.Instance.SyncDataBackground(teacherRec.ID);
                        FISCA.Presentation.MotherForm.SetStatusBarMessage("已變更教師狀態。" + DescriptionLabel.Text);
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

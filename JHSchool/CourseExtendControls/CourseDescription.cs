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

namespace JHSchool.CourseExtendControls
{
    public partial class CourseDescription : TaggingDescription
    {
        LabelX DescriptionLabel;

        public CourseDescription()
        {
            InitializeComponent();

            DescriptionLabel = new LabelX();
            DescriptionLabel.Text = string.Empty;
            DescriptionLabel.Dock = DockStyle.Left;
            DescriptionLabel.AutoSize = true;
            DescriptionLabel.Font = new Font(Font.FontFamily, 13);
            DescriptionPanel.Controls.Add(DescriptionLabel);

            Course.Instance.ItemUpdated += delegate(object sender, ItemUpdatedEventArgs e)
            {
                if (e.PrimaryKeys.Contains(PrimaryKey))
                    OnPrimaryKeyChanged(e);
            };
        }

        protected override void OnPrimaryKeyChanged(EventArgs arg)
        {
            base.OnPrimaryKeyChanged(arg);

            if (string.IsNullOrEmpty(PrimaryKey)) return;
            if(Course.Instance[PrimaryKey]!=null)
            DescriptionLabel.Text = Course.Instance[PrimaryKey].Name;
            DisplayInformation<CourseTag, List<CourseTagRecord>, CourseTagRecord>(CourseTag.Instance);
        }
    }
}

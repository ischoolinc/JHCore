using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DevComponents.DotNetBar;
using Framework;

namespace JHSchool.ClassExtendControls
{
    public partial class ClassDescription : TaggingDescription
    {
        LabelX DescriptionLabel;

        public ClassDescription()
        {
            InitializeComponent();

            DescriptionLabel = new LabelX();
            DescriptionLabel.Text = string.Empty;
            DescriptionLabel.Dock = DockStyle.Left;
            DescriptionLabel.AutoSize = true;
            DescriptionLabel.Font = new Font(Font.FontFamily, 13);
            DescriptionPanel.Controls.Add(DescriptionLabel);

            Class.Instance.ItemUpdated += delegate(object sender, ItemUpdatedEventArgs e)
            {
                if (e.PrimaryKeys.Contains(PrimaryKey))
                    OnPrimaryKeyChanged(e);
            };
        }

        protected override void OnPrimaryKeyChanged(EventArgs arg)
        {
            base.OnPrimaryKeyChanged(arg);

            if (string.IsNullOrEmpty(PrimaryKey)) return;
            if(Class.Instance[PrimaryKey]!=null)
            DescriptionLabel.Text = Class.Instance[PrimaryKey].Name;
            DisplayInformation<ClassTag, List<ClassTagRecord>, ClassTagRecord>(ClassTag.Instance);
        }
    }
}

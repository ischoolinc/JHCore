using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Framework.Security;
using FISCA.Presentation.Controls;

namespace JHSchool.StudentExtendControls
{
    public partial class NavScopeAceEditor : BaseForm, IAceEditor
    {
        public NavScopeAceEditor()
        {
            InitializeComponent();
        }

        #region IAceEditor 成員

        public void ShowEditor()
        {
            ShowDialog();
        }

        public string PermissionString
        {
            get
            {
                return textBoxX1.Text;
            }
            set
            {
                textBoxX1.Text = value;
            }
        }

        #endregion

        private void buttonX1_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}

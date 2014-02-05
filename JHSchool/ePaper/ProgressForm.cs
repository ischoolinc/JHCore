using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation.Controls;

namespace JHSchool
{
    public partial class ProgressForm : BaseForm
    {
        public ProgressForm(string caption)
        {
            InitializeComponent();
            Text = caption;
        }

        public int Minimum
        {
            get { return progressBarX1.Minimum; }
            set { progressBarX1.Minimum = value; }
        }

        public int Maximum
        {
            get { return progressBarX1.Maximum; }
            set { progressBarX1.Maximum = value; }
        }

        public int Value
        {
            get { return progressBarX1.Value; }
            set { progressBarX1.Value = value; }
        }

        public void Finish()
        {
            progressBarX1.Value = progressBarX1.Maximum;
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Close();
        }

        private void ProgressForm_Load(object sender, EventArgs e)
        {
        }
    }
}
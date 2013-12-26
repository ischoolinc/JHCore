using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using FISCA.Presentation.Controls;

namespace JHSchool.Legacy
{
    public partial class ExtensionTemplateSetup : BaseForm
    {
        int errCount = 0;

        public ExtensionTemplateSetup()
        {
            InitializeComponent();
            //XmlElement setting = Customization.Data.SystemInformation.Configuration["延伸欄位設定值"];
            //foreach ( XmlElement var in setting.SelectNodes("延伸欄位") )
            //{
                //if ( colType.Items.Contains(var.GetAttribute("格式")) )
                //    dataGridViewEx1.Rows.Add(var.GetAttribute("欄位名稱"), var.GetAttribute("格式"));
                //else
                //    dataGridViewEx1.Rows.Add(var.GetAttribute("欄位名稱"), "文字");
            //}
        }

        private void buttonX2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void buttonX1_Click(object sender, EventArgs e)
        {
            XmlDocument doc = new XmlDocument();
            XmlElement eleRoot = doc.CreateElement("延伸欄位設定值");
            foreach ( DataGridViewRow row in dataGridViewEx1.Rows )
            {
                if ( row.IsNewRow ) continue;
                string k = "" + row.Cells[0].Value, v = "" + row.Cells[1].Value;
                XmlElement eleField=doc.CreateElement("延伸欄位");
                eleField.SetAttribute("欄位名稱", k);
                eleField.SetAttribute("格式", v);
                eleRoot.AppendChild(eleField);
            }
            //Customization.Data.SystemInformation.Configuration["延伸欄位設定值"] = eleRoot;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void dataGridViewEx1_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if ( dataGridViewEx1.SelectedCells.Count > 0 )
                dataGridViewEx1.BeginEdit(true);
        }

        private void dataGridViewEx1_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if ( e.Button == MouseButtons.Left )
                dataGridViewEx1.EndEdit();
        }

        private void dataGridViewEx1_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            foreach ( DataGridViewRow row in dataGridViewEx1.Rows )
            {
                if ( row.IsNewRow ) continue;
                if (! colType.Items.Contains(""+row.Cells[1].Value) )
                    row.Cells[1].Value = colType.Items[0];
            }
        }

        private void dataGridViewEx1_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            dataGridViewEx1.EndEdit();
            if ( dataGridViewEx1.CurrentCell != null && dataGridViewEx1.CurrentCell.ColumnIndex == 0 )
            {
                if ( dataGridViewEx1.CurrentCell.ErrorText != "" )
                {
                    dataGridViewEx1.CurrentCell.ErrorText = "";
                    dataGridViewEx1.UpdateCellErrorText(dataGridViewEx1.CurrentCell.ColumnIndex, dataGridViewEx1.CurrentCell.RowIndex);
                    errCount--;
                }
                string k = "" + dataGridViewEx1.CurrentCell.Value;
                foreach ( string var in new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" } )
                {
                    if ( k.StartsWith(var) )
                    {
                        dataGridViewEx1.CurrentCell.ErrorText = "欄位名稱開頭不可為數字。";
                        dataGridViewEx1.UpdateCellErrorText(dataGridViewEx1.CurrentCell.ColumnIndex, dataGridViewEx1.CurrentCell.RowIndex);
                        errCount++;
                    }
                }
            }
            this.buttonX1.Enabled = (errCount == 0);
            dataGridViewEx1.BeginEdit(false);
        }
    }
}
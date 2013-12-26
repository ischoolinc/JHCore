using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using FISCA.DSAUtil;
using FISCA.Presentation.Controls;

namespace JHSchool.Legacy
{
    public partial class XmlBox : BaseForm
    {
        public XmlBox()
        {
            InitializeComponent();
        }

        public static void ShowXml(XmlElement xml, IWin32Window owner)
        {
            XmlBox box = new XmlBox();
            box.richTextBox1.Text = DSXmlHelper.Format(xml.OuterXml);
            box.ShowDialog(owner);
        }

        public static void ShowXml(XmlElement xml)
        {
            ShowXml(xml, null);
        }

    }
}
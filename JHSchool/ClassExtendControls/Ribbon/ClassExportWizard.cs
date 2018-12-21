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

        //�����e��
        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        private void ExportClass_Load(object sender, EventArgs e)
        {
            XmlElement element = SmartSchool.Feature.Class.ClassBulkProcess.GetExportDescription();

            #region XML���e
            /*	<FieldDescription>
					<Field DisplayText="�Z�Ũt�νs��" Name="ID" ReadOnly="True" />
					<Field DisplayText="�Z�ŦW��" Name="ClassName" ShiftCheckable="True" />
					<Field DisplayText="�Z�ɮv" Name="TeacherName" ShiftCheckable="True" />
					<Field DisplayText="�~��" Name="GradeYear" ShiftCheckable="True" />
					<Field DisplayText="��O" Name="DepartmentName" ShiftCheckable="True" />
					<Field DisplayText="�ҵ{�W��" Name="GraduationPlan" ShiftCheckable="True" />
					<Field DisplayText="�p��W�h" Name="CalculationRule" ShiftCheckable="True" />
					<Field DisplayText="�ƦC�Ǹ�" Name="DisplayOrder" ShiftCheckable="True" />
					<Field DisplayText="�Z�ŦW�ٳW�h" Name="NamingRule" ShiftCheckable="True" />
				</FieldDescription>
             */



            #endregion

            BaseFieldFormater formater = new BaseFieldFormater();
            //�N��Ʈ榡��,�òզ����X
            FieldCollection collection = formater.Format(element);

            // 2018/12/21 �o�~ �]���������� [10-03][??] ���ݰ��d�߾ǮկZ�Ŧ��վ㡨�ɮv�����\��
            // ���ץX�������A�Z�ɮv�]�ǤJ �������
            // ���Z�L�����A ��Ӫ��������~�M�u�O�o�˶�gList...
            // �����J�����۰ʽs�Z�Ҳժ� �A ��פJ�W�h ��Local ���]�w(�Z�ɮv����)
            // ��l���Ǯ� �̵M���°��k�A��Service ���פJ�W�h���

            List<string> list;

            IClassBaseInfoItemAPI ClassBaseInfoItem = FISCA.InteractionService.DiscoverAPI<IClassBaseInfoItemAPI>();
            if (ClassBaseInfoItem != null)
            {
                list = new List<string>(new string[] { "�Z�Ũt�νs��", "�Z�ŦW��","�Z�ɮv" });
            }
            else
            {
                list = new List<string>(new string[] { "�Z�Ũt�νs��", "�Z�ŦW��" });
            }
            
            //�N���X���e,�v�@��J�ϥΪ̤Ŀ�M�椤(Tag��m�@��field),�w�]��(true)
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
            //�P�_�Ŀ�Ӽ�
            if (GetSelectedFields().Count == 0)
            {
                FISCA.Presentation.Controls.MsgBox.Show("�����ܤֿ�ܤ@���ץX���!", "���ť�", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2017/8/22 �o�~�̾ڰ����p�ձM�� [03-05][04+] EXCEL�פJ�榡�i�_�ץ���xlsx�]�i�פJ�H ��אּ�s�� Aspose.Cells_201402 �g�k�A�䴩.xlsx �ץX
            saveFileDialog1.Filter = "Excel (*.xlsx)|*.xlsx|Excel (*.xls)|*.xls|�Ҧ��ɮ� (*.*)|*.*";
            saveFileDialog1.FileName = "�ץX�Z�Ű򥻸��";

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

                if (FISCA.Presentation.Controls.MsgBox.Show("�ɮצs�ɧ����A�O�_�}�Ҹ��ɮ�", "�O�_�}��", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    try
                    {
                        Process.Start(saveFileDialog1.FileName);
                    }
                    catch (Exception ex)
                    {
                        FISCA.Presentation.Controls.MsgBox.Show("�}���ɮ׵o�ͥ���:" + ex.Message, "���~", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                this.Close();
            }
        }

        private FieldCollection GetSelectedFields()
        {
            FieldCollection collection = new FieldCollection();
            //��Ŀﶵ�ت����X
            foreach (ListViewItem item in listView.CheckedItems)
            {
                Field field = item.Tag as Field;
                collection.Add(field);
            }
            return collection;
        }

        //�������
        private void chkSelect_CheckedChanged(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listView.Items)
            {
                item.Checked = chkSelect.Checked;
            }
        }
    }
}
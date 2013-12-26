using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation.Controls;
using Framework.Legacy;
using FISCA.DSAUtil;
using System.Xml;


namespace JHSchool
{
    public partial class SchoolInfoMangement : BaseForm
    {
        SchoolInfoEntity SchoolInfoEnt;
        public SchoolInfoMangement()
        {
            InitializeComponent();

        }



        // 以後交給 DAL
        private SchoolInfoEntity getSchoolInfoData()
        {
            XmlElement Element= Framework.Feature.Config.GetSchoolInfo();
            XmlElement sElement = Framework.Feature.Config.GetSystemConfig();
            SchoolInfoEntity sie = new SchoolInfoEntity();

            sie.ChinsesName = getNodeData("ChineseName", Element, "SchoolInformation");
            sie.EnglishName = getNodeData("EnglishName", Element, "SchoolInformation");
            sie.ChineseAddress = getNodeData("Address", Element, "SchoolInformation");
            sie.EnglishAddress = getNodeData("EnglishAddress", Element, "SchoolInformation");
            sie.Code = getNodeData("Code", Element, "SchoolInformation");
            sie.Fax = getNodeData("Fax", Element,"SchoolInformation");
            sie.Telephone = getNodeData("Telephone", Element,"SchoolInformation");
                // 校長
            sie.ChancellorChsName = getNodeData("ChancellorChineseName", Element, "SchoolInformation");
            sie.ChancellorEngName = getNodeData("ChancellorEnglishName", Element, "SchoolInformation");
                // 教務主任
            sie.EduDirectorName = getNodeData("EduDirectorName", Element, "SchoolInformation");
                // 學務主任
            sie.StuDirectorName = getNodeData("StuDirectorName", Element, "SchoolInformation");

            sie.SchoolYear = getNodeData("DefaultSchoolYear", sElement, "SystemConfig");
            sie.Semester = getNodeData("DefaultSemester", sElement, "SystemConfig");

            return sie;
        }


        private string getNodeData(string nodeName, XmlElement Element,string nodesName)
        {
            string value = "";
            foreach (XmlElement xe in Element.SelectNodes(nodesName))
                {
                    if (xe.SelectSingleNode(nodeName) != null)
                        value = xe.SelectSingleNode(nodeName).InnerText;
                }
                return value;
        }

        // 以後交給 DAL
        private void setSchoolInfoData(SchoolInfoEntity ScInfoEntity)
        {
            //Framework.Feature.Config.SetSchoolInfo();
            // 學校基本資料
            DSXmlHelper helper = new DSXmlHelper("GetSchoolInfoResponse");
            helper.AddElement("SchoolInformation");
            helper.AddElement("SchoolInformation", "ChineseName", ScInfoEntity.ChinsesName);
            helper.AddElement("SchoolInformation", "EnglishName", ScInfoEntity.EnglishName);
            helper.AddElement("SchoolInformation", "Address", ScInfoEntity.ChineseAddress );
            helper.AddElement("SchoolInformation", "EnglishAddress", ScInfoEntity.EnglishAddress );
            helper.AddElement("SchoolInformation", "Code", ScInfoEntity.Code );
            helper.AddElement("SchoolInformation", "Fax", ScInfoEntity.Fax );
            helper.AddElement("SchoolInformation", "Telephone", ScInfoEntity.Telephone );
            helper.AddElement("SchoolInformation", "ChancellorChineseName", ScInfoEntity.ChancellorChsName);
            helper.AddElement("SchoolInformation", "ChancellorEnglishName", ScInfoEntity.ChancellorEngName);
            helper.AddElement("SchoolInformation", "EduDirectorName", ScInfoEntity.EduDirectorName );
            helper.AddElement("SchoolInformation", "StuDirectorName", ScInfoEntity.StuDirectorName );           
            Framework.Feature.Config.SetSchoolInfo(helper.BaseElement);
            
            // 學年度學期
            DSXmlHelper helper1 = new DSXmlHelper("SetSystemConfigRequest");
            helper1.AddElement("SystemConfig");
            helper1.AddElement("SystemConfig", "DefaultSchoolYear",SchoolInfoEnt.SchoolYear);
            helper1.AddElement("SystemConfig", "DefaultSemester", SchoolInfoEnt.Semester);
            Framework.Feature.Config.SetSystemConfig(helper1.BaseElement);

        }
        
        private class SchoolInfoEntity
        {
            public string Code { get; set; }
            public string  SchoolYear { get; set; }
            public string  Semester { get; set; }
            public string ChinsesName { get; set; }
            public string EnglishName { get; set; }
            public string ChineseAddress { get; set; }
            public string EnglishAddress { get; set; }
            public string Telephone { get; set; }
            public string Fax { get; set; }
            public string ChancellorChsName { get; set; }
            public string ChancellorEngName { get; set; }
            public string EduDirectorName { get; set; }
            public string StuDirectorName { get; set; }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SchoolInfoEnt.ChancellorChsName = txtChancellorChsName.Text;
            SchoolInfoEnt.ChancellorEngName = txtChancellorEngName.Text;
            SchoolInfoEnt.ChineseAddress = txtSchoolChsAddress.Text;
            SchoolInfoEnt.ChinsesName = txtSchoolChsName.Text;
            SchoolInfoEnt.Code = txtSchoolCode.Text;
            SchoolInfoEnt.EduDirectorName = txtEduDirectorName.Text;
            SchoolInfoEnt.StuDirectorName = txtStuDirectorName.Text;
            SchoolInfoEnt.Telephone = txtPhone.Text;
            SchoolInfoEnt.EnglishAddress = txtSchoolEngAddess.Text;
            SchoolInfoEnt.EnglishName = txtSchoolEngName.Text;
            SchoolInfoEnt.Fax = txtFax.Text;
            SchoolInfoEnt.SchoolYear = intIptSchoolYear.Text;
            SchoolInfoEnt.Semester = intIptSemester.Text;
            setSchoolInfoData(SchoolInfoEnt);
            MessageBox.Show("資料儲存完成");
            PermRecLogProcess prlp = new PermRecLogProcess();
            prlp.SaveLog("核心", "修改", "修改學校基本資料.");
            this.Close();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void SchoolInfoMangement_Load(object sender, EventArgs e)
        {
            SchoolInfoEnt = getSchoolInfoData();
            txtChancellorChsName.Text = SchoolInfoEnt.ChancellorChsName;
            txtChancellorEngName.Text = SchoolInfoEnt.ChancellorEngName;
            txtEduDirectorName.Text = SchoolInfoEnt.EduDirectorName;
            txtFax.Text = SchoolInfoEnt.Fax;
            txtPhone.Text = SchoolInfoEnt.Telephone;
            txtSchoolChsAddress.Text = SchoolInfoEnt.ChineseAddress;
            txtSchoolChsName.Text = SchoolInfoEnt.ChinsesName;
            txtSchoolCode.Text = SchoolInfoEnt.Code;
            txtSchoolEngAddess.Text = SchoolInfoEnt.EnglishAddress;
            txtSchoolEngName.Text = SchoolInfoEnt.EnglishName;
            if(!string.IsNullOrEmpty(SchoolInfoEnt.SchoolYear))
                intIptSchoolYear.Text = SchoolInfoEnt.SchoolYear;

            if(!string.IsNullOrEmpty(SchoolInfoEnt.Semester))
                intIptSemester.Text = SchoolInfoEnt.Semester;
            txtStuDirectorName.Text = SchoolInfoEnt.StuDirectorName;
        }
    }

}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation.Controls;
using FISCA.DSAUtil;
using System.Xml;
using Framework.Feature;

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
            XmlElement Element= Config.GetSchoolInfo();
            XmlElement sElement = Config.GetSystemConfig();
            SchoolInfoEntity sie = new SchoolInfoEntity();

            sie.ChinsesName = getNodeData("ChineseName", Element, "SchoolInformation");
            sie.EnglishName = getNodeData("EnglishName", Element, "SchoolInformation");
            sie.ChineseAddress = getNodeData("Address", Element, "SchoolInformation");
            sie.EnglishAddress = getNodeData("EnglishAddress", Element, "SchoolInformation");
            sie.Code = getNodeData("Code", Element, "SchoolInformation");
            sie.Fax = getNodeData("Fax", Element,"SchoolInformation");
            sie.Telephone = getNodeData("Telephone", Element,"SchoolInformation");
            sie.SchoolYear = getNodeData("DefaultSchoolYear", sElement, "SystemConfig");
            sie.Semester = getNodeData("DefaultSemester", sElement, "SystemConfig");

            //校長
            sie.ChancellorChsName = getNodeData("ChancellorChineseName", Element, "SchoolInformation");
            sie.ChancellorEngName = getNodeData("ChancellorEnglishName", Element, "SchoolInformation");
            sie.ChancellorCellPhone = getNodeData("ChancellorCellPhone", Element, "SchoolInformation");
            sie.ChancellorEmail = getNodeData("ChancellorEmail", Element, "SchoolInformation");

            //教務主任
            sie.EduDirectorName = getNodeData("EduDirectorName", Element, "SchoolInformation");
            sie.EduDirectorCellPhone = getNodeData("EduDirectorCellPhone", Element, "SchoolInformation");
            sie.EduDirectorEmail = getNodeData("EduDirectorEmail", Element, "SchoolInformation");

            //學務主任
            sie.StuDirectorName = getNodeData("StuDirectorName", Element, "SchoolInformation");
            sie.StuDirectorCellPhone = getNodeData("StuDirectorCellPhone", Element, "SchoolInformation");
            sie.StuDirectorEmail = getNodeData("StuDirectorEmail", Element, "SchoolInformation");

            sie.AssociatedWithName = getNodeData("AssociatedWithName", Element, "SchoolInformation");
            sie.AssociatedWithCellPhone = getNodeData("AssociatedWithCellPhone", Element, "SchoolInformation");
            sie.AssociatedWithEmail = getNodeData("AssociatedWithEmail", Element, "SchoolInformation");

            sie.OtherTitle = getNodeData("OtherTitle", Element, "SchoolInformation");
            sie.OtherName = getNodeData("OtherName", Element, "SchoolInformation");
            sie.OtherCellPhone = getNodeData("OtherCellPhone", Element, "SchoolInformation");
            sie.OtherEmail = getNodeData("OtherEmail", Element, "SchoolInformation");

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
            helper.AddElement("SchoolInformation", "ChancellorCellPhone", ScInfoEntity.ChancellorCellPhone);
            helper.AddElement("SchoolInformation", "ChancellorEmail", ScInfoEntity.ChancellorEmail);

            helper.AddElement("SchoolInformation", "EduDirectorName", ScInfoEntity.EduDirectorName );
            helper.AddElement("SchoolInformation", "EduDirectorCellPhone", ScInfoEntity.EduDirectorCellPhone);
            helper.AddElement("SchoolInformation", "EduDirectorEmail", ScInfoEntity.EduDirectorEmail);

            helper.AddElement("SchoolInformation", "StuDirectorName", ScInfoEntity.StuDirectorName);
            helper.AddElement("SchoolInformation", "StuDirectorCellPhone", ScInfoEntity.StuDirectorCellPhone);
            helper.AddElement("SchoolInformation", "StuDirectorEmail", ScInfoEntity.StuDirectorEmail);

            helper.AddElement("SchoolInformation", "AssociatedWithName", ScInfoEntity.AssociatedWithName);
            helper.AddElement("SchoolInformation", "AssociatedWithCellPhone", ScInfoEntity.AssociatedWithCellPhone);
            helper.AddElement("SchoolInformation", "AssociatedWithEmail", ScInfoEntity.AssociatedWithEmail);

            helper.AddElement("SchoolInformation", "OtherTitle", ScInfoEntity.OtherTitle);
            helper.AddElement("SchoolInformation", "OtherName", ScInfoEntity.OtherName);
            helper.AddElement("SchoolInformation", "OtherCellPhone", ScInfoEntity.OtherCellPhone);
            helper.AddElement("SchoolInformation", "OtherEmail", ScInfoEntity.OtherEmail);


            Config.SetSchoolInfo(helper.BaseElement);
            
            // 學年度學期
            DSXmlHelper helper1 = new DSXmlHelper("SetSystemConfigRequest");
            helper1.AddElement("SystemConfig");
            helper1.AddElement("SystemConfig", "DefaultSchoolYear",SchoolInfoEnt.SchoolYear);
            helper1.AddElement("SystemConfig", "DefaultSemester", SchoolInfoEnt.Semester);
            Config.SetSystemConfig(helper1.BaseElement);

        }

        private void btnSave_Click(object sender, EventArgs e)
        {

            StringBuilder sb_log = new StringBuilder();

            // 學校基本資料變更記錄
            if (SchoolInfoEnt.ChineseAddress != txtSchoolChsAddress.Text)
            {
                sb_log.AppendLine(string.Format("「{0}」已由「{1}」修改為「{2}」", "學校中文地址", SchoolInfoEnt.ChineseAddress, txtSchoolChsAddress.Text));
                SchoolInfoEnt.ChineseAddress = txtSchoolChsAddress.Text;
            }

            if (SchoolInfoEnt.ChinsesName != txtSchoolChsName.Text)
            {
                sb_log.AppendLine(string.Format("「{0}」已由「{1}」修改為「{2}」", "學校中文名稱", SchoolInfoEnt.ChinsesName, txtSchoolChsName.Text));
                SchoolInfoEnt.ChinsesName = txtSchoolChsName.Text;
            }

            if (SchoolInfoEnt.Code != txtSchoolCode.Text)
            {
                sb_log.AppendLine(string.Format("「{0}」已由「{1}」修改為「{2}」", "學校代碼", SchoolInfoEnt.Code, txtSchoolCode.Text));
                SchoolInfoEnt.Code = txtSchoolCode.Text;
            }

            if (SchoolInfoEnt.Telephone != txtPhone.Text)
            {
                sb_log.AppendLine(string.Format("「{0}」已由「{1}」修改為「{2}」", "學校電話", SchoolInfoEnt.Telephone, txtPhone.Text));
                SchoolInfoEnt.Telephone = txtPhone.Text;
            }

            if (SchoolInfoEnt.EnglishAddress != txtSchoolEngAddess.Text)
            {
                sb_log.AppendLine(string.Format("「{0}」已由「{1}」修改為「{2}」", "學校英文地址", SchoolInfoEnt.EnglishAddress, txtSchoolEngAddess.Text));
                SchoolInfoEnt.EnglishAddress = txtSchoolEngAddess.Text;
            }

            if (SchoolInfoEnt.EnglishName != txtSchoolEngName.Text)
            {
                sb_log.AppendLine(string.Format("「{0}」已由「{1}」修改為「{2}」", "學校英文名稱", SchoolInfoEnt.EnglishName, txtSchoolEngName.Text));
                SchoolInfoEnt.EnglishName = txtSchoolEngName.Text;
            }

            if (SchoolInfoEnt.Fax != txtFax.Text)
            {
                sb_log.AppendLine(string.Format("「{0}」已由「{1}」修改為「{2}」", "學校傳真", SchoolInfoEnt.Fax, txtFax.Text));
                SchoolInfoEnt.Fax = txtFax.Text;
            }

            if (SchoolInfoEnt.SchoolYear != intSchoolYear.Text)
            {
                sb_log.AppendLine(string.Format("「{0}」已由「{1}」修改為「{2}」", "學年度", SchoolInfoEnt.SchoolYear, intSchoolYear.Text));
                SchoolInfoEnt.SchoolYear = intSchoolYear.Text;
            }

            if (SchoolInfoEnt.Semester != intSemester.Text)
            {
                sb_log.AppendLine(string.Format("「{0}」已由「{1}」修改為「{2}」", "學期", SchoolInfoEnt.Semester, intSemester.Text));
                SchoolInfoEnt.Semester = intSemester.Text;
            }

            if (SchoolInfoEnt.ChancellorChsName != txtChancellorChsName.Text)
            {
                sb_log.AppendLine(string.Format("「{0}」已由「{1}」修改為「{2}」", "校長中文姓名", SchoolInfoEnt.ChancellorChsName, txtChancellorChsName.Text));
                SchoolInfoEnt.ChancellorChsName = txtChancellorChsName.Text;
            }

            if (SchoolInfoEnt.ChancellorEngName != txtChancellorEngName.Text)
            {
                sb_log.AppendLine(string.Format("「{0}」已由「{1}」修改為「{2}」", "校長英文姓名", SchoolInfoEnt.ChancellorEngName, txtChancellorEngName.Text));
                SchoolInfoEnt.ChancellorEngName = txtChancellorEngName.Text;
            }

            if (SchoolInfoEnt.ChancellorCellPhone != txtChancellorCellPhone.Text)
            {
                sb_log.AppendLine(string.Format("「{0}」已由「{1}」修改為「{2}」", "校長手機", SchoolInfoEnt.ChancellorCellPhone, txtChancellorCellPhone.Text));
                SchoolInfoEnt.ChancellorCellPhone = txtChancellorCellPhone.Text;
            }

            if (SchoolInfoEnt.ChancellorEmail != txtChancellorEmail.Text)
            {
                sb_log.AppendLine(string.Format("「{0}」已由「{1}」修改為「{2}」", "校長電子信箱", SchoolInfoEnt.ChancellorEmail, txtChancellorEmail.Text));
                SchoolInfoEnt.ChancellorEmail = txtChancellorEmail.Text;
            }

            if (SchoolInfoEnt.EduDirectorName != txtEduDirectorName.Text)
            {
                sb_log.AppendLine(string.Format("「{0}」已由「{1}」修改為「{2}」", "教務主任姓名", SchoolInfoEnt.EduDirectorName, txtEduDirectorName.Text));
                SchoolInfoEnt.EduDirectorName = txtEduDirectorName.Text;
            }

            if (SchoolInfoEnt.EduDirectorCellPhone != txtEduDirectorCellPhone.Text)
            {
                sb_log.AppendLine(string.Format("「{0}」已由「{1}」修改為「{2}」", "教務主任手機", SchoolInfoEnt.EduDirectorCellPhone, txtEduDirectorCellPhone.Text));
                SchoolInfoEnt.EduDirectorCellPhone = txtEduDirectorCellPhone.Text;
            }

            if (SchoolInfoEnt.EduDirectorEmail != txtEduDirectorEmail.Text)
            {
                sb_log.AppendLine(string.Format("「{0}」已由「{1}」修改為「{2}」", "教務主任電子信箱", SchoolInfoEnt.EduDirectorEmail, txtEduDirectorEmail.Text));
                SchoolInfoEnt.EduDirectorEmail = txtEduDirectorEmail.Text;
            }

            if (SchoolInfoEnt.StuDirectorName != txtStuDirectorName.Text)
            {
                sb_log.AppendLine(string.Format("「{0}」已由「{1}」修改為「{2}」", "學務主任姓名", SchoolInfoEnt.StuDirectorName, txtStuDirectorName.Text));
                SchoolInfoEnt.StuDirectorName = txtStuDirectorName.Text;
            }

            if (SchoolInfoEnt.StuDirectorCellPhone != txtStuDirectorCellPhone.Text)
            {
                sb_log.AppendLine(string.Format("「{0}」已由「{1}」修改為「{2}」", "學務主任手機", SchoolInfoEnt.StuDirectorCellPhone, txtStuDirectorCellPhone.Text));
                SchoolInfoEnt.StuDirectorCellPhone = txtStuDirectorCellPhone.Text;
            }

            if (SchoolInfoEnt.StuDirectorEmail != txtStuDirectorEmail.Text)
            {
                sb_log.AppendLine(string.Format("「{0}」已由「{1}」修改為「{2}」", "學務主任電子信箱", SchoolInfoEnt.StuDirectorEmail, txtStuDirectorEmail.Text));
                SchoolInfoEnt.StuDirectorEmail = txtStuDirectorEmail.Text;
            }

            if (SchoolInfoEnt.AssociatedWithName != txtAssociatedWithName.Text)
            {
                sb_log.AppendLine(string.Format("「{0}」已由「{1}」修改為「{2}」", "聯絡人姓名", SchoolInfoEnt.AssociatedWithName, txtAssociatedWithName.Text));
                SchoolInfoEnt.AssociatedWithName = txtAssociatedWithName.Text;
            }

            if (SchoolInfoEnt.AssociatedWithCellPhone != txtAssociatedWithCellPhone.Text)
            {
                sb_log.AppendLine(string.Format("「{0}」已由「{1}」修改為「{2}」", "聯絡人手機", SchoolInfoEnt.AssociatedWithCellPhone, txtAssociatedWithCellPhone.Text));
                SchoolInfoEnt.AssociatedWithCellPhone = txtAssociatedWithCellPhone.Text;
            }

            if (SchoolInfoEnt.AssociatedWithEmail != txtAssociatedWithEmail.Text)
            {
                sb_log.AppendLine(string.Format("「{0}」已由「{1}」修改為「{2}」", "聯絡人電子信箱", SchoolInfoEnt.AssociatedWithEmail, txtAssociatedWithEmail.Text));
                SchoolInfoEnt.AssociatedWithEmail = txtAssociatedWithEmail.Text;
            }

            if (SchoolInfoEnt.OtherTitle != txtOtherTitle.Text)
            {
                sb_log.AppendLine(string.Format("「{0}」已由「{1}」修改為「{2}」", "其他職稱", SchoolInfoEnt.OtherTitle, txtOtherTitle.Text));
                SchoolInfoEnt.OtherTitle = txtOtherTitle.Text;
            }

            if (SchoolInfoEnt.OtherName != txtOtherName.Text)
            {
                sb_log.AppendLine(string.Format("「{0}」已由「{1}」修改為「{2}」", "其他姓名", SchoolInfoEnt.OtherName, txtOtherName.Text));
                SchoolInfoEnt.OtherName = txtOtherName.Text;
            }

            if (SchoolInfoEnt.OtherCellPhone != txtOtherCellPhone.Text)
            {
                sb_log.AppendLine(string.Format("「{0}」已由「{1}」修改為「{2}」", "其他手機", SchoolInfoEnt.OtherCellPhone, txtOtherCellPhone.Text));
                SchoolInfoEnt.OtherCellPhone = txtOtherCellPhone.Text;
            }

            if (SchoolInfoEnt.OtherEmail != txtOtherEmail.Text)
            {
                sb_log.AppendLine(string.Format("「{0}」已由「{1}」修改為「{2}」", "其他電子信箱", SchoolInfoEnt.OtherEmail, txtOtherEmail.Text));
                SchoolInfoEnt.OtherEmail = txtOtherEmail.Text;
            }


            setSchoolInfoData(SchoolInfoEnt);
            MessageBox.Show("資料儲存完成");


            FISCA.LogAgent.ApplicationLog.Log("管理學校基本資料", "修改", sb_log.ToString());

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
            txtChancellorCellPhone.Text = SchoolInfoEnt.ChancellorCellPhone;
            txtChancellorEmail.Text = SchoolInfoEnt.ChancellorEmail;

            txtEduDirectorName.Text = SchoolInfoEnt.EduDirectorName;
            txtEduDirectorCellPhone.Text = SchoolInfoEnt.EduDirectorCellPhone;
            txtEduDirectorEmail.Text = SchoolInfoEnt.EduDirectorEmail;

            txtStuDirectorName.Text = SchoolInfoEnt.StuDirectorName;
            txtStuDirectorCellPhone.Text = SchoolInfoEnt.StuDirectorCellPhone;
            txtStuDirectorEmail.Text = SchoolInfoEnt.StuDirectorEmail;

            txtAssociatedWithName.Text = SchoolInfoEnt.AssociatedWithName;
            txtAssociatedWithCellPhone.Text = SchoolInfoEnt.AssociatedWithCellPhone;
            txtAssociatedWithEmail.Text = SchoolInfoEnt.AssociatedWithEmail;

            txtOtherTitle.Text = SchoolInfoEnt.OtherTitle;
            txtOtherName.Text = SchoolInfoEnt.OtherName;
            txtOtherCellPhone.Text = SchoolInfoEnt.OtherCellPhone;
            txtOtherEmail.Text = SchoolInfoEnt.OtherEmail;


            txtFax.Text = SchoolInfoEnt.Fax;
            txtPhone.Text = SchoolInfoEnt.Telephone;
            txtSchoolChsAddress.Text = SchoolInfoEnt.ChineseAddress;
            txtSchoolChsName.Text = SchoolInfoEnt.ChinsesName;
            txtSchoolCode.Text = SchoolInfoEnt.Code;
            txtSchoolEngAddess.Text = SchoolInfoEnt.EnglishAddress;
            txtSchoolEngName.Text = SchoolInfoEnt.EnglishName;
            if(!string.IsNullOrEmpty(SchoolInfoEnt.SchoolYear))
                intSchoolYear.Text = SchoolInfoEnt.SchoolYear;

            if(!string.IsNullOrEmpty(SchoolInfoEnt.Semester))
                intSemester.Text = SchoolInfoEnt.Semester;
        }
    }
    public class SchoolInfoEntity
    {
         /// <summary>
         /// 學校代碼
         /// </summary>
         public string Code { get; set; }

         /// <summary>
         /// 學年度
         /// </summary>
         public string SchoolYear { get; set; }

         /// <summary>
         /// 學期
         /// </summary>
         public string Semester { get; set; }

         /// <summary>
         /// 學校名稱
         /// </summary>
         public string ChinsesName { get; set; }

         /// <summary>
         /// 學校英文名稱
         /// </summary>
         public string EnglishName { get; set; }

         /// <summary>
         /// 學校地址
         /// </summary>
         public string ChineseAddress { get; set; }

         /// <summary>
         /// 學校英文地址
         /// </summary>
         public string EnglishAddress { get; set; }

         /// <summary>
         /// 電話
         /// </summary>
         public string Telephone { get; set; }

         /// <summary>
         /// 傳真
         /// </summary>
         public string Fax { get; set; }

         #region 學校主管

         /// <summary>
         /// 校長中文姓名
         /// </summary>
         public string ChancellorChsName { get; set; }

         /// <summary>
         /// 校長英文姓名
         /// </summary>
         public string ChancellorEngName { get; set; }

         /// <summary>
         /// 校長手機
         /// </summary>
         public string ChancellorCellPhone { get; set; }

         /// <summary>
         /// 校長Emai
         /// </summary>
         public string ChancellorEmail { get; set; }

         /// <summary>
         /// 教務主任姓名
         /// </summary>
         public string EduDirectorName { get; set; }

         /// <summary>
         /// 教務主任手機
         /// </summary>
         public string EduDirectorCellPhone { get; set; }

         /// <summary>
         /// 教務主任Email
         /// </summary>
         public string EduDirectorEmail { get; set; }

         /// <summary>
         /// 學務主任姓名
         /// </summary>
         public string StuDirectorName { get; set; }

         /// <summary>
         /// 學務主任手機
         /// </summary>
         public string StuDirectorCellPhone { get; set; }

         /// <summary>
         /// 學務主任Email
         /// </summary>
         public string StuDirectorEmail { get; set; } 

         #endregion

         /// <summary>
         /// 資訊連絡人
         /// </summary>
         public string AssociatedWithName { get; set; }

         /// <summary>
         /// 資訊連絡人手機
         /// </summary>
         public string AssociatedWithCellPhone { get; set; }

         /// <summary>
         /// 資訊連絡人Email
         /// </summary>
         public string AssociatedWithEmail { get; set; }

         /// <summary>
         /// 其它名稱
         /// </summary>
         public string OtherTitle { get; set; }

         /// <summary>
         /// 其它姓名
         /// </summary>
         public string OtherName { get; set; }

         /// <summary>
         /// 其它手機
         /// </summary>
         public string OtherCellPhone { get; set; }

         /// <summary>
         /// 其它Email
         /// </summary>
         public string OtherEmail { get; set; }
    }
}

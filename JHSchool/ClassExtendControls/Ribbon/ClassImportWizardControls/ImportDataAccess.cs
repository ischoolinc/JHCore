using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using SmartSchool.Feature.Class;
using SmartSchool.Feature.Teacher;
using FISCA.DSAUtil;
using JHSchool.Legacy.ImportSupport;

namespace JHSchool.ClassExtendControls.Ribbon.ClassImportWizardControls
{
    internal class ImportDataAccess : IDataAccess
    {
        public XmlElement GetImportFieldList()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(Properties.Resources.JH_C_BulkDescription);
            return doc.DocumentElement;
            //return SmartSchool.Feature.Class.ClassBulkProcess.GetImportFieldList();
        }

        public XmlElement GetValidateFieldRule()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(Properties.Resources.JH_C_ImportValidatorRule);
            return doc.DocumentElement;
            //return SmartSchool.Feature.Class.ClassBulkProcess.GetValidateFieldRule();
        }

        public XmlElement GetUniqueFieldData()
        {
            return SmartSchool.Feature.Class.ClassBulkProcess.GetUniqueFieldData();
        }

        public XmlElement GetShiftCheckList(params string[] fieldNameList)
        {
            return SmartSchool.Feature.Class.ClassBulkProcess.GetShiftCheckList(fieldNameList);
        }

        public void InsertImportData(XmlElement data)
        {
            SmartSchool.Feature.Class.ClassBulkProcess.InsertImportData(data);
        }

        public void UpdateImportData(XmlElement data)
        {
            SmartSchool.Feature.Class.ClassBulkProcess.UpdateImportData(data);
        }
    }
}

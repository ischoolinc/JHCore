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
            return SmartSchool.Feature.Class.ClassBulkProcess.GetImportFieldList();
        }

        public XmlElement GetValidateFieldRule()
        {
            return SmartSchool.Feature.Class.ClassBulkProcess.GetValidateFieldRule();
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

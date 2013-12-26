using System;
using System.Collections.Generic;
using System.Text;
using DocValidate;

namespace JHSchool.StudentExtendControls.Ribbon.StudentImportWizardControls.ValidateModel
{
    class EnrollmentRowValidator : IRowVaildator
    {
        public EnrollmentRowValidator(string primaryKey)
        {
        }

        #region IRowVaildator 成員

        public string Correct(IRowSource Value)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void InitFromXMLNode(System.Xml.XmlElement XmlNode)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void InitFromXMLString(string XmlString)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public string KeyField()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public string ToString(string Description)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool Validate(IRowSource Value)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}

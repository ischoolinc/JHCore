using System;
using System.Collections.Generic;
using System.Text;
using DocValidate;

namespace JHSchool.StudentExtendControls.Ribbon.StudentImportWizardControls.ValidateModel
{
    class RowValidatorFactory : IRowValidatorFactory
    {
        private string _primary_key;

        public RowValidatorFactory(string primaryKey)
        {
            _primary_key = primaryKey;
        }

        #region IRowValidatorFactory 成員

        public IRowVaildator CreateRowValidator(string TypeName)
        {
            switch (TypeName.ToUpper())
            {
                case "ENROLLMENTROWVALIDATOR":
                    return new EnrollmentRowValidator(_primary_key);
            }

            return null;
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace JHSchool.Legacy.ImportSupport.Validators
{
    internal class CommentValidatorFactory : IValidatorFactory
    {
        private WizardContext _context;

        public CommentValidatorFactory(WizardContext context)
        {
            _context = context;
        }

        #region IFieldValidatorFactory 成員

        public DocValidate.IFieldValidator CreateFieldValidator(string TypeName)
        {
            if (TypeName.ToUpper() == "ClassLookup".ToUpper())
                return new ClassLookupFieldValidator(_context);
            else if (TypeName.ToUpper() == "TeacherLookup".ToUpper())
                return new TeacherLookupFieldValidator(_context);
            else if (TypeName.ToUpper() == "TemplateLookup".ToUpper())
                return new TemplateLookupFieldValidator(_context);
            //else if (TypeName.ToUpper() == "DeptLookup".ToUpper())
            //    return new DeptLookupFieldValidator(_context);
            else if (TypeName.ToUpper() == "GPLookup".ToUpper())
                return new GPLookupFieldValidator(_context);
            else if (TypeName.ToUpper() == "SCRLookup".ToUpper())
                return new SCRLookupFieldValidator(_context);
            else
                return null;
        }

        #endregion

        #region IRowValidatorFactory 成員

        private UpdateUniqueRowValidator _update_uniq;
        public UpdateUniqueRowValidator UpdateUnique
        {
            get { return _update_uniq; }
            set { _update_uniq = value; }
        }

        public DocValidate.IRowVaildator CreateRowValidator(string TypeName)
        {
            if (TypeName.ToUpper() == "InsertDBUnique".ToUpper())
                return new InsertDBUniqueRowValidator(_context);

            else if (TypeName.ToUpper() == "ShiftCheck".ToUpper())
                return new ShiftCheckRowValidator(_context);

            else if (TypeName.ToUpper() == "UpdateIdentify".ToUpper())
                return new UpdateIdentifyRowValidator(_context);

            else if (TypeName.ToUpper() == "UpdateUnique".ToUpper())
                return UpdateUnique;

            else if (TypeName.ToUpper() == "InsertSheetUnique".ToUpper())
                return new InsertSheetUniqueRowValidator(_context);

            else
                return null;
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace JHSchool.StudentExtendControls.Ribbon.StudentImportWizardControls.ValidateModel
{
    public class ValidateColumnCollection : Dictionary<string, ValidateColumn>
    {
        public string[] GetNames()
        {
            List<string> names = new List<string>(Keys);
            return names.ToArray();
        }
    }
}

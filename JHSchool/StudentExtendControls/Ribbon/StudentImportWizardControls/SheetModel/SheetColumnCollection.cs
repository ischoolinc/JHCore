using System;
using System.Collections.Generic;
using System.Text;

namespace JHSchool.StudentExtendControls.Ribbon.StudentImportWizardControls.SheetModel
{
    public class SheetColumnCollection : Dictionary<string, SheetColumn>
    {
        public string[] GetNames()
        {
            List<string> names = new List<string>(Keys);
            return names.ToArray();
        }
    }
}

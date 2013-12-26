using System;
using System.Collections.Generic;
using System.Text;


namespace JHSchool.Legacy.ImportSupport
{
    public class WizardContext
    {
        public WizardContext()
        {
            _support_fields = new ImportFieldCollection();
            _conditions = new List<ImportCondition>();
            _selected_fields = new List<string>();
            _exts = new Dictionary<string, object>();
        }

        private string _source_file;

        public string SourceFile
        {
            get { return _source_file; }
            set { _source_file = value; }
        }

        private ImportMode _mode = ImportMode.None;

        public ImportMode CurrentMode
        {
            get { return _mode; }
            set { _mode = value; }
        }

        private ImportCondition _identify_field = null;

        public ImportCondition IdentifyField
        {
            get { return _identify_field; }
            set { _identify_field = value; }
        }

        private IDataAccess _data_source;

        public IDataAccess DataSource
        {
            get { return _data_source; }
            set { _data_source = value; }
        }

        private ImportField _shift_check_field = null;

        public ImportField ShiftCheckField
        {
            get { return _shift_check_field; }
            set { _shift_check_field = value; }
        }

        private ImportFieldCollection _support_fields;

        public ImportFieldCollection SupportFields
        {
            get { return _support_fields; }
            set { _support_fields = value; }
        }

        private List<ImportCondition> _conditions;
        public List<ImportCondition> UpdateConditions
        {
            get { return _conditions; }
            set { _conditions = value; }
        }

        private List<string> _selected_fields;
        public List<string> SelectedFields
        {
            get { return _selected_fields; }
        }

        private Dictionary<string, object> _exts;
        public Dictionary<string, object> Extensions
        {
            get { return _exts; }
        }

    }
}

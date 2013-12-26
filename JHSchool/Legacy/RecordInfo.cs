using System;
using System.Collections.Generic;
using System.Text;

namespace JHSchool.Legacy
{
    class RecordInfo
    {
        private bool _isAddedRow;

        public bool IsAddedRow
        {
            get { return _isAddedRow; }
            set { _isAddedRow = value; }
        }
        private string _id;

        public string ID
        {
            get { return _id; }
            set { _id = value; }
        }
        private string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
    }
}

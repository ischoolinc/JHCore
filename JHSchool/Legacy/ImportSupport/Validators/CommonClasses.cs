using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using DocValidate;

namespace JHSchool.Legacy.ImportSupport.Validators
{
    public class Record : Dictionary<string, string>
    {
        public Record(XmlElement data)
        {
            foreach (XmlAttribute each in data.Attributes)
                Add(each.LocalName, each.Value);
        }

        public Record(IRowSource rowSource, IEnumerable<string> fields)
        {
            foreach (string field in fields)
                Add(field, rowSource.GetFieldData(field));
        }

        private int _source_row_index = -1;

        public int SourceRowIndex
        {
            get { return _source_row_index; }
            set { _source_row_index = value; }
        }
    }

    public class RecordSet : IEnumerable<Record>
    {
        private List<Record> _records;

        public RecordSet()
        {
            _records = new List<Record>();
        }

        public void AddRecord(Record record)
        {
            _records.Add(record);
        }

        #region IEnumerable<Record> 成員

        public IEnumerator<Record> GetEnumerator()
        {
            return _records.GetEnumerator();
        }

        #endregion

        #region IEnumerable 成員

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _records.GetEnumerator();
        }

        #endregion
    }

    public class DuplicateInfo
    {
        public DuplicateInfo(ImportCondition cond, Record record)
        {
            _condition = cond;
            _record = record;
        }

        private ImportCondition _condition;
        public ImportCondition Condition
        {
            get { return _condition; }
        }

        private Record _record;
        public Record Record
        {
            get { return _record; }
        }

    }

    public class ConditionKeySet
    {
        private ImportCondition _condition;
        private Dictionary<string, Record> _records;
        private bool _exclude_empty_key;

        public ConditionKeySet(ImportCondition condition, RecordSet records, bool excludeEmptyKey)
        {
            _records = new Dictionary<string, Record>();
            _condition = condition;
            _exclude_empty_key = excludeEmptyKey;

            foreach (Record each in records)
                AddRecord(each);
        }

        public ConditionKeySet(ImportCondition condition, RecordSet record)
            : this(condition, record, false)
        {
        }

        public ConditionKeySet(ImportCondition condition, bool excludeEmptyKey)
        {
            _records = new Dictionary<string, Record>();
            _condition = condition;
            _exclude_empty_key = excludeEmptyKey;
        }

        public ConditionKeySet(ImportCondition condition)
            : this(condition, false)
        {
        }

        /// <summary>
        /// 使用 InternalName。
        /// </summary>
        public void AddRecord(Record record)
        {
            if (_exclude_empty_key)
            {
                if (!_condition.IsEmptyKey(record))
                    AddRecordByInternalName(record);
            }
            else
                AddRecordByInternalName(record);
        }

        private void AddRecordByInternalName(Record record)
        {
            string key = _condition.GetKeyByInternalName(record);

            if (!_records.ContainsKey(key)) _records.Add(key, record);
        }

        /// <summary>
        /// 使用 FieldName。
        /// </summary>
        public void AddRecord(IRowSource rowSource, IEnumerable<string> fields)
        {
            if (_exclude_empty_key)
            {
                if (!_condition.IsEmptyKey(rowSource))
                    AddRecordByFieldName(rowSource, fields);
            }
            else
                AddRecordByFieldName(rowSource, fields);
        }

        private void AddRecordByFieldName(IRowSource rowSource, IEnumerable<string> fields)
        {
            Record record = new Record(rowSource, fields);

            string key = _condition.GetKeyByFieldName(rowSource);

            if (!_records.ContainsKey(key)) _records.Add(key, record);
        }

        public ImportCondition Condition
        {
            get { return _condition; }
        }

        /// <summary>
        /// 判斷資料是否已存在。
        /// </summary>
        /// <param name="rowSource"></param>
        /// <returns></returns>
        public bool Contains(IRowSource rowSource)
        {
            return _records.ContainsKey(Condition.GetKeyByFieldName(rowSource));
        }

        public Record GetRecord(IRowSource rowSource)
        {
            return _records[Condition.GetKeyByFieldName(rowSource)];
        }
    }

    public class ConditionKeySetCollection
    {
        private Dictionary<ImportCondition, ConditionKeySet> _key_set_list;
        private RecordSet _all_record;

        public ConditionKeySetCollection(List<ImportCondition> conditions, XmlElement data, string recordName)
        {
            RecordSet records = new RecordSet();
            _key_set_list = new Dictionary<ImportCondition, ConditionKeySet>();

            foreach (XmlElement each in data.SelectNodes(recordName))
                records.AddRecord(new Record(each));

            InitialKeySet(conditions, records);
        }

        public ConditionKeySetCollection(List<ImportCondition> conditions)
        {
            _all_record = new RecordSet();
            _key_set_list = new Dictionary<ImportCondition, ConditionKeySet>();

            foreach (ImportCondition each in conditions)
                _key_set_list.Add(each, new ConditionKeySet(each, each.EmptySkipValidate));
        }

        private void InitialKeySet(List<ImportCondition> conditions, RecordSet records)
        {
            foreach (ImportCondition each in conditions)
                _key_set_list.Add(each, new ConditionKeySet(each, records, each.EmptySkipValidate));

            _all_record = records;
        }

        public RecordSet AllRecord
        {
            get { return _all_record; }
        }

        public void AddRecord(Record record)
        {
            foreach (ConditionKeySet each in _key_set_list.Values)
                each.AddRecord(record);
        }

        public void AddRecord(IRowSource rowSource, IEnumerable<string> fields)
        {
            foreach (ConditionKeySet each in _key_set_list.Values)
                each.AddRecord(rowSource, fields);
        }

        public List<DuplicateInfo> GetDuplicate(IRowSource rowSource)
        {
            List<DuplicateInfo> duplicates = new List<DuplicateInfo>();
            foreach (ConditionKeySet each in _key_set_list.Values)
            {
                if (each.Contains(rowSource))
                    duplicates.Add(new DuplicateInfo(each.Condition, each.GetRecord(rowSource)));
            }

            return duplicates;
        }

        public DuplicateInfo GetDuplicateBy(ImportCondition cond, IRowSource rowSource)
        {
            if (!_key_set_list.ContainsKey(cond))
                return null;

            ConditionKeySet keyset = _key_set_list[cond];

            if (keyset.Contains(rowSource))
                return new DuplicateInfo(keyset.Condition, keyset.GetRecord(rowSource));
            else
                return null;
        }
    }
}

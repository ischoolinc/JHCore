using System;
using System.Collections.Generic;
using System.Text;
using DocValidate;
using System.Xml;
using FISCA.DSAUtil;

namespace JHSchool.Legacy.ImportSupport.Validators
{
    internal class UpdateUniqueRowValidator : IRowVaildator
    {
        private SheetHelper _sheet;
        private ConditionKeySetCollection _key_sets;
        private ImportCondition _primary_condition;
        private List<ImportCondition> _check_conditions;
        private bool _skip_validate;

        public UpdateUniqueRowValidator(WizardContext context, SheetHelper sheet)
        {
            //在 Insert 模式不進行此類型驗證。
            _skip_validate = (context.CurrentMode != ImportMode.Update);

            if (_skip_validate) return;

            _sheet = sheet;
            _primary_condition = context.IdentifyField;
            _check_conditions = new List<ImportCondition>();

            List<ImportCondition> conditions = new List<ImportCondition>();
            foreach (ImportCondition each in context.UpdateConditions)
            {
                if (each.ContainsAllField(context.SelectedFields.ToArray()))
                    conditions.Add(each);
            }

            _check_conditions = conditions;

            if (!conditions.Contains(context.IdentifyField))
                conditions.Add(context.IdentifyField); //連同 Primary Condition 一起加入。

            XmlElement recordData = context.DataSource.GetUniqueFieldData();

            _key_sets = new ConditionKeySetCollection(conditions, recordData, "Record");
        }

        #region IRowVaildator 成員

        public void InitFromXMLNode(XmlElement XmlNode)
        {
        }

        public void InitFromXMLString(string XmlString)
        {
            throw new Exception("The method or operation is not implemented.(InitFromXMLString)");
        }

        public string Correct(IRowSource Value)
        {
            throw new Exception("The method or operation is not implemented.(Correct)");
        }

        public string KeyField()
        {
            return "<XmlContent>";
        }

        public string ToString(string Description)
        {
            return string.Empty;
        }

        public bool Validate(IRowSource Value)
        {
            //格過驗證。
            if (_skip_validate) return true;

            int rowIndex = -1;
            SheetRowSource sheetRow = Value as SheetRowSource;
            if (sheetRow != null) rowIndex = sheetRow.CurrentRowIndex;

            DuplicateInfo dup = _key_sets.GetDuplicateBy(_primary_condition, Value);

            if (dup != null)
            {
                Record record = dup.Record;
                record.SourceRowIndex = rowIndex;

                foreach (ImportCondition each in _check_conditions)
                {
                    foreach (ImportField eachField in each.Fields)
                        record[eachField.InternalName] = Value.GetFieldData(eachField.FieldName);
                }
            }
            return true;
        }
        #endregion

        public CellCommentManager CheckUpdateResult()
        {
            CellCommentManager comments = new CellCommentManager();

            foreach (ImportCondition eachCond in _check_conditions)
            {
                if (eachCond == _primary_condition) continue;

                Dictionary<string, Record> records = new Dictionary<string, Record>();

                foreach (Record each in _key_sets.AllRecord)
                {
                    if (eachCond.EmptySkipValidate) //檢查是否要略過驗證空白資料。
                        if (eachCond.IsEmptyKey(each)) //如果是空白資料的話就 Skip。
                            continue;

                    string key = eachCond.GetKeyByInternalName(each);

                    //Console.WriteLine(each["CourseID"] + "    " + key);

                    if (!records.ContainsKey(key))
                        records.Add(key, each);
                    else
                    {
                        int rowIndex = each.SourceRowIndex;

                        if (rowIndex < 0)
                        {
                            rowIndex = records[key].SourceRowIndex;
                            if (rowIndex < 0) continue;
                        }

                        foreach (ImportField eachField in eachCond.Fields)
                        {
                            int columnIndex = _sheet.GetFieldIndex(eachField.FieldName);

                            string msg = string.Format("更新資料後會造成欄位「{0}」與資料庫中資料重複。", eachCond.GetCombineFieldName());
                            comments.WriteError(rowIndex, columnIndex, msg);
                        }
                    }
                }
            }

            return comments;
        }
    }
}

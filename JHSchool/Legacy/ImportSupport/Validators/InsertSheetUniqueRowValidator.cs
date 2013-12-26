using System;
using System.Collections.Generic;
using System.Text;
using DocValidate;
using System.Xml;
using FISCA.DSAUtil;

namespace JHSchool.Legacy.ImportSupport.Validators
{
    internal class InsertSheetUniqueRowValidator : IRowVaildator
    {
        private IEnumerable<string> _selected_fields;
        private ConditionKeySetCollection _key_sets;
        private bool _skip_validate;

        public InsertSheetUniqueRowValidator(WizardContext context)
        {
            //在 Update 模式不進行此類型驗證。
            _skip_validate = (context.CurrentMode != ImportMode.Insert);

            if (_skip_validate) return;

            _selected_fields = context.SelectedFields;

            List<ImportCondition> conditions = new List<ImportCondition>();
            foreach (ImportCondition each in context.UpdateConditions)
            {
                if (each.ContainsAllField(context.SelectedFields.ToArray()))
                    conditions.Add(each);
            }

            _key_sets = new ConditionKeySetCollection(conditions);

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

        private List<DuplicateInfo> _prevous_result;

        public string ToString(string Description)
        {
            DSXmlHelper result = new DSXmlHelper("Fields");

            foreach (DuplicateInfo each in _prevous_result)
            {
                ImportCondition cond = each.Condition;
                string combineName = cond.GetCombineFieldName();
                foreach (ImportField eachField in cond.Fields)
                {
                    XmlElement elmField = result.AddElement("Field");
                    elmField.SetAttribute("Name", eachField.FieldName);
                    elmField.SetAttribute("Description", string.Format("欄位「{0}」資料在工作表中重複。", combineName));
                }
            }

            return result.GetRawXml();
        }

        public bool Validate(IRowSource Value)
        {
            //格過驗證。
            if (_skip_validate) return true;

            List<DuplicateInfo> dups = _key_sets.GetDuplicate(Value);
            _prevous_result = dups;

            _key_sets.AddRecord(Value, _selected_fields);
            
            return dups.Count <= 0;
        }

        #endregion
    }
}

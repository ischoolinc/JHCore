using System;
using System.Collections.Generic;
using System.Text;
using DocValidate;
using System.Xml;
using FISCA.DSAUtil;

namespace JHSchool.Legacy.ImportSupport.Validators
{
    internal class InsertDBUniqueRowValidator : IRowVaildator
    {
        private ConditionKeySetCollection _key_sets;
        private bool _skip_validate;

        public InsertDBUniqueRowValidator(WizardContext context)
        {
            //在 Update 模式不進行此類型驗證。
            _skip_validate = (context.CurrentMode != ImportMode.Insert);

            if (_skip_validate) return;

            List<ImportCondition> conditions = new List<ImportCondition>();
            foreach (ImportCondition each in context.UpdateConditions)
            {
                if (each.ContainsAllField(context.SelectedFields.ToArray()))
                    conditions.Add(each);
            }

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
                    elmField.SetAttribute("Description", string.Format("欄位「{0}」與資料庫中資料重複。", combineName));
                }
            }

            return result.GetRawXml();
        }

        public bool Validate(IRowSource Value)
        {
            //格過驗證。
            if (_skip_validate) return true;

            List<DuplicateInfo> conds = _key_sets.GetDuplicate(Value);
            _prevous_result = conds;

            return conds.Count <= 0;
        }

        #endregion
    }
}

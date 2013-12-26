using System;
using System.Collections.Generic;
using System.Text;
using DocValidate;
using System.Xml;
using FISCA.DSAUtil;

namespace JHSchool.Legacy.ImportSupport.Validators
{
    /// <summary>
    /// 此類別驗證更新條件是否存在於資料庫中。
    /// </summary>
    internal class UpdateIdentifyRowValidator : IRowVaildator
    {
        private bool _skip_validate;
        private ImportCondition _cond;
        private ConditionKeySetCollection _key_sets;

        public UpdateIdentifyRowValidator(WizardContext context)
        {
            //當不是 Update 模式時不驗證。
            _skip_validate = (context.CurrentMode != ImportMode.Update);

            if (_skip_validate) return;

            _cond = context.IdentifyField;

            List<ImportCondition> conditions = new List<ImportCondition>();
            conditions.Add(context.IdentifyField);

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
            DSXmlHelper result = new DSXmlHelper("Fields");

            ImportCondition cond = _cond;
            string combineName = cond.GetCombineFieldName();
            foreach (ImportField eachField in cond.Fields)
            {
                XmlElement elmField = result.AddElement("Field");
                elmField.SetAttribute("Name", eachField.FieldName);
                elmField.SetAttribute("Description", string.Format("欄位「{0}」為識別欄，值必須存在於資料庫中。", combineName));
            }

            return result.GetRawXml();
        }

        public bool Validate(IRowSource Value)
        {
            //略過驗證。
            if (_skip_validate) return true;

            List<DuplicateInfo> dups = _key_sets.GetDuplicate(Value);

            return dups.Count > 0;
        }
        #endregion
    }
}
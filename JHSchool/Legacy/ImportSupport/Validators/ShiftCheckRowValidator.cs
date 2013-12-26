using System;
using System.Collections.Generic;
using System.Text;
using DocValidate;
using System.Xml;
using FISCA.DSAUtil;

namespace JHSchool.Legacy.ImportSupport.Validators
{
    internal class ShiftCheckRowValidator : IRowVaildator
    {
        private bool _skip_validate;
        private ImportField _check_field;
        private ConditionKeySetCollection _key_sets;

        public ShiftCheckRowValidator(WizardContext context)
        {
            //未指定 CheckField 時不進行驗證。
            _skip_validate = (context.ShiftCheckField == null);

            if (_skip_validate) return;

            _check_field = context.ShiftCheckField;

            List<string> requireFields = new List<string>();
            requireFields.AddRange(context.IdentifyField.Fields.ToInternalNames());
            requireFields.Add(context.ShiftCheckField.InternalName);

            XmlElement recordData = context.DataSource.GetShiftCheckList(requireFields.ToArray());

            List<ImportCondition> cond = new List<ImportCondition>();
            cond.Add(context.IdentifyField);

            _key_sets = new ConditionKeySetCollection(cond, recordData, "Record");

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
            if (_check_field != null)
                return _check_field.FieldName;
            else
                return string.Empty;
        }

        public string ToString(string Description)
        {
            string dbValue = _previous_result.Record[_check_field.InternalName];

            string msg;
            if (string.IsNullOrEmpty(dbValue))
                msg = string.Format("驗證欄驗證失敗，此欄位的資料應空白。");
            else
                msg = string.Format("驗證欄驗證失敗，此欄位的資料應為「{0}」。", dbValue);

            return msg;
        }

        private DuplicateInfo _previous_result;

        public bool Validate(IRowSource Value)
        {
            //格過驗證。
            if (_skip_validate) return true;

            List<DuplicateInfo> dups = _key_sets.GetDuplicate(Value);

            if (dups.Count <= 0)
                return true;
            else
            {
                DuplicateInfo dup = dups[0];
                _previous_result = dup;

                string serverValue = dup.Record[_check_field.InternalName].Trim();
                string sourceValue = Value.GetFieldData(_check_field.FieldName).Trim();

                return (serverValue == sourceValue);
            }
        }
        #endregion
    }
}

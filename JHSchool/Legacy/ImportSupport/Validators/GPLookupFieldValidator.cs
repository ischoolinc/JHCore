using System;
using System.Collections.Generic;
using System.Text;
using DocValidate;
using System.Xml;
using JHSchool.Legacy.ImportSupport.Lookups;

namespace JHSchool.Legacy.ImportSupport.Validators
{
    internal class GPLookupFieldValidator : IFieldValidator
    {
        private bool _skip_empty;
        private bool _activate_validator;
        private GPLookup _lookup;
        private WizardContext _context;

        public GPLookupFieldValidator(WizardContext context)
        {
            _context = context;
        }

        #region IFieldValidator 成員

        public string Correct(string Value)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void InitFromXMLNode(System.Xml.XmlElement XmlNode)
        {
            _skip_empty = bool.Parse(XmlNode.GetAttribute("SkipEmpty"));

            _activate_validator = false;
            foreach (XmlElement each in XmlNode.SelectNodes("ActivatorField"))
            {
                string fieldName = each.InnerText;
                _activate_validator |= (_context.SelectedFields.Contains(fieldName));
            }

            if (!_activate_validator) return;

            _lookup = _context.Extensions[GPLookup.Name] as GPLookup;
        }

        public void InitFromXMLString(string XmlString)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public string ToString(string Description)
        {
            return Description;
        }

        public bool Validate(string Value)
        {
            if (!_activate_validator) return true;

            if (string.IsNullOrEmpty(Value.Trim()))
                if (_skip_empty) return true;

            return _lookup.Contains(Value);
        }

        #endregion
    }
}

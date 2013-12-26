using System;
using System.Collections.Generic;
using System.Text;
using JHSchool.Legacy.Export.RequestHandler;
using JHSchool.Legacy.Export.ResponseHandler;

namespace JHSchool.Legacy.Export.Util
{
    public class FieldUtil
    {
        public static FieldCollection Match(FieldCollection ruleFields, FieldCollection selectedFields)
        {
            FieldCollection collection = new FieldCollection();
            foreach (Field field in ruleFields)
            {
                foreach (Field sField in selectedFields)
                {
                    if (sField.FieldName != field.FieldName) continue;
                    collection.Add(field);
                }
            }
            return collection;
        }

        public static ExportFieldCollection Match(ExportFieldCollection ruleFields, FieldCollection selectedFields)
        {
            ExportFieldCollection collection = new ExportFieldCollection();
            foreach (ExportField field in ruleFields)
            {
                foreach (Field sField in selectedFields)
                {
                    if (sField.FieldName != field.RequestName) continue;
                    collection.Add(field);
                }
            }
            return collection;
        }
    }
}

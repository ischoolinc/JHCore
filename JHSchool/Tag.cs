using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA.DSAUtil;
using FISCA.Authentication;
using Framework;

namespace JHSchool
{
    public class Tag : CacheManager<TagRecord>
    {
        private static Tag _instance;

        public static Tag Instance
        {
            get
            {
                if (_instance == null) _instance = new Tag();
                return _instance;
            }
        }

        private const string ServiceName = "SmartSchool.Tag.GetDetailList";

        protected override Dictionary<string, TagRecord> GetAllData()
        {
            DSXmlHelper helper = new DSXmlHelper("Request");
            helper.AddElement("Field");
            helper.AddElement("Field", "All");

            DSRequest dsreq = new DSRequest(helper);
            Dictionary<string, TagRecord> result = new Dictionary<string, TagRecord>();
            string srvname = ServiceName;
            foreach (var item in DSAServices.CallService(srvname, dsreq).GetContent().GetElements("Tag"))
            {
                TagRecord tag = new TagRecord(item);
                result.Add(tag.ID, tag);
            }
            return result;
        }

        protected override Dictionary<string, TagRecord> GetData(IEnumerable<string> primaryKeys)
        {
            bool execute_required = false;
            DSXmlHelper helper = new DSXmlHelper("Request");
            helper.AddElement("Field");
            helper.AddElement("Field", "All");
            helper.AddElement("Condition");

            foreach (string each in primaryKeys)
            {
                helper.AddElement("Condition", "ID", each);
                execute_required = true;
            }

            DSRequest dsreq = new DSRequest(helper);
            Dictionary<string, TagRecord> result = new Dictionary<string, TagRecord>();

            if (execute_required)
            {
                string srvname = ServiceName;
                foreach (var item in DSAServices.CallService(srvname, dsreq).GetContent().GetElements("Tag"))
                {
                    TagRecord tag = new TagRecord(item);
                    result.Add(tag.ID, tag);
                }
            }

            return result;
        }
    }

    public enum TagCategory
    {
        Student, Class, Teacher, Course
    }

    public static class Tag_Extends
    {
        public static List<TagRecord> GetTagsBy(this IEnumerable<TagRecord> tags, TagCategory category)
        {
            return GetTagsByEntity(tags, category.ToString().ToUpper());
        }

        private static List<TagRecord> GetTagsByEntity(IEnumerable<TagRecord> tags, string category)
        {
            List<TagRecord> results = new List<TagRecord>();
            foreach (TagRecord each in tags)
            {
                if (each.Category.ToUpper() == category)
                    results.Add(each);
            }

            return results;
        }
    }
}

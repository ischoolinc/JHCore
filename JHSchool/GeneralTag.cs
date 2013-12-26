using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA.DSAUtil;
using FISCA.Authentication;
using Framework;

namespace JHSchool
{
    public abstract class GeneralTag<T> : CacheManager<List<T>> where T : GeneralTagRecord, new()
    {
        protected abstract string ServiceName { get; }

        protected abstract string EntityConditionName { get; }

        protected override Dictionary<string, List<T>> GetAllData()
        {
            DSXmlHelper helper = new DSXmlHelper("Request");
            helper.AddElement("Field");
            helper.AddElement("Field", "All");

            DSRequest dsreq = new DSRequest(helper);
            Dictionary<string, List<T>> result = new Dictionary<string, List<T>>();
            string srvname = ServiceName;
            foreach (var item in DSAServices.CallService(srvname, dsreq).GetContent().GetElements("Tag"))
            {
                T objT = new T();
                objT.Initialize(item);

                if (!result.ContainsKey(objT.RefEntityID))
                    result.Add(objT.RefEntityID, new List<T>());

                result[objT.RefEntityID].Add(objT);
            }
            return result;
        }

        protected override Dictionary<string, List<T>> GetData(IEnumerable<string> primaryKeys)
        {
            bool execute_required = false;
            DSXmlHelper helper = new DSXmlHelper("Request");
            helper.AddElement("Field");
            helper.AddElement("Field", "All");
            helper.AddElement("Condition");

            foreach (string each in primaryKeys)
            {
                helper.AddElement("Condition", EntityConditionName, each);
                execute_required = true;
            }

            DSRequest dsreq = new DSRequest(helper);
            Dictionary<string, List<T>> result = new Dictionary<string, List<T>>();

            if (execute_required)
            {
                string srvname = ServiceName;
                foreach (var item in DSAServices.CallService(srvname, dsreq).GetContent().GetElements("Tag"))
                {
                    T objT = new T();
                    objT.Initialize(item);

                    if (!result.ContainsKey(objT.RefEntityID))
                        result.Add(objT.RefEntityID, new List<T>());

                    result[objT.RefEntityID].Add(objT);
                }
            }

            foreach (string primaryKey in primaryKeys)
            {
                if (!result.ContainsKey(primaryKey))
                    result.Add(primaryKey, new List<T>());
            }

            return result;
        }
    }
}

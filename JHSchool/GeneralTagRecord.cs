using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace JHSchool
{
    public abstract class GeneralTagRecord
    {
        /// <summary>
        /// 設定每一個屬性的值。
        /// </summary>
        internal virtual void Initialize(XmlElement data)
        {
            RefTagID = data.SelectSingleNode("RefTagID").InnerText;
            RefEntityID = GetEntityID(data); //每個 Entity  的  Element Name 不同。
        }

        /// <summary>
        /// EntityID 屬性的名稱，每個 Entity 都不同，所以使用 Template Method Pattern。
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected abstract string GetEntityID(XmlElement data);

        internal string RefEntityID { get; set; }

        public string RefTagID { get; protected set; }

        public string Prefix
        {
            get
            {
                TagRecord tag = Tag.Instance[RefTagID];
                if (tag == null)
                    throw new ArgumentException("類別資訊已經不存在於系統中，可能已經刪除。");

                return tag.Prefix;
            }
        }

        public string Name
        {
            get
            {
                TagRecord tag = Tag.Instance[RefTagID];
                if (tag == null)
                    throw new ArgumentException("類別資訊已經不存在於系統中，可能已經刪除。");

                return tag.Name;
            }
        }

        public System.Drawing.Color Color
        {
            get
            {
                TagRecord tag = Tag.Instance[RefTagID];
                if (tag == null)
                    throw new ArgumentException("類別資訊已經不存在於系統中，可能已經刪除。");

                return tag.Color;
            }
        }

        public string FullName
        {
            get
            {
                TagRecord tag = Tag.Instance[RefTagID];
                if (tag == null)
                    throw new ArgumentException("類別資訊已經不存在於系統中，可能已經刪除。");

                return tag.FullName;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace JHSchool
{
    public class TagRecord : IComparable<TagRecord>
    {
        public TagRecord(XmlElement data)
        {
            ID = data.GetAttribute("ID");
            Prefix = data.SelectSingleNode("Prefix").InnerText;
            Name = data.SelectSingleNode("Name").InnerText;
            Category = data.SelectSingleNode("Category").InnerText;

            int ci;
            if (int.TryParse(data.SelectSingleNode("Color").InnerText, out ci))
                ColorCode = ci;
            else
                ColorCode = System.Drawing.Color.White.ToArgb(); //預設是白色。
        }

        public string ID { get; private set; }

        public string Prefix { get; private set; }

        public string Name { get; private set; }

        public string Category { get; private set; }

        private int _color_code;
        internal int ColorCode
        {
            get { return _color_code; }
            set
            {
                _color_code = value;
                Color = System.Drawing.Color.FromArgb(value);
            }
        }

        public System.Drawing.Color Color { get; private set; }

        public string FullName
        {
            get
            {
                if (string.IsNullOrEmpty(Prefix))
                    return Name;
                else
                    return string.Format("{0}:{1}", Prefix, Name);
            }
        }

        #region IComparable<TagRecord> 成員

        public int CompareTo(TagRecord other)
        {
            return Prefix.CompareTo(other.Prefix);
        }

        #endregion
    }
}

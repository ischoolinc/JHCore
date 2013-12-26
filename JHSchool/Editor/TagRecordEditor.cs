using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JHSchool.Editor
{
    public class TagRecordEditor
    {
        public bool Remove { get; set; }

        public string ID { get; private set; }

        public string Prefix { get; set; }

        public string Name { get; set; }

        public string Category { get; private set; }

        private System.Drawing.Color _color;
        public System.Drawing.Color Color
        {
            get { return _color; }
            set
            {
                _color = value;
                ColorCode = _color.ToArgb();
            }
        }

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

        internal int ColorCode { get; set; }

        internal TagRecord Tag { get; set; }

        /// <summary>
        /// 取得修改狀態
        /// </summary>
        public EditorStatus EditorStatus
        {
            get
            {
                if (Remove)
                    return EditorStatus.Delete;

                if (Tag == null)
                {
                    if (Remove) return EditorStatus.NoChanged;

                    return EditorStatus.Insert;
                }
                else
                {
                    if (Tag.Prefix != Prefix ||
                        Tag.Name != Name ||
                        Tag.ColorCode != ColorCode)
                    {
                        return EditorStatus.Update;
                    }
                }
                return EditorStatus.NoChanged;
            }
        }

        public void Save()
        {
            if (EditorStatus != EditorStatus.NoChanged)
            {
                JHSchool.Feature.EditTag.SaveTagRecordEditor(new TagRecordEditor[] { this });
            }
        }

        internal TagRecordEditor(TagRecord record)
        {
            Tag = record;
            Remove = false;
            ID = record.ID;
            Prefix = record.Prefix;
            Name = record.Name;
            Category = record.Category;
            Color = record.Color;
        }

        internal TagRecordEditor(TagCategory category)
        {
            Tag = null;
            Remove = false;
            ID = Prefix = Name = string.Empty;
            Category = category.ToString();
            Color = System.Drawing.Color.White;
        }
    }

    public static class TagRecordEditor_ExtendMethods
    {
        public static TagRecordEditor GetEditor(this TagRecord record)
        {
            return new TagRecordEditor(record);
        }

        /// <summary>
        /// 新增 Tag 資料。
        /// </summary>
        /// <param name="category">Tag 類型。</param>
        /// <returns></returns>
        public static TagRecordEditor AddTag(this Tag tagmanager, TagCategory category)
        {
            return new TagRecordEditor(category);
        }
    }
}

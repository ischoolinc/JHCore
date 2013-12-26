using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevComponents.DotNetBar;

namespace JHSchool
{
    /// <summary>
    /// 代值與顯示文字的項目，可用於 ComboBox。
    /// </summary>
    public class ValueTextItem : ComboBoxItem
    {
        static ValueTextItem()
        {
            Null = new ValueTextItem("", "");
        }

        public static ValueTextItem Null { get; set; }

        public ValueTextItem(string value, string text)
        {
            Value = value;
            Text = text;
        }

        public string Value { get; set; }
    }

    /// <summary>
    /// 代值與顯示文字的項目，可用於 ComboBox。
    /// </summary>
    public class ValueTextItem<T> : ComboBoxItem
    {
        static ValueTextItem()
        {
            Null = new ValueTextItem("", "");
        }

        public static ValueTextItem Null { get; set; }

        public ValueTextItem(string text, T obj)
        {
            Tag = obj;
        }

        public ValueTextItem(string value, string text)
        {
            Value = value;
            Text = text;
        }

        public string Value { get; set; }

        public new T Tag { get; set; }
    }
}

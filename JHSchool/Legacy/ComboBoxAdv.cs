using System;
using System.Collections.Generic;
using System.Text;
using DevComponents.DotNetBar.Controls;
using System.Windows.Forms;
using System.Drawing;

namespace JHSchool.Legacy
{
    [Obsolete("這個Adv是做什麼的？")]
    public class ComboBoxAdv : ComboBoxEx
    {
        private string _emptyDisplay = "<空白>";
        private string _emptyValue = "";

        public ComboBoxAdv()
            : base()
        {
            this.WatermarkText = "";
            this.Font = new System.Drawing.Font(Framework.DotNetBar.FontStyles.GeneralFontFamily, 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            //this.DropDownStyle = ComboBoxStyle.DropDownList;
        }
        
        /// <summary>
        /// 覆寫選取索引變更事件
        /// </summary>
        /// <param name="e"></param>
        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            if (SelectedIndex == -1||SelectedIndex>=Items.Count)
            {
                base.OnSelectedIndexChanged(e);
                return;
            }

            KeyValuePair<string, string> selectedItem = (KeyValuePair<string, string>)SelectedItem;
            bool hasEmptyItem = false;

            //判斷是否有空值
            for (int i = 0; i < Items.Count; i++)
            {
                KeyValuePair<string, string> kvp = (KeyValuePair<string, string>)Items[i];
                if (kvp.Key == _emptyValue)
                {
                    hasEmptyItem = true;
                    break;
                }
            }

            //若無空值且目前所選取到的項目不為空值時
            if (selectedItem.Key != _emptyValue && !hasEmptyItem)
            {
                Items.Insert(0, new KeyValuePair<string, string>(_emptyValue, _emptyDisplay));
                SetComboBoxValue(selectedItem.Key);
            }
            else if (selectedItem.Key == _emptyValue)
            {
                for (int i = 0; i < Items.Count; i++)
                {
                    KeyValuePair<string, string> kvp = (KeyValuePair<string, string>)Items[i];
                    if (kvp.Key == _emptyValue)
                    {
                        Items.RemoveAt(i);
                        break;
                    }
                }
                //this.SelectedItem = null;
            }
            base.OnSelectedIndexChanged(e);
        }

        public void SetComboBoxValue(string value)
        {
            for (int i = 0; i < Items.Count; i++)
            {
                KeyValuePair<string, string> kvp = (KeyValuePair<string, string>)Items[i];
                if (kvp.Key == value)
                {
                    SelectedIndex = i;
                    return;
                }
            }
        }

        public void SetComboBoxText(string text)
        {
            int index = FindString(text);

            if (index >= 0)
            {
                KeyValuePair<string, string> kvp = (KeyValuePair<string, string>)Items[index];
                SelectedIndex = index;
            }

            if (DropDownStyle == ComboBoxStyle.DropDown)
                Text = text;
        }

        public void AddItem(string item)
        {
            AddItem(item, item);
        }

        public void AddItem(string key, string value)
        {
            KeyValuePair<string, string> kvp = new KeyValuePair<string, string>(key, value);
            Items.Add(kvp);
            this.DisplayMember = "Value";
            this.ValueMember = "Key";
        }

        public string GetValue()
        {
            if ( SelectedIndex < Items.Count && SelectedItem != null )
                return ( (KeyValuePair<string, string>)SelectedItem ).Key;
            return _emptyValue;
        }

        public string GetText()
        {
            if (Text == _emptyDisplay)
                return _emptyValue;
            else
                return Text;
        }
    }
}

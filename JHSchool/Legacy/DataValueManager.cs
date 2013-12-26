using System;
using System.Collections.Generic;
using System.Text;
using FISCA.DSAUtil;

namespace JHSchool.Legacy
{

    public class DataValueManager
    {
        private Dictionary<string, string> _displayTexts;
        private Dictionary<string, string> _nowValues;
        private Dictionary<string, string> _oldValues;

        public DataValueManager()
        {
            Initialize();
        }

        public void AddValue(string name, string value)
        {
            AddValue(name, value, name);
        }

        /// <summary>
        /// 加入項目
        /// </summary>
        /// <param name="name">項目索引</param>
        /// <param name="value">項目值</param>
        public void AddValue(string name, string value, string displayText)
        {
            if ( _nowValues.ContainsKey(name) )
                _nowValues[name] = value;
            else
                _nowValues.Add(name, value);

            if ( _oldValues.ContainsKey(name) )
                _oldValues[name] = value;
            else
                _oldValues.Add(name, value);

            if ( _displayTexts.ContainsKey(name) )
                _displayTexts[name] = displayText;
            else
                _displayTexts.Add(name, displayText);
        }

        /// <summary>
        /// 變更項目
        /// </summary>
        /// <param name="name">項目索引</param>
        /// <param name="value">新值</param>
        public void SetValue(string name, string value)
        {
            if ( _nowValues.ContainsKey(name) )
                _nowValues[name] = value;
        }

        /// <summary>
        /// 取出目前所有項目
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetValues()
        {
            return _nowValues;
        }

        /// <summary>
        /// 取出指定名稱的原始資料。
        /// </summary>
        public string GetOldValue(string name)
        {
            return _oldValues[name];
        }

        public string GetDisplayText(string name)
        {
            return _displayTexts[name];
        }

        /// <summary>
        /// 將所有項目清空重設
        /// </summary>
        public void ResetValues()
        {
            Initialize();
        }

        /// <summary>
        /// 將變更項目設為預設項目
        /// </summary>
        public void MakeDirtyToClean()
        {
            foreach ( string key in _nowValues.Keys )
            {
                _oldValues[key] = _nowValues[key];
            }
        }

        /// <summary>
        /// 判斷是否已有值被變更
        /// </summary>
        public bool IsDirty
        {
            get
            {
                foreach ( string key in _oldValues.Keys )
                {
                    if ( IsDirtyItem(key) )
                        return true;
                }
                return false;
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        private void Initialize()
        {
            _nowValues = new Dictionary<string, string>();
            _oldValues = new Dictionary<string, string>();
            _displayTexts = new Dictionary<string, string>();
        }

        public DSRequest GetRequest(string rootName, string dataElementName, string fieldElementName, string conditionElementName, string conditionName, string id)
        {
            DSRequest dsreq = new DSRequest();
            DSXmlHelper helper = new DSXmlHelper(rootName);
            if ( !string.IsNullOrEmpty(dataElementName) )
            {
                helper.AddElement(dataElementName);
                helper.AddElement(dataElementName, fieldElementName);
                helper.AddElement(dataElementName, conditionElementName);
                fieldElementName = dataElementName + "/" + fieldElementName;
                conditionElementName = dataElementName + "/" + conditionElementName;
            }
            else
            {
                helper.AddElement(fieldElementName);
                helper.AddElement(conditionElementName);
            }

            foreach ( string key in _nowValues.Keys )
            {
                if ( _nowValues[key] != _oldValues[key] )
                {
                    helper.AddElement(fieldElementName, key, _nowValues[key]);
                }
            }

            helper.AddElement(conditionElementName, conditionName, id);
            dsreq.SetContent(helper);
            //Console.WriteLine(helper.GetRawXml());
            return dsreq;
        }

        /// <summary>
        /// 取出變更項目清單
        /// </summary>
        /// <returns>變更項目清單，key值為索引,value為變更後的值</returns>
        public Dictionary<string, string> GetDirtyItems()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();

            foreach ( string key in _oldValues.Keys )
            {
                if ( IsDirtyItem(key) )
                    dic.Add(key, _nowValues[key]);
            }
            return dic;
        }

        /// <summary>
        /// 判斷key值是否已變更
        /// </summary>
        /// <param name="key">索引</param>
        /// <returns>若已變更則傳回 true，反之傳回 false</returns>
        public bool IsDirtyItem(string key)
        {
            return _oldValues[key] != _nowValues[key];
        }
    }
}

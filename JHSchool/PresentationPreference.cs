using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using FISCA.Presentation;
using Framework.Feature;
using FISCA.Authentication;
using FISCA.DSAUtil;
using Framework;

namespace JHSchool
{
    /// <summary>
    /// 提供 FISCA.Presentation 喜好設定提供者實作。
    /// </summary>
    internal class PresentationPreference : IPreferenceProvider
    {
        private PreferenceProvider _legacy;
        private ConfigData _config_data, _origin_config;

        public PresentationPreference()
        {
            _legacy = new PreferenceProvider(); //TODO 過一段時間後，就拿掉吧。
            _config_data = User.Configuration["UserPresentationPreference"];
            _origin_config = _config_data.Async();
        }

        #region IPreferenceProvider 成員

        public XmlElement this[string Key]
        {
            get
            {
                XmlElement xmlpre = _origin_config.GetXml(Key, null);

                if (xmlpre == null)
                    return _legacy[Key];
                else
                    return xmlpre;
            }
            set
            {
                _config_data.SetXml(Key, value);
                _config_data.Save();
            }
        }

        #endregion
    }

    internal class PreferenceProvider : IPreferenceProvider
    {
        private XmlElement BackupElement;

        public PreferenceProvider()
        {
            RootElement = GetPreference().GetContent().BaseElement;
            BackupElement = (XmlElement)new XmlDocument().ImportNode(RootElement, true);
        }

        [AutoRetryOnWebException()]
        public static DSResponse GetPreference()
        {
            DSXmlHelper request = new DSXmlHelper("Content");
            request.AddElement(".", "UserName", DSAServices.UserAccount);

            return DSAServices.CallService("SmartSchool.Personal.GetPreference", new DSRequest(request));
        }

        private XmlElement RootElement;
        public XmlElement this[string Key]
        {
            get
            {
                XmlElement element = (XmlElement)RootElement.SelectSingleNode(Key);
                if (element == null)
                {
                    return null;
                }
                else
                    return (XmlElement)element;
            }
            set
            {
                throw new NotSupportedException();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework;

namespace JHSchool
{
    public static class School
    {
        /// <summary>
        /// 取得學校的組態資料。
        /// </summary>
        public static SchoolConfigRedirect Configuration { get; internal set; }

        /// <summary>
        /// 取得預設學年度(是「預設」喔，不要誤會意思！)。
        /// </summary>
        public static string DefaultSchoolYear
        {
            get { return Framework.Legacy.GlobalOld.SystemConfig.DefaultSchoolYear.ToString(); }
        }

        /// <summary>
        /// 取得預設學期(是「預設」喔，不要誤會意思！)。
        /// </summary>
        public static string DefaultSemester
        {
            get { return Framework.Legacy.GlobalOld.SystemConfig.DefaultSemester.ToString(); }
        }

        /// <summary>
        /// 取得學校中文名稱。
        /// </summary>
        public static string ChineseName
        {
            get { return Framework.Legacy.GlobalOld.SchoolInformation.ChineseName; }
        }

        /// <summary>
        /// 取得學校英文名稱。
        /// </summary>
        public static string EnglishName
        {
            get { return Framework.Legacy.GlobalOld.SchoolInformation.EnglishName; }
        }

        /// <summary>
        /// 取得學校地址。
        /// </summary>
        public static string Address
        {
            get { return Framework.Legacy.GlobalOld.SchoolInformation.Address; }
        }

        /// <summary>
        /// 取得學生電話資料。
        /// </summary>
        public static string Telephone
        {
            get { return Framework.Legacy.GlobalOld.SchoolInformation.Telephone; }
        }

        #region SchoolConfigRedirect
        public class SchoolConfigRedirect
        {
            private ConfigurationManager Manager { get; set; }

            internal SchoolConfigRedirect(ConfigurationManager manager)
            {
                Manager = manager;
            }

            public ConfigData this[string configNamespace]
            {
                get { return Manager[configNamespace]; }
            }

            public void Sync(string configNamespace)
            {
                Manager.Sync(configNamespace);
            }

            public void Remove(ConfigData config)
            {
                Manager.Remove(config);
            }
        }
        #endregion
    }
}

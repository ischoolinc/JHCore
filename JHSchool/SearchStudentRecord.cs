using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace JHSchool
{
    class SearchStudentRecord
    {
        /// <summary>
        /// 學號
        /// </summary>
        public string StudentNumber { get; set; }
        /// <summary>
        /// 身份證號
        /// </summary>
        public string IDNumber { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 系統編號
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 登入帳號
        /// </summary>
        public string SA_Login_Name { get; set; }
        /// <summary>
        /// 父親
        /// </summary>
        public string Father_Name { get; set; }
        /// <summary>
        /// 母親
        /// </summary>
        public string Mother_Name { get; set; }
        /// <summary>
        /// 監護人
        /// </summary>
        public string Custodian_Name { get; set; }
        /// <summary>
        /// 英文姓名
        /// </summary>
        public string English_Name { get; set; }

        public SearchStudentRecord(DataRow row)
        {
            ID = "" + row[0];
            Name = "" + row[1];
            StudentNumber = "" + row[2];
            IDNumber = "" + row[3];
            SA_Login_Name = "" + row[4];
            Father_Name = "" + row[5];
            Mother_Name = "" + row[6];
            Custodian_Name = "" + row[7];
            English_Name = "" + row[8];
        }
    }
}

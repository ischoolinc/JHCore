using System;
using System.Collections.Generic;
using System.Text;

namespace JHSchool
{
    // 搜尋用
    class SearchParentEntity
    {
        /// <summary>
        /// 學生系統編號
        /// </summary>
        public string StudentID { get; set; }
        /// <summary>
        /// 父親姓名
        /// </summary>
        public string FatherName { get; set; }
        /// <summary>
        /// 母親姓名
        /// </summary>
        public string MotherName { get; set; }
        /// <summary>
        /// 監護人姓名
        /// </summary>
        public string CustodianName { get; set; }
        /// <summary>
        /// 畫在上 StudentRecord
        /// </summary>
        public StudentRecord studRec { get; set; }

    }
}

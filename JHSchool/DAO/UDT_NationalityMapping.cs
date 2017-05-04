using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA.UDT;

namespace JHSchool.DAO
{
    /// <summary>
    /// 國籍對照資料
    /// </summary>
    [TableName("ischool.mapping.nationality")]
    public class UDT_NationalityMapping : ActiveRecord
    {
        /// <summary>
        /// 中文名稱
        /// </summary>        
        [Field(Field = "name", Indexed = false)]
        public string Name { get; set; }

        /// <summary>
        /// 英文名稱
        /// </summary>        
        [Field(Field = "eng_name", Indexed = false)]
        public string Eng_Name { get; set; }
    }
}

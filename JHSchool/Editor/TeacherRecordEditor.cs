using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JHSchool.Editor
{
    public class TeacherRecordEditor
    {
        public bool Remove { get; set; }
        public string ID { get; set; }
        public string Name { get; set; }
        public string Nickname { get; set; }
        public string Status { get; set; }
        public string Gender { get; set; }
        public string IDNumber { get; set; }
        //public string UniqName { get; set; }
        public string ContactPhone { get; set; }
        public string Category { get; set; }
        //public string SuperviseByClassID { get; set; }

        internal TeacherRecord Teacher { get; private set; }

        /// <summary>
        /// 取得修改狀態
        /// </summary>
        public EditorStatus EditorStatus
        {
            get
            {
                if (Remove)
                    return EditorStatus.Delete;

                if (Teacher == null)
                {
                    if (Remove) return EditorStatus.NoChanged;

                    return EditorStatus.Insert;
                }
                else
                {
                    if (Teacher.Name != Name ||
                        Teacher.Nickname != Nickname ||
                        Teacher.Status != Status ||
                        Teacher.Gender != Gender ||
                        Teacher.IDNumber != IDNumber ||
                        Teacher.ContactPhone != ContactPhone ||
                        Teacher.Category != Category)
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
                
                //EditClass.SaveClassRecordEditor(new ClassRecordEditor[] { this });
            }
        }
    }
}

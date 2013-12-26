using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JHSchool.Editor
{
    public abstract class GeneralTagRecordEditor
    {
        public bool Remove { get; set; }
        internal string RefEntityID { get; set; }
        public string RefTagID { get; protected set; }

        internal abstract GeneralTagRecord GetEntityTagRecord();
        internal GeneralTagRecord GeneralTagRecord { get { return GetEntityTagRecord(); } }

        /// <summary>
        /// 取得修改狀態
        /// </summary>
        public EditorStatus EditorStatus
        {
            get
            {
                if (Remove)
                    return EditorStatus.Delete;

                if (GeneralTagRecord == null)
                {
                    if (Remove) return EditorStatus.NoChanged;

                    return EditorStatus.Insert;
                }
                else
                {
                    if (GeneralTagRecord.RefTagID != RefTagID ||
                        GeneralTagRecord.RefEntityID != RefEntityID)
                    {
                        return EditorStatus.Update;
                    }
                }
                return EditorStatus.NoChanged;
            }
        }

        public abstract void Save();
    }
}

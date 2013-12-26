using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace JHSchool.SF.Evaluation
{
    public static class SemesterScoreEditor
    {
        private static Func<string, DialogResult> Handler1 { get; set; }
        private static Func<string, int, int, DialogResult> Handler2 { get; set; }

        public static DialogResult ShowDialog(string studentId)
        {
            if (Handler1 != null) return Handler1(studentId);
            else return DialogResult.None;
        }

        public static DialogResult ShowDialog(string studentId, int schoolYear, int semester)
        {
            if (Handler2 != null) return Handler2(studentId, schoolYear, semester);
            else return DialogResult.None;
        }

        public static void RegisterHandler(Func<string, DialogResult> handler)
        {
            Handler1 = handler;
        }

        public static void RegisterHandler(Func<string, int, int, DialogResult> handler)
        {
            Handler2 = handler;
        }

        /// <summary>
        /// 判斷 Handler 是否存在。
        /// </summary>
        //public static bool HandlerExists { get { return Handler != null; } }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace JHSchool.SF.Evaluation
{
    public static class QuickInputSemesterScoreForm
    {
        private static Func<string, DialogResult> Handler { get; set; }

        public static DialogResult ShowDialog(string studentId)
        {
            if (Handler != null)
                return Handler(studentId);
            else
                return DialogResult.None;
        }

        public static void RegisterHandler(Func<string, DialogResult> handler)
        {
            Handler = handler;
        }

        /// <summary>
        /// 判斷 Handler 是否存在。
        /// </summary>
        public static bool HandlerExists { get { return Handler != null; } }
    }
}

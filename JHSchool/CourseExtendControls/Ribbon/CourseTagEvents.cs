using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JHSchool.CourseExtendControls.Ribbon
{
    public static class CourseTagEvents
    {
        public static void RaiseTagChanged()
        {
            if (TagChanged != null)
                TagChanged(null, EventArgs.Empty);
        }

        public static event EventHandler TagChanged;
    }
}

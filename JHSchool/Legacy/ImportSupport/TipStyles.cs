using System;
using System.Collections.Generic;
using System.Text;
using Aspose.Cells;
using System.Drawing;

namespace JHSchool.Legacy.ImportSupport
{
    public class TipStyles
    {
        public TipStyles(SheetHelper helper)
        {
            Style template = InitialTemplateStyle(helper);

            _default = template;
            _warning = helper.NewStyle();
            _error = helper.NewStyle();
            _correct = helper.NewStyle();
            _header = helper.NewStyle();

            _warning.Copy(template);
            _error.Copy(template);
            _correct.Copy(template);
            _header.Copy(template);

            _warning.Pattern = BackgroundType.Solid;
            _error.Pattern = BackgroundType.Solid;
            _correct.Pattern = BackgroundType.Solid;
            _header.Pattern = BackgroundType.Solid;

            _warning.ForegroundColor = helper.MatchColor(Color.Yellow);
            _error.ForegroundColor = helper.MatchColor(Color.Red);
            _correct.ForegroundColor = helper.MatchColor(Color.Teal);
            _header.ForegroundColor = helper.MatchColor(Color.Blue);
        }

        private static Style InitialTemplateStyle(SheetHelper helper)
        {
            Style template = helper.NewStyle();

            template.Font.Color = helper.MatchColor(Color.Black);
            template.ForegroundColor = helper.MatchColor(Color.White);

            return template;
        }

        private Style _default;
        public Style Default
        {
            get { return _default; }
        }

        private Style _header;
        public Style Header
        {
            get { return _header; }
        }

        private Style _warning;
        public Style Warning
        {
            get { return _warning; }
        }

        private Style _error;
        public Style Error
        {
            get { return _error; }
        }

        private Style _correct;
        public Style Correct
        {
            get { return _correct; }
        }

    }
}

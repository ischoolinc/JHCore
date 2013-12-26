using System;
using System.Collections.Generic;
using System.Text;
using Aspose.Cells;
using System.Drawing;

namespace JHSchool.StudentExtendControls.Ribbon.StudentImportWizardControls.ValidateModel
{
    public class TipStyle
    {
        private Style _normal, _correct, _warning, _error, _header;

        public TipStyle(Workbook book, Style template)
        {
            _normal = CreateStyle(book, template, Color.White);
            _normal.Copy(template);
            _normal.Pattern = BackgroundType.None;
            _normal.Font.Color = Color.Black;

            _correct = CreateStyle(book, _normal, Color.Teal);
            _warning = CreateStyle(book, _normal, Color.Yellow);

            _error = CreateStyle(book, _normal, Color.Red);
            _error.Font.Color = Color.White;

            _header = CreateStyle(book, _normal, Color.Blue);
            _header.Font.Color = Color.White;
        }

        public Style Normal
        {
            get { return _normal; }
        }

        public Style Correct
        {
            get { return _correct; }
        }

        public Style Warning
        {
            get { return _warning; }
        }

        public Style Error
        {
            get { return _error; }
        }

        public Style Header
        {
            get { return _header; }
        }

        private Style CreateStyle(Workbook book, Style template, Color color)
        {
            int index = book.Styles.Add();
            Style s = book.Styles[index];

            s.Copy(template);
            s.Pattern = BackgroundType.Solid;
            s.ForegroundColor = color;

            return s;
        }
    }
}

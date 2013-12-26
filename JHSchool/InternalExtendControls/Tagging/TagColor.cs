using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace JHSchool.InternalExtendControls.Tagging
{
    internal class TagColor
    {
        private Bitmap _image;
        private Color _color;

        public TagColor(Color color)
        {
            InitialImage(color);
        }

        public TagColor(int argb)
        {
            InitialImage(Color.FromArgb(argb));
        }

        private void InitialImage(Color color)
        {
            _image = new Bitmap(16, 16);

            if (color == Color.Empty)
                _color = Color.White;
            else
                _color = color;

            using (Graphics gx = Graphics.FromImage(_image))
            {
                Rectangle rect = new Rectangle(2, 2, 13, 13);
                Rectangle rectSmall = new Rectangle(3, 3, 12, 12);

                SolidBrush brush = new SolidBrush(color);
                gx.FillRectangle(brush, rectSmall);
                gx.DrawRectangle(Pens.Black, rect);
            }
        }

        public Color Color
        {
            get { return _color; }
        }

        public Image Image
        {
            get { return _image; }
        }
    }

}

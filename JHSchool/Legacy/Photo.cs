using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Imaging;
using System.IO;
using System.Drawing;

namespace JHSchool.Legacy
{
    class Photo
    {
        public const int MAX_WIDTH = 225;
        public const int MAX_HEIGHT = 300;
        public static readonly ImageFormat IMAGE_FORMAT = ImageFormat.Jpeg;

        public static string GetBase64Encoding(FileInfo file)
        {
            FileStream fs = null;
            try
            {
                fs = new FileStream(file.FullName, FileMode.Open);

                byte[] bytes = new byte[fs.Length];
                fs.Read(bytes, 0, (int)fs.Length);
                string s = Convert.ToBase64String(bytes);
                fs.Close();
                return s;
            }
            catch ( Exception )
            {
                return string.Empty;
            }
        }

        public static string GetBase64Encoding(Bitmap image)
        {
            try
            {
                MemoryStream ms = new MemoryStream();
                image.Save(ms, ImageFormat.Jpeg);

                byte[] bytes = new byte[ms.Length];
                ms.Seek(0, SeekOrigin.Begin);
                ms.Read(bytes, 0, (int)ms.Length);
                string s = Convert.ToBase64String(bytes);
                ms.Close();
                return s;
            }
            catch ( Exception )
            {
                return string.Empty;
            }
        }

        public static Bitmap ConvertFromBase64Encoding(string base64, bool resize)
        {
            byte[] bs = Convert.FromBase64String(base64);
            MemoryStream ms = new MemoryStream(bs);
            Bitmap bm = new Bitmap(ms);
            if ( resize )
                return Resize(bm);
            else
                return bm;
        }

        public static Bitmap ConvertFromBase64Encoding(string base64, int maxWidth, int maxHeight)
        {
            byte[] bs = Convert.FromBase64String(base64);
            MemoryStream ms = new MemoryStream(bs);
            Bitmap bm = new Bitmap(ms);
            return Resize(bm, maxWidth, maxHeight);
        }

        public static Bitmap Resize(Bitmap photo, int maxWidth, int maxHeight)
        {
            Size s = GetResize(photo, maxWidth, maxHeight);
            return new Bitmap(photo, s);
        }

        public static Bitmap Resize(Bitmap photo)
        {
            Size s = GetResize(photo);
            return new Bitmap(photo, s);
        }

        public static Size GetResize(Bitmap photo)
        {
            return GetResize(photo, Photo.MAX_WIDTH, Photo.MAX_HEIGHT);
        }

        public static Size GetResize(Bitmap photo, int maxWidth, int maxHeight)
        {
            int width = photo.Width;
            int height = photo.Height;
            Size newSize;

            if ( width < maxWidth && height < maxHeight )
                return new Size(width, height);

            decimal maxW = Convert.ToDecimal(maxWidth);
            decimal maxH = Convert.ToDecimal(maxHeight);

            decimal mp = decimal.Divide(maxW, maxH);
            decimal p = decimal.Divide(width, height);


            // 若長寬比預設比例較寬, 則以傳入之長為縮放基準
            if ( mp > p )
            {
                decimal hp = decimal.Divide(maxH, height);
                decimal newWidth = decimal.Multiply(hp, width);
                newSize = new Size(decimal.ToInt32(newWidth), maxHeight);
            }
            else
            {
                decimal wp = decimal.Divide(maxW, width);
                decimal newHeight = decimal.Multiply(wp, height);
                newSize = new Size(maxWidth, decimal.ToInt32(newHeight));
            }

            return newSize;
        }

        public static Bitmap Resize(FileInfo file)
        {
            Bitmap b = new Bitmap(file.FullName);
            return Resize(b);
        }

        public static Bitmap Resize(FileInfo file, int maxWidth, int maxHeight)
        {
            Bitmap b = new Bitmap(file.FullName);
            return Resize(b, maxWidth, maxHeight);
        }
    }
}

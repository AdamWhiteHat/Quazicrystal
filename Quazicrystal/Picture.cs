using System;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.IO;

namespace Quazicrystal
{
    public static class Picture
    {
        public static Bitmap QuasicrystalImage(double scale, double symmetries, int pow, int size, int step)
        {
            Point offset = new Point(size, size);
            return PlotImage(Crystal.Get(symmetries, pow, new Size(size, size), offset, step, scale));
        }

        public static int ColorFunction(double w)
        {
            // (1 + Tanh[ 10 * (w - 0.5) ] ) / 2
            int res = (int)Math.Truncate(((1.0f + Math.Tanh(10.0f * (w - 0.5f))) / 2.0f) * 255.0f);
            int result = res & byte.MaxValue;
            return result;
        }

        public static Bitmap PlotImage(IEnumerable<Tuple<int, int, int>> plotPoints)
        {
            int width = (plotPoints.Max(tup => tup.Item1) - plotPoints.Min(tup => tup.Item1)) + 1;
            int height = (plotPoints.Max(tup => tup.Item2) - plotPoints.Min(tup => tup.Item2)) + 1;
            Bitmap result = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format16bppRgb555);

            foreach (Tuple<int, int, int> pxl in plotPoints)
            {
                result.SetPixel(pxl.Item1, pxl.Item2, System.Drawing.Color.FromArgb(pxl.Item3, pxl.Item3, pxl.Item3));
            }

            return result;
        }

        private static void SaveBmp(Bitmap bmp, string path)
        {
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);

            BitmapData bitmapData = bmp.LockBits(rect, ImageLockMode.ReadOnly, bmp.PixelFormat);

            var pixelFormats = ConvertBmpPixelFormat(bmp.PixelFormat);

            BitmapSource source = BitmapSource.Create(bmp.Width,
                                                      bmp.Height,
                                                      bmp.HorizontalResolution,
                                                      bmp.VerticalResolution,
                                                      pixelFormats,
                                                      null,
                                                      bitmapData.Scan0,
                                                      bitmapData.Stride * bmp.Height,
                                                      bitmapData.Stride);

            bmp.UnlockBits(bitmapData);


            FileStream stream = new FileStream(path, FileMode.Create);

            TiffBitmapEncoder encoder = new TiffBitmapEncoder();

            encoder.Compression = TiffCompressOption.Zip;
            encoder.Frames.Add(BitmapFrame.Create(source));
            encoder.Save(stream);

            stream.Close();
        }

        private static System.Windows.Media.PixelFormat ConvertBmpPixelFormat(System.Drawing.Imaging.PixelFormat pixelformat)
        {
            System.Windows.Media.PixelFormat pixelFormats = System.Windows.Media.PixelFormats.Default;

            switch (pixelformat)
            {
                case System.Drawing.Imaging.PixelFormat.Format32bppArgb:
                    pixelFormats = PixelFormats.Bgr32;
                    break;

                case System.Drawing.Imaging.PixelFormat.Format8bppIndexed:
                    pixelFormats = PixelFormats.Gray8;
                    break;

                case System.Drawing.Imaging.PixelFormat.Format16bppGrayScale:
                    pixelFormats = PixelFormats.Gray16;
                    break;
            }

            return pixelFormats;
        }
    }
}

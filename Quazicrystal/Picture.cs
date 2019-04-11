using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quazicrystal
{
	public static class Picture
	{
		public static Bitmap QuasicrystalImage(double scale, double symmetries, int pow, int size, int step)
		{
			return PlotImage(Crystal.Get(scale, symmetries, pow, size, step));
		}

		public static Bitmap PlotImage(IEnumerable<Tuple<int, int, int>> plotPoints)
		{
			int width = (plotPoints.Max(tup => tup.Item1) - plotPoints.Min(tup => tup.Item1)) + 1;
			int height = (plotPoints.Max(tup => tup.Item2) - plotPoints.Min(tup => tup.Item2)) + 1;
			Bitmap result = new Bitmap(width, height);

			foreach (Tuple<int, int, int> pxl in plotPoints)
			{
				result.SetPixel(pxl.Item1, pxl.Item2, Color.FromArgb(pxl.Item3, pxl.Item3, pxl.Item3));
			}

			return result;
		}

		public static int ColorFunction(double w)
		{
			// (1 + Tanh[ 10 * (w - 0.5) ] ) / 2
			int res = (int)Math.Truncate(((1.0f + Math.Tanh(10.0f * (w - 0.5f))) / 2.0f) * 255.0f);
			int result = res & byte.MaxValue;
			return result;
		}
	}
}

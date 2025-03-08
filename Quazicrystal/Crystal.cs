using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Quazicrystal
{
	public static class Crystal
	{
		public static List<Tuple<int, int, int>> Get(double symmetries, int pow, Size size, Point offset, int step, double scale)
		{
			double angle = Math.Pow(Math.PI, pow) / symmetries;
			//(Math.PI*2)
			//double max = (Pi * (1.0m - (1.0m / sym)));

			int trunc = (int)Math.Truncate(symmetries);
			if (symmetries % 1 != 0)
			{
				trunc += 1;
			}

			IEnumerable<double> rangeTheta = Enumerable.Range(0, trunc).Select(i => angle * i);
			List<Tuple<double, double>> precalculatedTheta = rangeTheta.Select(t => new Tuple<double, double>(Math.Sin(t), (Math.Cos(t)))).ToList();

			IEnumerable<int> rangeX = Enumerable.Range(offset.X, size.Height); // 2000, 6000
			List<int> rangeY = Enumerable.Range(offset.Y, size.Height).ToList();

			// {int, int, int} => {x, y, amplitude} 
			List<Tuple<int, int, int>> result =
				rangeX.AsParallel()
					.SelectMany(x =>
						rangeY.Select(y =>
							new Tuple<int, int, int>(
								x - offset.X,
								y - offset.Y,
								Picture.ColorFunction(precalculatedTheta.Select(theta => WaveFunction(scale, theta.Item1, theta.Item2, x, y, step)).Sum())
							)
						)
					).ToList();

			return result;
		}

		public static double WaveFunction(double scale, double sinTheta, double cosTheta, double x, double y, double step)
		{
			// Sum( Cos[ (Sin(theta) * X) + (Cos(theta) * Y) + step] )
			double sum = ((sinTheta * x) + (cosTheta * y) + (step));
			sum *= scale;
			return Math.Cos(sum);
			//return (step % 2 == 0) ? Math.Cos(sum) : Math.Sin(sum);
		}
	}
}

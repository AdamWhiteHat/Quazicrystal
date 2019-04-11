using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Drawing.Imaging;

namespace Quazicrystal
{
	public partial class MainForm : Form
	{
		public MainForm()
		{
			InitializeComponent();
			tbSymmetries.Text = "7";
			tbSize.Text = "200";
			tbScale.Text = "1.0";
		}

		private void btnGenerate_Click(object sender, EventArgs e)
		{
			int pow = 1;
			double symmetries = double.Parse(tbSymmetries.Text);
			double scale = double.Parse(tbScale.Text);

			int size = int.Parse(tbSize.Text);
			int range = size / 2;

			int steps = (int)Math.Truncate(symmetries);
			if (symmetries % 1 != 0)
			{
				steps += 1;
			}

			IEnumerable<int> rangeSteps = Enumerable.Range(0, steps).Select(n => n * steps);

			counter = 0;
			uid = (int)(DateTime.UtcNow.ToFileTimeUtc() % 1000);

			List<Task<Bitmap>> tasks =
				rangeSteps.Select(step =>
					{
						Task<Bitmap> proc = new Task<Bitmap>(() => { return Picture.QuasicrystalImage(scale, symmetries, pow, range, step); });
						proc.ContinueWith((t) => SetPicturebox(t.Result));
						return proc;
					}).ToList();

			Task runner = new Task(() =>
			{
				foreach (Task<Bitmap> tsk in tasks)
				{
					tsk.Start();
					tsk.Wait();
				}
			});
			runner.Start();

		}


		private static int counter = 0;
		private static int uid = 0;
		private void SetPicturebox(Bitmap image)
		{
			if (pictureBox1.InvokeRequired)
			{
				pictureBox1.Invoke(new Action(() => SetPicturebox(image)));
			}
			else
			{
				pictureBox1.Image = image;
				image.Save($"Quasicrystal_{tbSymmetries.Text}FoldSymmetry_{image.Width}X{image.Height}_{uid}_{(++counter)}of{tbSymmetries.Text}.png", ImageFormat.Png);
			}
		}
	}
}

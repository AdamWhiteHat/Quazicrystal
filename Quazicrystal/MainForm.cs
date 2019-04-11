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
			double symmetries = double.Parse(tbSymmetries.Text);

			int range = int.Parse(tbSize.Text);
			double scale = double.Parse(tbScale.Text);

			int steps = (int)Math.Truncate(symmetries);
			if (symmetries % 1 != 0)
			{
				steps += 1;
			}

			int pow = 1;

			IEnumerable<int> rangeSteps = Enumerable.Range(0, steps).Select(n => n*steps);

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

		private void SetPicturebox(Bitmap image)
		{
			if (pictureBox1.InvokeRequired)
			{
				pictureBox1.Invoke(new Action(() => SetPicturebox(image)));
			}
			else
			{
				pictureBox1.Image = image;
			}
		}
	}
}

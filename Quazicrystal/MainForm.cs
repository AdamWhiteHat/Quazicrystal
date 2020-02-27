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
			tbSize.Text = "400";
			tbScale.Text = "1.0";
		}

		private void btnGenerate_Click(object sender, EventArgs e)
		{
			GenerateQuazicrystal();
		}

		private void GenerateQuazicrystal()
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

			IEnumerable<int> rangeSteps = Enumerable.Range(0, steps);

			counter = 0;
			uid = (int)(DateTime.UtcNow.ToFileTimeUtc() % 1000);

			List<Task<Bitmap>> tasks =
				rangeSteps.Select(step =>
					{
						Task<Bitmap> proc = new Task<Bitmap>(() => { return Picture.QuasicrystalImage(scale, symmetries, pow, range, step); });
						proc.ContinueWith((t) => SetPicturebox(t.Result));
						return proc;
					}).ToList();

			SetResizable(false);

			Task runner = new Task(() =>
			{
				foreach (Task<Bitmap> tsk in tasks)
				{
					tsk.Start();
					tsk.Wait();
				}
				SetResizable(true);
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


				//ImageCodecInfo bmpCodec = ImageCodecInfo.GetImageEncoders()[0];
				//if (bmpCodec != null)
				//{
				//	EncoderParameters parameters = new EncoderParameters();
				//	parameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.ColorDepth, 8);

					image.Save($"Quasicrystal{uid}_{tbSymmetries.Text}-FoldSymmetry_{image.Width}x{image.Height}_@{tbScale.Text}X_{(++counter)}of{tbSymmetries.Text}.bmp", ImageFormat.Bmp);
				//}
				
			}
		}

		private void SetResizable(bool allowResize)
		{
			if (this.InvokeRequired)
			{
				this.Invoke(new Action(() => SetResizable(allowResize)));
			}
			else
			{
				
				this.SizeGripStyle = allowResize ? SizeGripStyle.Show : SizeGripStyle.Hide;

				if (allowResize)
				{
					this.WindowState = FormWindowState.Normal;

					Size formSize = this.Size;
					Size tableSize = tableLayoutPanel1.Size;
					Size pictureSize = pictureBox1.Size;

					this.AutoSize = false;
					this.Size = formSize;

					tableLayoutPanel1.AutoSize = false;
					tableLayoutPanel1.Size = tableSize;
					tableLayoutPanel1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

					pictureBox1.Dock = DockStyle.None;
					pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
					pictureBox1.Size = pictureSize;
					pictureBox1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

				}
				else
				{
					pictureBox1.Anchor = AnchorStyles.Top | AnchorStyles.Left;
					tableLayoutPanel1.Anchor = AnchorStyles.Top | AnchorStyles.Left;

					tableLayoutPanel1.AutoSize = true;
					this.AutoSize = true;

					pictureBox1.Dock = DockStyle.Fill;
					pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;

				}

			}
		}

		private void textbox_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				GenerateQuazicrystal();
			}
		}
	}
}

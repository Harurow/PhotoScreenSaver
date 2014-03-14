using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using Brushes = System.Windows.Media.Brushes;
using Pen = System.Windows.Media.Pen;

namespace PhotoScreenSaver.Model
{
	public class TextureManager
	{
		#region singleton

		private static readonly object InstSync = new object();

		private static volatile TextureManager __inst;

		public static TextureManager Instance
		{
			get
			{
				if (__inst != null)
				{
					return __inst;
				}
				lock (InstSync)
				{
					if (__inst == null)
					{
						__inst = new TextureManager();
					}
				}
				return __inst;
			}
		}

		#region ctor

		private TextureManager()
		{
			// initialize instance
		}

		#endregion

		#endregion

		#region properties and fields

		#region const and read only

		#endregion

		public double FrameRatio = 0.030; 

		#endregion

		#region public methdos

		public async Task<Texture> CreateTextureAsync(string path)
		{
			var bi = await CreateBitmapSourceAsync(path);

			var frameSize = Math.Min(bi.Width, bi.Height) * FrameRatio;
			var w = bi.Width + frameSize * 2;
			var h = bi.Height + frameSize * 2;

			var dg = new DrawingGroup();
			using (var dc = dg.Open())
			{
				dc.DrawRectangle(Brushes.White, new Pen(Brushes.Snow, frameSize / 10.0), new Rect(0, 0, w, h));
				dc.DrawImage(bi, new Rect(frameSize, frameSize, bi.Width, bi.Height));
				dc.Close();
			}

			var material = new DiffuseMaterial(new ImageBrush(new DrawingImage(dg)));

			return new Texture(path, material, w, h);
		}

		#endregion

		#region helpers

		[DllImport("gdi32.dll")]
		private static extern bool DeleteObject(IntPtr hObject);

		private static async Task<BitmapSource> CreateBitmapSourceAsync(string path)
		{
			IntPtr hBmp = IntPtr.Zero;
			await Task.Run(() =>
			{
				using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
				{
					using (var bmp = (Bitmap)Image.FromStream(fs))
					{
						hBmp = bmp.GetHbitmap();
					}
				}
			});

			try
			{
				return Imaging.CreateBitmapSourceFromHBitmap(
					hBmp, IntPtr.Zero, Int32Rect.Empty,
					BitmapSizeOptions.FromEmptyOptions());
			}
			finally
			{
				DeleteObject(hBmp);
			}
		}

		#endregion
	}
}
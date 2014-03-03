using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

namespace PhotoScreenSaver
{
	public sealed class PhotoManager
	{
		#region inner class

		private class PhotoInfoInternal
		{
			private int _refCount;
			public readonly PhotoInfo PhotoInfo;

			private PhotoInfoInternal(PhotoInfo info)
			{
				_refCount = 1;
				PhotoInfo = info;
			}

			public void AddRef()
			{
				_refCount++;
			}

			public int ReleaseRef()
			{
				_refCount--;
				return _refCount;
			}

			public static PhotoInfoInternal Create(string path)
			{
				var srcImg = new BitmapImage(new Uri(path));

				var photoFrame = Math.Min(srcImg.Width, srcImg.Height)/40;

				var bmp = new WriteableBitmap(
					(int) (srcImg.Width + photoFrame*2),
					(int) (srcImg.Height + photoFrame*2),
					96, 96, PixelFormats.Rgb24, null);

				var dg = new DrawingGroup();
				using (var dc = dg.Open())
				{
					var photoArea = new Rect(
						photoFrame, photoFrame,
						srcImg.Width, srcImg.Height);

					dc.DrawRectangle(Brushes.White, null, new Rect(0, 0, bmp.Width, bmp.Height));
					dc.DrawImage(srcImg, photoArea);
				}

				var info = new PhotoInfo(new DiffuseMaterial(new ImageBrush(new DrawingImage(dg))),
					bmp.Width, bmp.Height);

				return new PhotoInfoInternal(info);
			}
		}

		#endregion

		#region singleton

		private static readonly object InstSync = new object();

		private static volatile PhotoManager __inst;

		public static PhotoManager Instance
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
						__inst = new PhotoManager();
					}
				}
				return __inst;
			}
		}

		#region ctor

		private PhotoManager()
		{
			// initialize instance
			
		}

		#endregion

		#endregion

		#region properties and fields

		#region const and read only

		private readonly object _sync = new object();
		private readonly Dictionary<string, PhotoInfoInternal> _photos = new Dictionary<string, PhotoInfoInternal>();

		#endregion

		#endregion

		#region public methods

		public PhotoInfo GetPhotoTexture(string path)
		{
			lock (_sync)
			{
				if (_photos.ContainsKey(path))
				{
					return GetPhotoTextureInternal(path);
				}
				return CreatePhotoTexture(path);
			}
		}

		public void ReleasePhotoTexture(string path)
		{
			lock (_sync)
			{
				if (_photos.ContainsKey(path))
				{
					var pi = _photos[path];
					if (pi.ReleaseRef() <= 0)
					{
						_photos.Remove(path);
					}
				}
			}
		}

		#endregion

		#region helpers

		private PhotoInfo GetPhotoTextureInternal(string path)
		{
			var info = _photos[path];
			info.AddRef();
			
			return info.PhotoInfo;
		}

		private PhotoInfo CreatePhotoTexture(string path)
		{
			var pi = PhotoInfoInternal.Create(path);
			_photos[path] = pi;
			return pi.PhotoInfo;
		}

		#endregion
	}
}
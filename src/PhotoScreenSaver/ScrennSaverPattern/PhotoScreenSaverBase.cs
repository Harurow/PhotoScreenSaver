using System;
using System.Threading;
using System.Windows;
using System.Windows.Media.Media3D;
using PhotoScreenSaver.Lib;
using PhotoScreenSaver.Model;

namespace PhotoScreenSaver.ScrennSaverPattern
{
	public abstract class PhotoScreenSaverBase : IPhotoScreenSaver
	{
		#region properties and fields

		protected Timer _timer;

		public TimeSpan FireInterval { get; set; }

		protected readonly RandomEx Random;

		protected readonly Window Owner;

		protected readonly Model3DGroup Model3DGroup;

		public double WorldWidth { get; set; }

		public double WorldHeight { get; set; }

		#endregion

		#region ctor

		protected PhotoScreenSaverBase(Window owner, Model3DGroup model3DGroup, double worldWidth, double worldHeight)
		{
			Random = RandomEx.Instance;

			Owner = owner;
			Model3DGroup = model3DGroup;
			WorldWidth = worldWidth;
			WorldHeight = worldHeight;
		}

		#endregion

		#region public methods

		public void Start()
		{
			if (_timer != null)
			{
				throw new InvalidOperationException();
			}
			_timer = new Timer(FireInternal, null, TimeSpan.Zero, FireInterval);
		}

		public void Stop()
		{
			var timer = _timer;
			_timer = null;
			if (timer != null)
			{
				timer.Dispose();
			}
		}

		#endregion

		#region helpers

		private void FireInternal(object state)
		{
			var path = PhotoList.Instance.GetNext();
			if (!string.IsNullOrEmpty(path))
			{
				Owner.Dispatcher.BeginInvoke((Action<string>)FireCaller, path);
			}
		}

		private async void FireCaller(string path)
		{
			var mesh = PhotoGeometry.Instance.GetGeometry();
			var texture = await TextureManager.Instance.CreateTextureAsync(path);
			var photo = PhotoModel.CreatePhotoModel(mesh, texture);
			Fire(photo);
		}

		#endregion

		#region abstract methods

		protected abstract void Fire(PhotoModel photo);

		#endregion
	}
}
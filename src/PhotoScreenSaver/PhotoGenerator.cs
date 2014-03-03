using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace PhotoScreenSaver
{
	public static class PhotoGenerator
	{
		public static string TestPath = @"C:\Users\Public\Pictures\Sample Pictures\koala.jpg";

		private static readonly object _sync = new object();
		private static MeshGeometry3D _fwMeth;

		private static int _count;
		public static GeometryModel3D CreatePhoto(string imagePath)
		{
			var photo = PhotoManager.Instance.GetPhotoTexture(imagePath);

			var geo = new GeometryModel3D
			{
				Geometry = CreateMesh(),
				Material = photo.Material
			};

			var w = photo.Width;
			var h = photo.Height;

			var rand = RandomEx.Instance;
			var transGroup = new Transform3DGroup();
			var angle = rand.NextStandard() * 120 - 60;
			
			// 画像のアスペクト比に合わせる
			if (h > w)
			{
				transGroup.Children.Add(new ScaleTransform3D(w / h, 1, 0));
			}
			else if (w > h)
			{
				transGroup.Children.Add(new ScaleTransform3D(1, h / w, 1));
			}

			// 傾き
			var r = new AxisAngleRotation3D(new Vector3D(0, 0, 1), angle);
			transGroup.Children.Add(new RotateTransform3D(r));

			//transGroup.Children.Add(new TranslateTransform3D(rand.NextDouble() * 6 - 3, -3, 0));

			geo.Transform = transGroup;
			_count++;
			
		
			return geo;
		}

		private static MeshGeometry3D CreateMesh()
		{
			if (_fwMeth == null)
			{
				lock (_sync)
				{
					if (_fwMeth == null)
					{
						var mesh = new MeshGeometry3D
						{
							Positions = new Point3DCollection(new[]
							{
								new Point3D(-1, -1, 0),
								new Point3D(1, -1, 0),
								new Point3D(1, 1, 0),
								new Point3D(-1, 1, 0)
							}),
							TriangleIndices = new Int32Collection(new[] {0, 1, 2, 0, 2, 3}),
							TextureCoordinates = new PointCollection(new[]
							{
								new Point(-1, 1),
								new Point(1, 1),
								new Point(1, -1),
								new Point(-1, -1)
							})
						};
						_fwMeth = mesh;
					}
				}
			}
			return _fwMeth;
		}
	}
}
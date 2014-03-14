using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace PhotoScreenSaver.Model
{
	public class PhotoGeometry
	{
		#region singleton

		private static readonly object InstSync = new object();

		private static volatile PhotoGeometry __inst;

		public static PhotoGeometry Instance
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
						__inst = new PhotoGeometry();
					}
				}
				return __inst;
			}
		}

		#region ctor

		private PhotoGeometry()
		{
			// initialize instance

		}

		#endregion

		#endregion

		#region properties and fields

		#region const and read only

		private readonly object _sync = new object();

		#endregion

		private volatile MeshGeometry3D _mesh;

		#endregion

		#region public

		public MeshGeometry3D GetGeometry()
		{
			if (_mesh != null)
			{
				return _mesh;
			}

			lock (_sync)
			{
				if (_mesh != null)
				{
					return _mesh;
				}

				_mesh = new MeshGeometry3D
				{
					Positions = new Point3DCollection(new[]
					{
						new Point3D(-0.5, 0.5, 0),
						new Point3D(-0.5, -0.5, 0),
						new Point3D(0.5, -0.5, 0),
						new Point3D(0.5, 0.5, 0)

					}),
					TriangleIndices = new Int32Collection(new[] {0, 1, 2, 0, 2, 3}),
					TextureCoordinates = new PointCollection(new[]
					{
						new Point(0, 0),
						new Point(0, 1),
						new Point(1, 1),
						new Point(1, 0)
					})
				};
			}
			return _mesh;
		}

		#endregion
	}
}
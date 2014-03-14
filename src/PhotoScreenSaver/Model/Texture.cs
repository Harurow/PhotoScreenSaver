using System.Windows.Media.Media3D;

namespace PhotoScreenSaver.Model
{
	public class Texture
	{
		#region properties and fields

		#region const and read only

		public readonly string Path;
		public readonly DiffuseMaterial Material;
		public readonly double Width;
		public readonly double Height;

		#endregion

		#endregion

		public Texture(string path, DiffuseMaterial material, double w, double h)
		{
			Path = path;
			Material = material;
			Width = w;
			Height = h;
		}
	}
}
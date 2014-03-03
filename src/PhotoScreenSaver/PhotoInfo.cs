using System.Windows.Media.Media3D;

namespace PhotoScreenSaver
{
	public class PhotoInfo
	{
		public readonly DiffuseMaterial Material;
		public readonly double Width;
		public readonly double Height;

		public PhotoInfo(DiffuseMaterial material, double width, double height)
		{
			Material = material;
			Width = width;
			Height = height;
		}
	}
}
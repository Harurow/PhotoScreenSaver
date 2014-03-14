using System.Windows.Media.Media3D;

namespace PhotoScreenSaver.Model
{
	public class PhotoModel
	{
		private static readonly object Sync = new object();
		private static int _nameSeed;

		public readonly string ModelName;
		public readonly MeshGeometry3D Mesh;
		public readonly Texture Texture;
		public readonly GeometryModel3D Model;

		private PhotoModel(MeshGeometry3D mesh, Texture texture, GeometryModel3D model)
		{
			lock (Sync)
			{
				ModelName = string.Format("PHOTO_{0:X}", _nameSeed++);
			}

			Mesh = mesh;
			Texture = texture;
			Model = model;
		}

		public static PhotoModel CreatePhotoModel(MeshGeometry3D mesh, Texture texture)
		{
			var model = new GeometryModel3D(mesh, texture.Material);

			var tg = new Transform3DGroup();
			model.Transform = tg;

			var scaleX = 1.0;
			var scaleY = 1.0;
			if (texture.Width < texture.Height)
			{
				scaleX = texture.Width / texture.Height;
			}
			else
			{
				scaleY = texture.Height / texture.Width;
			}
			tg.Children.Add(new ScaleTransform3D(scaleX, scaleY, 1.0));

			return new PhotoModel(mesh, texture, model);
		}
	}
}
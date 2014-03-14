using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;
using PhotoScreenSaver.Model;

namespace PhotoScreenSaver.ScrennSaverPattern
{
	public class SnapScreenSaver : PhotoScreenSaverBase
	{
		#region properties and fields

		private readonly KeySpline AnimationKeySpline = new KeySpline(0, 0, 0, 0.9);

		#region const

		#endregion

		public double MaxAngle = 90.0;
		public double FireTimeSpan = 5.0;
		public double InTimeSpan = 2;
		public double OutTimeSpan = 30.0;

		#endregion

		#region ctor

		public SnapScreenSaver(Window owner, Model3DGroup model3DGroup, double worldWidth, double worldHeight)
			: base(owner, model3DGroup, worldWidth, worldHeight)
		{
			FireInterval = TimeSpan.FromSeconds(FireTimeSpan);
		}

		#endregion

		#region override methods

		protected override void Fire(PhotoModel photo)
		{
			Owner.Dispatcher.BeginInvoke(
				new Action(() =>
				{
					Owner.RegisterName(photo.ModelName, photo.Model);

					var tg = (Transform3DGroup)photo.Model.Transform;
					CreateTransform(tg);

					var sb = CreateStoryboard(photo);

					Model3DGroup.Children.Add(photo.Model);
					Owner.BeginStoryboard(sb);
				}));
		}

		#endregion

		#region helpers

		private double NextStandard(double factor)
		{
			return Random.NextStandard()*factor - factor/2.0;
		}

		private double NextAngle
		{
			get
			{
				return NextStandard(MaxAngle);
			}
		}

		private Transform3D CreateAngleTransform()
		{
			return new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 0, 1), NextAngle));
		}

		private Transform3D CreateScatterTransform()
		{
			return new TranslateTransform3D(NextStandard(WorldWidth), WorldHeight / -2, 0);
		}

		private void CreateTransform(Transform3DGroup owner)
		{
			owner.Children.Add(CreateAngleTransform());
			owner.Children.Add(CreateScatterTransform());
		}

		private void AddScrollKeyFrames(Storyboard sb, PhotoModel photo)
		{
			var keyFrames = new DoubleAnimationUsingKeyFrames();
			sb.Children.Add(keyFrames);
			keyFrames.KeyFrames.Add(new SplineDoubleKeyFrame(0, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(InTimeSpan)), AnimationKeySpline));
			keyFrames.KeyFrames.Add(new EasingDoubleKeyFrame(WorldHeight, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(OutTimeSpan))));
			Storyboard.SetTargetProperty(keyFrames, new PropertyPath("(Model3D.Transform).(Transform3DGroup.Children)[2].(TranslateTransform3D.OffsetY)"));
			Storyboard.SetTargetName(keyFrames, photo.ModelName);
		}

		private void AddFadeInFadeOutKeyFrames(Storyboard sb, PhotoModel photo)
		{
			photo.Texture.Material.Color = Colors.Transparent;

			var keyFrames = new ColorAnimationUsingKeyFrames();
			sb.Children.Add(keyFrames);
			keyFrames.KeyFrames.Add(new SplineColorKeyFrame(Colors.White, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(InTimeSpan)), AnimationKeySpline));
			keyFrames.KeyFrames.Add(new LinearColorKeyFrame(Colors.White, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(OutTimeSpan - InTimeSpan))));
			keyFrames.KeyFrames.Add(new SplineColorKeyFrame(Colors.Transparent, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(OutTimeSpan)), AnimationKeySpline));

			Storyboard.SetTargetProperty(keyFrames, new PropertyPath("(GeometryModel3D.Material).(DiffuseMaterial.Color)"));
			Storyboard.SetTargetName(keyFrames, photo.ModelName);
		}

		private Storyboard CreateStoryboard(PhotoModel photo)
		{
			var sb = new Storyboard();

			AddScrollKeyFrames(sb, photo);
			AddFadeInFadeOutKeyFrames(sb, photo);

			sb.Completed += (sender, args) =>
			{
				Model3DGroup.Children.Remove(photo.Model);
				Owner.UnregisterName(photo.ModelName);
			};

			return sb;
		}

		#endregion
	}
}
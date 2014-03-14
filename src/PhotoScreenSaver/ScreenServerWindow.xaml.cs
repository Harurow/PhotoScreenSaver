using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;
using PhotoScreenSaver.Lib;
using PhotoScreenSaver.ScrennSaverPattern;
using Screen = System.Windows.Forms.Screen;
using Application = System.Windows.Application;
using Cursors = System.Windows.Input.Cursors;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using Point = System.Windows.Point;
using SystemInformation = System.Windows.Forms.SystemInformation;

namespace PhotoScreenSaver
{
	/// <summary>
	/// ScreenServerWindow.xaml の相互作用ロジック
	/// </summary>
	public partial class ScreenServerWindow
	{
		#region properties and fields

		#region const and read only

		private readonly Duration CameraAnimateDuration = new Duration(TimeSpan.FromSeconds(0.5));

		private readonly IEasingFunction CameraAnimationFunction = new CubicEase();

		#endregion

		public double WorldWidth { get; private set; }

		public double WorldHeight { get; private set; }

		private bool ShowDateTime{ get; set; }

		private IPhotoScreenSaver _screenSaver;

		#endregion

		public ScreenServerWindow()
		{
			InitializeComponent();
			Cursor = Cursors.None;

			var now = DateTime.Now;
			var time = TimeFormatForScreenSaver.GetTimeString(now);
			var date = TimeFormatForScreenSaver.GetDateString(now);

			ShowDateTime = true;
			DateTime1Time.Text = time;
			DateTime1Date.Text = date;

			if (!ShowDateTime)
			{
				DateTime1.Visibility = Visibility.Hidden;
				DateTime2.Visibility = Visibility.Hidden;
			}
			else
			{
				DateTime1.Opacity = 0.75;
				DateTime2.Opacity = 0;
			}
			DateTimeTimer.Instance.ChangedTime += OnChangedDateTime;
		}

		public ScreenServerWindow(Screen screen)
			: this()
		{
			Topmost = true;
			Left = screen.Bounds.Left;
			Top = screen.Bounds.Top;
			ShowDateTime = screen.Primary;

			if (Topmost)
			{
#if (!DEBUG)
				WindowState = WindowState.Maximized;
#else
				ResizeMode = ResizeMode.CanResizeWithGrip;
#endif
			}
		}

		protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
		{
			base.OnRenderSizeChanged(sizeInfo);
			CalcWorldSize();
		}

		private void CalcWorldSize()
		{
			double w, h;
			Util.CalcWorldSize(Camera.FieldOfView, Camera.Position.Z,
				View.ActualWidth, View.ActualHeight, out w, out h);
			WorldWidth = w;
			WorldHeight = h;
		}

		private void ShutdownApplication()
		{
			Close();
			Application.Current.Shutdown();
		}

		private void Window_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.IsDown)
			{
				if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
				{
					return;
				}

				if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
				{
					var fov = 0.0;

					if (e.Key == Key.Up)
					{
						if (10 < Camera.FieldOfView)
						{
							fov = Math.Max(10.0, Camera.FieldOfView - 10.0);
						}
					}
					else if (e.Key == Key.Down)
					{
						if (Camera.FieldOfView < 90)
						{
							fov = Math.Min(90.0, Camera.FieldOfView + 10.0);
						}
					}
					else
					{
						ShutdownApplication();
						return;
					}

					if (Math.Abs(Camera.FieldOfView - fov) > 0.001)
					{
						Camera.BeginAnimation(PerspectiveCamera.FieldOfViewProperty,
							new DoubleAnimation(Camera.FieldOfView, fov, CameraAnimateDuration)
							{ EasingFunction = CameraAnimationFunction });
					}
					return;
				}

				var distance = 0.1;
				var x = Camera.Position.X;
				var y = Camera.Position.Y;
				switch (e.Key)
				{
					case Key.Up:
						y = Math.Max(-2, y - distance);
						break;
					case Key.Down:
						y = Math.Min(2, y + distance);
						break;
					case Key.Left:
						x = Math.Min(2, x + distance);
						break;
					case Key.Right:
						x = Math.Max(-2, x - distance);
						break;
					default:
						ShutdownApplication();
						break;
				}
				
				Camera.BeginAnimation(
					ProjectionCamera.PositionProperty,
					new Point3DAnimation(
						Camera.Position,
						new Point3D(x, y, Camera.Position.Z),
						CameraAnimateDuration)
						{ EasingFunction = CameraAnimationFunction });
			}
		}

		private Point? _lastPosition;
		private void Window_MouseMove(object sender, MouseEventArgs e)
		{
			if (_lastPosition == null)
			{
				_lastPosition = e.GetPosition(this);
			}
			else
			{
				var curPos = e.GetPosition(this);
				
				if (Math.Abs(_lastPosition.Value.X - curPos.X) > SystemInformation.DragSize.Width
					|| Math.Abs(_lastPosition.Value.Y - curPos.Y) > SystemInformation.DragSize.Height)
				{
#if (!DEBUG)
					ShutdownApplication();
#endif
				}
			}
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
		}

		private void RootObject_Loaded(object sender, RoutedEventArgs e)
		{
			CalcWorldSize();

			_screenSaver = new SnapScreenSaver(this, ModelGroup, WorldWidth, WorldHeight);
			_screenSaver.Start();
		}

		private void OnChangedDateTime(object sender, DateTimeEventArgs e)
		{
			if (ShowDateTime)
			{
				var time = TimeFormatForScreenSaver.GetTimeString(e.DateTime);
				var date = TimeFormatForScreenSaver.GetDateString(e.DateTime);

				Dispatcher.BeginInvoke(
					new Action(()=>{
							string storyBoardName;
							if (DateTime1.Opacity < 0.75)
							{
								DateTime1Time.Text = time;
								DateTime1Date.Text = date;
								storyBoardName = "ChangeDateTime2To1";
							}
							else
							{
								DateTime2Time.Text = time;
								DateTime2Date.Text = date;
								storyBoardName = "ChangeDateTime1To2";
							}
							var story = (Storyboard)FindResource(storyBoardName);
							BeginStoryboard(story);
					}));
			}
		}
	}
}

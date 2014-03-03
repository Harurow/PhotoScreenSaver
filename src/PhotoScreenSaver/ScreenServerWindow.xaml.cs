using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;
using PhotoScreenSaver.Lib;
using Screen = System.Windows.Forms.Screen;
using Application = System.Windows.Application;
using Cursors = System.Windows.Input.Cursors;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using Point = System.Windows.Point;
using SystemInformation = System.Windows.Forms.SystemInformation;
using System.Collections.Generic;

namespace PhotoScreenSaver
{
	/// <summary>
	/// ScreenServerWindow.xaml の相互作用ロジック
	/// </summary>
	public partial class ScreenServerWindow
	{
		private readonly List<GeometryModel3D> _photos = new List<GeometryModel3D>();

		private bool ShowDateTime{ get; set; }

		public ScreenServerWindow()
		{
			InitializeComponent();
			Cursor = Cursors.None;

			var now = DateTime.Now;
			var time = TimeFormatForScreenSaver.GetTimeString(now);
			var date = TimeFormatForScreenSaver.GetDateString(now);

			DateTime1Time.Text = time;
			DateTime1Date.Text = date;
		}

		public ScreenServerWindow(Screen screen)
			: this()
		{
			Topmost = true;
			Left = screen.Bounds.Left;
			Top = screen.Bounds.Top;
			ShowDateTime = screen.Primary;

			DateTimeTimer.Instance.ChangedTime += OnChangedDateTime;

			if (!ShowDateTime)
			{
				DateTime1.Visibility = Visibility.Hidden;
				DateTime2.Visibility = Visibility.Hidden;
			}
			else
			{
				DateTime1.Opacity = 0.5;
				DateTime2.Opacity = 0;
			}

			if (Topmost)
			{
#if (!DEBUG)
				WindowState = WindowState.Maximized;
#else
				ResizeMode = ResizeMode.CanResizeWithGrip;
#endif
			}
		}

		#region Overrides of FrameworkElement

		protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
		{
			base.OnRenderSizeChanged(sizeInfo);

			var sb = new StringBuilder();

			sb.AppendFormat("ActualW:{0}, ActualH:{1}", View.ActualWidth, View.ActualHeight);
			sb.AppendLine();


			if (_photos.Count > 0)
			{
				var photo = _photos[0];
				sb.AppendFormat("X:{0}, Y:{1}, Z:{2}, SX:{3}, SY:{4}, SZ{5}",
					photo.Bounds.X, photo.Bounds.X, photo.Bounds.Z,
					photo.Bounds.SizeX, photo.Bounds.SizeY, photo.Bounds.Z);
			}

			DebugText.Text = sb.ToString();
		}

		#endregion

		private void ShutdownApplication()
		{
			Close();
			Application.Current.Shutdown();
		}

		private void Window_KeyDown(object sender, KeyEventArgs e)
		{
			ShutdownApplication();
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
			var q = Enumerable.Range(0, 32).Select(
				n =>
				{
					var photo = PhotoGenerator.CreatePhoto(PhotoGenerator.TestPath);
					ModelGroup.Children.Add(photo);
					return photo;
				});

			_photos.AddRange(q);
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
							if (DateTime1.Opacity < 0.5)
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

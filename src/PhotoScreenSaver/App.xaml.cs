using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using PhotoScreenSaver.Lib;
using MessageBox = System.Windows.MessageBox;
using Screen = System.Windows.Forms.Screen;

namespace PhotoScreenSaver
{

	/// <summary>
	/// App.xaml の相互作用ロジック
	/// </summary>
	public partial class App
	{
		private void Application_Startup(object sender, StartupEventArgs e)
		{
			switch (Util.GetScreenSaverBehavior())
			{
				case ScreenSaverBehavior.ShowConfigure:
					var configWindow = new ConfigWindow();
					configWindow.Show();
					break;

				case ScreenSaverBehavior.ShowPreview:
					var preParent = Util.GetPreviewHandle();
					var prev = new ScreenServerWindow();
					preParent.RootVisual = prev.RootObject;
					break;

				case ScreenSaverBehavior.RunScreenSaver:
					foreach (var screen in Screen.AllScreens)
					{
						var wnd = new ScreenServerWindow(screen);
						wnd.Show();
					}
					break;
				
				default:
					MessageBox.Show("引数が無効です.");
					Shutdown();
					break;
			}
		}
	}
}

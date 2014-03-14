using System;
using System.Windows;
using System.Windows.Interop;

namespace PhotoScreenSaver.Lib
{
	public class Util
	{
		static Util()
		{
			PreviewHandle = IntPtr.Zero;
		}

		/// <summary>
		/// コマンドラインの引数から指定されたスクリーンセーバの動作を取得する
		/// </summary>
		/// <returns></returns>
		public static ScreenSaverBehavior GetScreenSaverBehavior()
		{
			var args = Environment.GetCommandLineArgs();
			if (args.Length <= 1)
			{
				return ScreenSaverBehavior.RunScreenSaver;
			}

			var arg = args[1].ToLower();
			if (arg.StartsWith("/c"))
			{
				return ScreenSaverBehavior.ShowConfigure;
			}

			if (arg == "/p" && args.Length >= 3)
			{
				PreviewHandle = new IntPtr(uint.Parse(args[2]));
				return ScreenSaverBehavior.ShowPreview;
			}

			if (arg == "/s")
			{
				return ScreenSaverBehavior.RunScreenSaver;
			}
			return ScreenSaverBehavior.Invalid;
		}

		
		public static IntPtr PreviewHandle { get; private set; }

		public static HwndSource GetPreviewHandle()
		{
			var wnd = PreviewHandle;
			if (wnd == IntPtr.Zero)
			{
				return null;
			}

			Int32Rect rect;
			if (!NativeMethods.GetClientRect(wnd, out rect))
			{
				throw new ApplicationException("プレビューウィンドウの領域取得に失敗しました。");
			}

			var param = new HwndSourceParameters("ScreenSaverPreview");
			param.SetPosition(0, 0);
			param.SetSize(rect.Width, rect.Height);
			param.ParentWindow = wnd;
			param.WindowStyle = (int)(NativeWindowStyle.WS_VISIBLE
								| NativeWindowStyle.WS_CHILD
								| NativeWindowStyle.WS_CLIPCHILDREN);

			var prev = new HwndSource(param);

			prev.Disposed += (s, e) => Application.Current.Shutdown();

			return prev;
		}

		public static void CalcWorldSize(double fieldOfView, double z,
			double actualWidth, double actualHeight,
			out double worldWidth, out double worldHeight)
		{
			var theta = fieldOfView / 2;
			var rad = Math.PI * theta / 180.0;

			worldWidth = z * Math.Tan(rad) * 2;
			worldHeight = actualHeight * worldWidth / actualWidth;
		}
	}
}

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

			return new HwndSource(param);
		}
	}
}

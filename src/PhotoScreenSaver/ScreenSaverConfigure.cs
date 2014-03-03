using System;
using Microsoft.Win32;
using Path = System.IO.Path;

namespace PhotoScreenSaver
{
	public class ScreenSaverConfigure
	{
		#region singleton

		private static readonly object InstSync = new object();

		private static volatile ScreenSaverConfigure __inst;

		public static ScreenSaverConfigure Instance
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
						__inst = new ScreenSaverConfigure();
					}
				}
				return __inst;
			}
		}

		#region ctor

		private ScreenSaverConfigure()
		{
			// initialize instance
			Load();
		}

		#endregion

		#endregion

		#region properties and fields

		#region const and read only

		private const string Root = @"Software\Harurow\SlideshowScreenSaver";

		#endregion

		public string PhotoPath { get; set; }

		public string Culture { get; private set; }

		public string TimeFormat { get; private set; }

		public string DateFormat { get; private set; }

		#endregion

		#region public methods

		public void Load()
		{
			using(var key = CreateKey(@"PhotoDir"))
			{
				if (key == null)
				{
					throw new InvalidOperationException();
				}

				PhotoPath = (string)key.GetValue("Path1", Environment.GetFolderPath(Environment.SpecialFolder.MyPictures));
			}
			using (var key = CreateKey("DateTimeFormat"))
			{
				Culture = (string)key.GetValue("Culture", "en-US");
				DateFormat = (string) key.GetValue("DateFormat", "dddd MMMM d");
				TimeFormat = (string) key.GetValue("TimeFormat", "H:mm");
			}
		}

		public void Save()
		{
			using(var key = CreateKey(@"PhotoDir"))
			{
				if (key == null)
				{
					throw new InvalidOperationException();
				}

				key.SetValue("Path1", PhotoPath, RegistryValueKind.String);
			}
		}

		#endregion

		#region helpers

		private RegistryKey CreateKey(string subPath)
		{
			return Registry.CurrentUser.CreateSubKey(Path.Combine(Root, subPath));
		}

		#endregion
	}
}
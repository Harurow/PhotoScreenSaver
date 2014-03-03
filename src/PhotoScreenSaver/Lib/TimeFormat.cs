using System;
using System.Globalization;

namespace PhotoScreenSaver.Lib
{
	public static class TimeFormatForScreenSaver
	{
		public readonly static CultureInfo Culture = new CultureInfo("en-US");
		private readonly static string TimeFormat;
		private readonly static string DateFormat;

		static TimeFormatForScreenSaver()
		{
			var conf = ScreenSaverConfigure.Instance;

			Culture = new CultureInfo(conf.Culture);
			TimeFormat = conf.TimeFormat;
			DateFormat = conf.DateFormat;
		}

		public static string GetTimeString(DateTime time)
		{
			return time.ToString(TimeFormat, Culture);
		}

		public static string GetDateString(DateTime time)
		{
			return time.ToString(DateFormat, Culture);
		}
	}
}
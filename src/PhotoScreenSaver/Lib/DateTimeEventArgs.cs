using System;

namespace PhotoScreenSaver.Lib
{
	public class DateTimeEventArgs : EventArgs
	{
		public readonly DateTime DateTime;

		public DateTimeEventArgs(DateTime dateTime)
		{
			DateTime = dateTime;
		}
	}
}
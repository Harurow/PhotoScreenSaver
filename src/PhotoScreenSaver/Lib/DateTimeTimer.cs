using System;
using System.Threading;

namespace PhotoScreenSaver.Lib
{
	public sealed class DateTimeTimer
	{
		#region singleton

		private static readonly object InstSync = new object();

		private static volatile DateTimeTimer __inst;

		public static DateTimeTimer Instance
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
						__inst = new DateTimeTimer();
					}
				}
				return __inst;
			}
		}

		#region ctor

		#endregion

		#endregion

		#region ChangedTime

		private event EventHandler<DateTimeEventArgs> _changedTime;

		public event EventHandler<DateTimeEventArgs> ChangedTime
		{
			add { _changedTime += value; }
			remove { _changedTime -= value; }
		}

		private void OnChangedTime(DateTimeEventArgs e)
		{
			var handler = _changedTime;
			if (handler != null)
			{
				handler(this, e);
			}
		}

		#endregion

		#region event hanlders

		private void OnTimer(object obj)
		{
			OnChangedTime(new DateTimeEventArgs(_nextTime));
			_timer.Change(CalcDue(DateTime.Now), Timeout.InfiniteTimeSpan);
		}

		#endregion

		#region helpers

		private DateTime _nextTime;
		private TimeSpan CalcDue(DateTime cur)
		{
			var now = new DateTime(cur.Year, cur.Month, cur.Day, cur.Hour, cur.Minute, 0);
			_nextTime = now.AddMinutes(1);
			return (_nextTime - DateTime.Now);
		}

		private readonly Timer _timer;
		private DateTimeTimer()
		{
			// initialize instance
			_timer = new Timer(OnTimer, null, CalcDue(DateTime.Now), Timeout.InfiniteTimeSpan);
		}

		#endregion
	}
}
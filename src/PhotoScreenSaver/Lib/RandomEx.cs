using System;
using System.Linq;

namespace PhotoScreenSaver.Lib
{
	public sealed class RandomEx : Random
	{
		#region singleton

		private static readonly object InstSync = new object();

		private static volatile RandomEx __inst;

		public static RandomEx Instance
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
						__inst = new RandomEx();
					}
				}
				return __inst;
			}
		}

		#region ctor

		private RandomEx()
			: base(Environment.TickCount)
		{
			// initialize instance
		}

		#endregion

		#endregion

		/// <summary>
		/// 正規分布乱数
		/// </summary>
		/// <returns></returns>
		public double NextStandard()
		{
			var sum = Enumerable.Range(0, 12).Sum(n => NextDouble());
			return sum/12;
		}
	}
}
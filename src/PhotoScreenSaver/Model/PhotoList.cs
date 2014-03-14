using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using PhotoScreenSaver.Lib;

namespace PhotoScreenSaver.Model
{
	public class PhotoList
	{
		#region singleton

		private static readonly object InstSync = new object();

		private static volatile PhotoList __inst;

		public static PhotoList Instance
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
						__inst = new PhotoList();
					}
				}
				return __inst;
			}
		}

		#region ctor

		private PhotoList()
		{
			// initialize instance
			Random = RandomEx.Instance;
		}

		#endregion

		#endregion

		#region properties and fields

		private readonly RandomEx Random;
		private string[] _photoPath;

		#endregion

		#region public method

		public void Reset()
		{
			lock (InstSync)
			{
				_photoPath = null;
			}
		}

		public string GetNext()
		{
			if (_photoPath == null)
			{
				lock (InstSync)
				{
					if (_photoPath == null)
					{
						var path = ScreenSaverConfigure.Instance.PhotoPath;
						_photoPath = Directory.GetFiles(path).Where(
							s => Regex.IsMatch(s, @"(\.bmp|\.png|\.jpg|\.jpeg|\.jpe|\.gif|\.tif|\.tiff)$", RegexOptions.IgnoreCase)).ToArray();
					}
				}
			}

			if (_photoPath.Length == 0)
			{
				return string.Empty;
			}

			var index = Random.Next() % _photoPath.Length;
			
			return _photoPath[index];
		}

		#endregion
	}
}
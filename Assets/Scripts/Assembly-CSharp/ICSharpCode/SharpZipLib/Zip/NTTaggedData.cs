using System;
using System.IO;

namespace ICSharpCode.SharpZipLib.Zip
{
	public class NTTaggedData : ITaggedData
	{
		private DateTime _lastAccessTime = DateTime.FromFileTime(0L);

		private DateTime _lastModificationTime = DateTime.FromFileTime(0L);

		private DateTime _createTime = DateTime.FromFileTime(0L);

		public short TagID
		{
			get
			{
				return 10;
			}
		}

		public DateTime LastModificationTime
		{
			get
			{
				return _lastModificationTime;
			}
			set
			{
				if (!IsValidValue(value))
				{
					throw new ArgumentOutOfRangeException("value");
				}
				_lastModificationTime = value;
			}
		}

		public DateTime CreateTime
		{
			get
			{
				return _createTime;
			}
			set
			{
				if (!IsValidValue(value))
				{
					throw new ArgumentOutOfRangeException("value");
				}
				_createTime = value;
			}
		}

		public DateTime LastAccessTime
		{
			get
			{
				return _lastAccessTime;
			}
			set
			{
				if (!IsValidValue(value))
				{
					throw new ArgumentOutOfRangeException("value");
				}
				_lastAccessTime = value;
			}
		}

		public void SetData(byte[] data, int index, int count)
		{
			using (MemoryStream stream = new MemoryStream(data, index, count, false))
			{
				using (ZipHelperStream zipHelperStream = new ZipHelperStream(stream))
				{
					zipHelperStream.ReadLEInt();
					while (zipHelperStream.Position < zipHelperStream.Length)
					{
						int num = zipHelperStream.ReadLEShort();
						int num2 = zipHelperStream.ReadLEShort();
						if (num == 1)
						{
							if (num2 >= 24)
							{
								long fileTime = zipHelperStream.ReadLELong();
								_lastModificationTime = DateTime.FromFileTime(fileTime);
								long fileTime2 = zipHelperStream.ReadLELong();
								_lastAccessTime = DateTime.FromFileTime(fileTime2);
								long fileTime3 = zipHelperStream.ReadLELong();
								_createTime = DateTime.FromFileTime(fileTime3);
							}
							break;
						}
						zipHelperStream.Seek(num2, SeekOrigin.Current);
					}
				}
			}
		}

		public byte[] GetData()
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (ZipHelperStream zipHelperStream = new ZipHelperStream(memoryStream))
				{
					zipHelperStream.IsStreamOwner = false;
					zipHelperStream.WriteLEInt(0);
					zipHelperStream.WriteLEShort(1);
					zipHelperStream.WriteLEShort(24);
					zipHelperStream.WriteLELong(_lastModificationTime.ToFileTime());
					zipHelperStream.WriteLELong(_lastAccessTime.ToFileTime());
					zipHelperStream.WriteLELong(_createTime.ToFileTime());
					return memoryStream.ToArray();
				}
			}
		}

		public static bool IsValidValue(DateTime value)
		{
			bool result = true;
			try
			{
				value.ToFileTimeUtc();
			}
			catch
			{
				result = false;
			}
			return result;
		}
	}
}

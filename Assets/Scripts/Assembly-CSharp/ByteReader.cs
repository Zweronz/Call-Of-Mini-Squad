using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class ByteReader
{
	private byte[] mBuffer;

	private int mOffset;

	private static BetterList<string> mTemp = new BetterList<string>();

	public bool canRead
	{
		get
		{
			return mBuffer != null && mOffset < mBuffer.Length;
		}
	}

	public ByteReader(byte[] bytes)
	{
		mBuffer = bytes;
	}

	public ByteReader(TextAsset asset)
	{
		mBuffer = asset.bytes;
	}

	private static string ReadLine(byte[] buffer, int start, int count)
	{
		return Encoding.UTF8.GetString(buffer, start, count);
	}

	public string ReadLine()
	{
		int num = mBuffer.Length;
		while (mOffset < num && mBuffer[mOffset] < 32)
		{
			mOffset++;
		}
		int num2 = mOffset;
		if (num2 < num)
		{
			int num3 = 0;
			do
			{
				if (num2 < num)
				{
					num3 = mBuffer[num2++];
					continue;
				}
				num2++;
				break;
			}
			while (num3 != 10 && num3 != 13);
			string result = ReadLine(mBuffer, mOffset, num2 - mOffset - 1);
			mOffset = num2;
			return result;
		}
		mOffset = num;
		return null;
	}

	public Dictionary<string, string> ReadDictionary()
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		char[] separator = new char[1] { '=' };
		while (canRead)
		{
			string text = ReadLine();
			if (text == null)
			{
				break;
			}
			if (!text.StartsWith("//"))
			{
				string[] array = text.Split(separator, 2, StringSplitOptions.RemoveEmptyEntries);
				if (array.Length == 2)
				{
					string key = array[0].Trim();
					string value = array[1].Trim().Replace("\\n", "\n");
					dictionary[key] = value;
				}
			}
		}
		return dictionary;
	}

	public BetterList<string> ReadCSV()
	{
		mTemp.Clear();
		if (canRead)
		{
			string text = ReadLine();
			if (text == null)
			{
				return null;
			}
			text = text.Replace("\\n", "\n");
			int num = 0;
			bool flag = false;
			int i = 0;
			for (int length = text.Length; i < length; i++)
			{
				switch (text[i])
				{
				case ',':
					if (!flag)
					{
						mTemp.Add(text.Substring(num, i - num));
						num = i + 1;
					}
					break;
				case '"':
					if (flag)
					{
						if (i + 1 >= length)
						{
							mTemp.Add(text.Substring(num, i - num).Replace("\"\"", "\""));
							return mTemp;
						}
						if (text[i + 1] != '"')
						{
							mTemp.Add(text.Substring(num, i - num));
							flag = false;
							if (text[i + 1] == ',')
							{
								i++;
								num = i + 1;
							}
						}
						else
						{
							i++;
						}
					}
					else
					{
						num = i + 1;
						flag = true;
					}
					break;
				}
			}
			if (num < text.Length)
			{
				mTemp.Add(text.Substring(num, text.Length - num));
			}
			return mTemp;
		}
		return null;
	}
}

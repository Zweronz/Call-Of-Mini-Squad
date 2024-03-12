using System;
using UnityEngine;

public class UICommon
{
	public static string StringFormat(string formatString, params object[] args)
	{
		string text = formatString;
		if (args == null)
		{
			throw new ArgumentNullException();
		}
		for (int i = 0; i < args.Length; i++)
		{
			text = text.Replace("{" + i + "}", args[i].ToString());
		}
		return text;
	}

	public static string LocalizationStringFormat(string key, params object[] args)
	{
		return StringFormat(Localization.Localize(key), args);
	}

	public static Rect RectIntersect(Rect rect1, Rect rect2)
	{
		float num = Mathf.Max(rect1.xMin, rect2.xMin);
		float num2 = Mathf.Min(rect1.xMax, rect2.xMax);
		float num3 = Mathf.Max(rect1.yMin, rect2.yMin);
		float num4 = Mathf.Min(rect1.yMax, rect2.yMax);
		float num5 = num2 - num;
		float num6 = num4 - num3;
		return new Rect(num, num3, (!(num5 < 0f)) ? num5 : 0f, (!(num6 < 0f)) ? num6 : 0f);
	}

	public static bool IsRectIntersect(Rect rect1, Rect rect2)
	{
		Rect rect3 = RectIntersect(rect1, rect2);
		return rect3.width > 0f && rect3.height > 0f;
	}
}

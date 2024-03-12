using System.Collections;
using System.Collections.Generic;

public static class ArrayListExtensions
{
	public static List<T> ToList<T>(this ArrayList arrayList)
	{
		List<T> list = new List<T>(arrayList.Count);
		foreach (T array in arrayList)
		{
			list.Add(array);
		}
		return list;
	}
}

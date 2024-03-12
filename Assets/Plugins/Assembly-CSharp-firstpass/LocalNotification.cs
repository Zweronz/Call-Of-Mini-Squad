public class LocalNotification
{
	public static bool Check(string key)
	{
		return true;
	}

	public static void Schedule(string key, string message, string sound, int time, int loopType)
	{
	}

	public static void CancelOne(string key)
	{
	}

	public static void CancelAll()
	{
	}

	public static string[] GetAll()
	{
		return new string[0];
	}
}

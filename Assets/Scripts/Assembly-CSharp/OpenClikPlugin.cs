public class OpenClikPlugin
{
	private enum Status
	{
		kShowBottom = 0,
		kShowFull = 1,
		kShowTop = 2,
		kHide = 3
	}

	public enum Request_Type
	{
		bannerBottom = 0,
		interstitial = 1,
		bannerTop = 2
	}

	private static Status s_Status;

	public static void Initialize(string key)
	{
		s_Status = Status.kHide;
	}

	public static void Request(int type)
	{
	}

	public static void Show(int type)
	{
	}

	public static void Show(bool show_full)
	{
		int type = 0;
		if (show_full)
		{
			type = 1;
		}
		if (s_Status == Status.kHide)
		{
			Show(type);
			if (show_full)
			{
				s_Status = Status.kShowFull;
			}
			else
			{
				s_Status = Status.kShowBottom;
			}
		}
		else if (s_Status == Status.kShowFull)
		{
			if (!show_full)
			{
				Show(type);
				s_Status = Status.kShowBottom;
			}
		}
		else if (s_Status == Status.kShowBottom && show_full)
		{
			Show(type);
			s_Status = Status.kShowFull;
		}
	}

	public static void Hide()
	{
		s_Status = Status.kHide;
	}

	public static bool IsAdReady()
	{
		return true;
	}

	public static bool IsVisible()
	{
		return true;
	}
}

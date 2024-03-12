public class TwitterPlugin
{
	public enum Status
	{
		kNotAllow = -3,
		kNoAccount = -2,
		kError = -1,
		kDoing = 0,
		kSuccess = 1
	}

	public static void sendMsgWithUI(string msg)
	{
	}

	public static void sendMsg(string msg)
	{
	}

	public static Status getSendMsgStatus()
	{
		return Status.kSuccess;
	}

	public static void followUser(string name)
	{
	}

	public static Status getFollowUserStatuss()
	{
		return Status.kSuccess;
	}

	public static bool canTweet()
	{
		return false;
	}
}

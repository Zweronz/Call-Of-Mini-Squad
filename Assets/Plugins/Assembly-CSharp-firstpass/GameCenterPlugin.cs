public class GameCenterPlugin
{
	public enum LOGIN_STATUS
	{
		LOGIN_STATUS_IDLE = 0,
		LOGIN_STATUS_WAIT = 1,
		LOGIN_STATUS_SUCCESS = 2,
		LOGIN_STATUS_ERROR = 3
	}

	public enum SUBMIT_STATUS
	{
		SUBMIT_STATUS_IDLE = 0,
		SUBMIT_STATUS_WAIT = 1,
		SUBMIT_STATUS_SUCCESS = 2,
		SUBMIT_STATUS_ERROR = 3
	}

	public enum FRIEND_STATUS
	{
		FRIEND_LOAD = 0,
		FRIEND_LOADDATA = 1,
		FRIEND_ERROR = 2,
		FRIEND_SUCCESS = 3
	}

	public static void Initialize()
	{
	}

	public static void Uninitialize()
	{
	}

	public static bool IsSupported()
	{
		return true;
	}

	public static bool IsLogin()
	{
		return false;
	}

	public static bool Login()
	{
		return false;
	}

	public static LOGIN_STATUS LoginStatus()
	{
		return LOGIN_STATUS.LOGIN_STATUS_IDLE;
	}

	public static string GetAccount()
	{
		return string.Empty;
	}

	public static string GetName()
	{
		return string.Empty;
	}

	public static bool SubmitScore(string category, int score)
	{
		return false;
	}

	public static SUBMIT_STATUS SubmitScoreStatus(string category, int score)
	{
		return SUBMIT_STATUS.SUBMIT_STATUS_IDLE;
	}

	public static bool SubmitAchievement(string category, int percent)
	{
		return false;
	}

	public static SUBMIT_STATUS SubmitAchievementStatus(string category, int percent)
	{
		return SUBMIT_STATUS.SUBMIT_STATUS_IDLE;
	}

	public static bool OpenLeaderboard()
	{
		return false;
	}

	public static bool OpenLeaderboard(string category)
	{
		return false;
	}

	public static bool OpenAchievement()
	{
		return false;
	}

	public static void LoadFriends()
	{
	}

	public static FRIEND_STATUS GetLoadFriendStatus()
	{
		return FRIEND_STATUS.FRIEND_SUCCESS;
	}

	public static int GetFriendCount()
	{
		return -1;
	}

	public static string GetFriendAccount(int index)
	{
		return string.Empty;
	}
}

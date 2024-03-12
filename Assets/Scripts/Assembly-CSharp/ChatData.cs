public class ChatData
{
	public enum EUSERTYPE
	{
		E_GM = 0,
		E_SystemInfo = 1,
		E_NormalUser = 2
	}

	public string userId = string.Empty;

	public string userName = string.Empty;

	public EUSERTYPE userType = EUSERTYPE.E_NormalUser;

	public string msg = string.Empty;

	public long dateSeconds;

	public static EUSERTYPE GetUserType(string _str)
	{
		EUSERTYPE eUSERTYPE = EUSERTYPE.E_NormalUser;
		if (_str == "gm")
		{
			return EUSERTYPE.E_GM;
		}
		if (_str == "system")
		{
			return EUSERTYPE.E_SystemInfo;
		}
		return EUSERTYPE.E_NormalUser;
	}
}

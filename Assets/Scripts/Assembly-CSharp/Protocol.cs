using System;
using System.Collections;
using LitJson;

public abstract class Protocol
{
	public enum ServerFeedbackCode
	{
		noCrystal = 1000,
		noMoney = 1001,
		noHonor = 1002,
		noItems = 1003,
		noUser = 1004,
		geniusPointsNotEnough = 1005,
		geniusLocked = 1006,
		geniusMaxLevel = 1007,
		geniusPointsMaxBuy = 1008,
		geniusUnlocked = 1009,
		geniusBuyReset = 1010,
		evolutionLocked = 1011,
		evolutionUnlocked = 1012,
		evolutionMaxLevel = 1013,
		weaponLocked = 1014,
		weaponMaxLevel = 1015,
		skillLocked = 1016,
		skillMaxLevel = 1017,
		skillMaxBreakthrough = 1018,
		weaponMaxBreakthrough = 1019,
		helmetLocked = 1020,
		helmetMaxLevel = 1021,
		armorLocked = 1022,
		armorMaxLevel = 1023,
		ornamentLocked = 1024,
		ornamentMaxLevel = 1025,
		helmetUnlocked = 1026,
		armorUnlocked = 1027,
		ornamentUnlocked = 1028,
		noTeamLevel = 1029,
		helmetNoPrevLevel = 1030,
		armorNoPrevLevel = 1031,
		ornamentNoPrevLevel = 1032,
		heroLocked = 1033,
		teamPositionUnlocked = 1034,
		userNameDirty = 1035,
		userNameExist = 1036,
		chapterNotReachable = 1037,
		actionPointsNotEnough = 1038,
		chapterMaxDayTimes = 1039,
		teamPositionLocked = 1040,
		noPvpTimes = 1041,
		noPvpTime = 1042,
		teamLeaderEmpty = 1043,
		completeQuest_notMeet = 1044,
		purchase_iap_fail = 1045,
		purchase_iap_verifing = 1046,
		iapBuyOnce = 1047,
		iapBuyMonthly = 1048,
		purchase_iap_verified = 1049,
		teamPositionNotNextLocked = 1050,
		repeatUserName = 1051,
		purchase_iap_null = 1052,
		invalidUserName = 1053,
		messageTakeItems_taken = 1054,
		messageTakeItems_noItems = 1055,
		mcs_deviceUserAlreadBoundAnotherUser = 1056,
		mcs_deviceUserAlreadBoundThisUser = 1057,
		mcs_userAlreadyRegistered = 1058,
		mcs_userNotExists = 1059,
		mcs_invalidPassword = 1060,
		sendEmailError = 1061,
		kickedOff = 1062
	}

	protected string m_serverName = string.Empty;

	protected string m_protocolAction = string.Empty;

	public string ServerName
	{
		get
		{
			return m_serverName;
		}
	}

	public string ProtocolAction
	{
		get
		{
			return m_protocolAction;
		}
	}

	public virtual void Initialize(string serverName, string protocolAction)
	{
		m_serverName = serverName;
		m_protocolAction = protocolAction;
	}

	public virtual string GetRequest()
	{
		Hashtable obj = new Hashtable();
		return JsonMapper.ToJson(obj);
	}

	public virtual int GetResponse(string response)
	{
		try
		{
			JsonData jsonData = JsonMapper.ToObject(response);
			int num = int.Parse((string)jsonData["code"]);
			if (num != 0)
			{
				return num;
			}
			return 0;
		}
		catch (Exception)
		{
			return -1;
		}
	}

	public int CommonResponse(string response)
	{
		try
		{
			JsonData jsonData = JsonMapper.ToObject(response);
			int num = (int)jsonData["Code"];
			if (num != 0)
			{
				return num;
			}
			return 0;
		}
		catch
		{
			return -1;
		}
	}

	public Defined.COST_TYPE GetCostType(string cost)
	{
		switch (cost)
		{
		case "money":
			return Defined.COST_TYPE.Money;
		case "crystal":
			return Defined.COST_TYPE.Crystal;
		case "honor":
			return Defined.COST_TYPE.Honor;
		default:
			return Defined.COST_TYPE.Money;
		}
	}

	public static int GetCode(JsonData _jsonData)
	{
		int result = -1;
		string text = _jsonData["code"].ToString();
		switch (text)
		{
		case "0":
			result = 0;
			break;
		case "noCrystal":
			result = 1000;
			break;
		case "noMoney":
			result = 1001;
			break;
		case "noHonor":
			result = 1002;
			break;
		case "noItems":
			result = 1003;
			break;
		case "noUser":
			result = 1004;
			break;
		case "geniusPointsNotEnough":
			result = 1005;
			break;
		case "geniusLocked":
			result = 1006;
			break;
		case "geniusMaxLevel":
			result = 1007;
			break;
		case "geniusPointsMaxBuy":
			result = 1008;
			break;
		case "geniusUnlocked":
			result = 1009;
			break;
		case "geniusBuyReset":
			result = 1010;
			break;
		case "evolutionLocked":
			result = 1011;
			break;
		case "evolutionUnlocked":
			result = 1012;
			break;
		case "evolutionMaxLevel":
			result = 1013;
			break;
		case "weaponLocked":
			result = 1014;
			break;
		case "weaponMaxLevel":
			result = 1015;
			break;
		case "skillLocked":
			result = 1016;
			break;
		case "skillMaxLevel":
			result = 1017;
			break;
		case "skillMaxBreakthrough":
			result = 1018;
			break;
		case "weaponMaxBreakthrough":
			result = 1019;
			break;
		case "helmetLocked":
			result = 1020;
			break;
		case "helmetMaxLevel":
			result = 1021;
			break;
		case "armorLocked":
			result = 1022;
			break;
		case "armorMaxLevel":
			result = 1023;
			break;
		case "ornamentLocked":
			result = 1024;
			break;
		case "ornamentMaxLevel":
			result = 1025;
			break;
		case "helmetUnlocked":
			result = 1026;
			break;
		case "armorUnlocked":
			result = 1027;
			break;
		case "ornamentUnlocked":
			result = 1028;
			break;
		case "noTeamLevel":
			result = 1029;
			break;
		case "helmetNoPrevLevel":
			result = 1030;
			break;
		case "armorNoPrevLevel":
			result = 1031;
			break;
		case "ornamentNoPrevLevel":
			result = 1032;
			break;
		case "heroLocked":
			result = 1033;
			break;
		case "teamPositionUnlocked":
			result = 1034;
			break;
		case "userNameDirty":
			result = 1035;
			break;
		case "userNameExist":
			result = 1036;
			break;
		case "chapterNotReachable":
			result = 1037;
			break;
		case "actionPointsNotEnough":
			result = 1038;
			break;
		case "chapterMaxDayTimes":
			result = 1039;
			break;
		case "teamPositionLocked":
			result = 1040;
			break;
		case "noPvpTimes":
			result = 1041;
			break;
		case "noPvpTime":
			result = 1042;
			break;
		case "teamLeaderEmpty":
			result = 1043;
			break;
		case "completeQuest_notMeet":
			result = 1044;
			break;
		case "purchase_iap_fail":
			result = 1045;
			break;
		case "purchase_iap_verifing":
			result = 1046;
			break;
		case "iapBuyOnce":
			result = 1047;
			break;
		case "iapBuyMonthly":
			result = 1048;
			break;
		case "purchase_iap_verified":
			result = 1049;
			break;
		case "teamPositionNotNextLocked":
			result = 1050;
			break;
		case "repeatUserName":
			result = 1051;
			break;
		case "purchase_iap_null":
			result = 1052;
			break;
		case "invalidUserName":
			result = 1053;
			break;
		case "messageTakeItems_taken":
			result = 1054;
			break;
		case "messageTakeItems_noItems":
			result = 1055;
			break;
		case "mcs_deviceUserAlreadBoundAnotherUser":
			result = 1056;
			break;
		case "mcs_deviceUserAlreadBoundThisUser":
			result = 1057;
			break;
		case "mcs_userAlreadyRegistered":
			result = 1058;
			break;
		case "mcs_userNotExists":
			result = 1059;
			break;
		case "mcs_invalidPassword":
			result = 1060;
			break;
		case "sendEmailError":
			result = 1061;
			break;
		case "kickedOff":
			result = 1062;
			break;
		}
		if (text != "0")
		{
		}
		return result;
	}
}

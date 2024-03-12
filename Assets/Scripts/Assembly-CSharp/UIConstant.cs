using System.Collections.Generic;

public class UIConstant
{
	public const float gLocalVersion = 1.01f;

	public const string gStrLocalVersion = "1.0.1";

	public const int UI_NAMECHARMAXLENGTH = 6;

	public const int UI_BAG_HEROMAXCOUNT = 50;

	public const int UI_BAG_HEADMAXCOUNT = 50;

	public const int UI_BAG_BODYMAXCOUNT = 50;

	public const int UI_BAG_ACCESSORIESMAXCOUNT = 50;

	public const int UI_BAG_OTHERSMAXCOUNT = 50;

	public const int UI_STUFF_EXP1Index = 0;

	public const int UI_STUFF_EXP2Index = 1;

	public const int UI_STUFF_EXP3Index = 2;

	public const string UI_SKILL_ATK_TYPE = "[TSkill#atk*]";

	public const string UI_SKILL_BR_TYPE = "[TSkill#br*]";

	public const string UI_SKILL_DR_TYPE = "[TSkill#dr*]";

	public const string UI_SKILL_TargetHitRateDecrease_TYPE = "[TSkill#TargetHitRateDecrease*]";

	public const string UI_SKILL_TargetMoveSpeedDecrease_TYPE = "[TSkill#TargetMoveSpeedDecrease*]";

	public const string UI_SKILL_TargetAtkDecrease_TYPE = "[TSkill#TargetAtkDecrease*]";

	public const string UI_SKILL_MINECOUNT_TYPE = "[TSkill#MINECOUNT*]";

	public const int UserNameMinLength = 4;

	public const int UserNameMaxLength = 15;

	public const string gServerSuffix = "/gameapi/ds2.do";

	public const string gAccountServerSuffix = "/gameapi/ta.do";

	public const string gChatServerSuffix = "/gameapi/ds2.do";

	private static bool bDebug = false;

	public static float ProbeMailSeconds = 15f;

	public static UtilUIShopIAPItemData[] gLSIAPItemsData = null;

	public static PVP_Target[] gLSTargetAreanData = null;

	public static UtilUIArenaTeamDetailInfo.TargetPlayerDetailDataInfo[] gLSTargetDetailData = null;

	public static Dictionary<int, PVP_RankTargetData> gDictRankGlobalTargetData = new Dictionary<int, PVP_RankTargetData>();

	public static Dictionary<int, PVP_RankTargetData> gDictRankLocalTargetData = new Dictionary<int, PVP_RankTargetData>();

	public static Dictionary<int, PVP_RankTargetData> gDictTopTargetData = new Dictionary<int, PVP_RankTargetData>();

	public static PVP_RankTargetData gMyRankDataInfo = null;

	public static int gProbeNewMailsCount = 0;

	public static Dictionary<string, MailData> gDictMailData = new Dictionary<string, MailData>();

	public static List<string> glsMailReaded = new List<string>();

	public static int gMaxMailCount = 50;

	public static Dictionary<Defined.COST_TYPE, int> gDictMailRewardStatistics = new Dictionary<Defined.COST_TYPE, int>();

	public static Dictionary<string, AchievementData> gDictAchievementData = new Dictionary<string, AchievementData>();

	public static string gStrFileContent = string.Empty;

	public static List<ChatData> gLsChatData = new List<ChatData>();

	public static long gLastChatMsgTime = 999999999999999999L;

	public static float gSendMsgPerSeconds = 5f;

	public static int gCouseHeroId = 1;

	public static List<NewUnlockData.E_NewUnlockType> gLsNewUnlockedInfo = new List<NewUnlockData.E_NewUnlockType>();

	public static int gProbeNewAchievementCount = 0;

	public static int gProbeUnreadMsgsCount = 0;

	public static bool bNeedLoseConnect = true;

	public static bool DebugMode
	{
		get
		{
			return bDebug;
		}
		set
		{
			bDebug = value;
		}
	}

	public static int MONEY { get; set; }

	public static int CRYSTAL { get; set; }

	public static int HORNOR { get; set; }

	public static int EXP { get; set; }

	public static int MoneyExchangRate { get; set; }

	public static int HornorExchangRate { get; set; }

	public static int TradeMoneyNotEnough { get; set; }

	public static int TradeHornorNotEnough { get; set; }
}

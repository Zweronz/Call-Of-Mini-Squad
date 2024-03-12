public class NewUnlockData
{
	public enum E_NewUnlockType
	{
		E_None = 0,
		E_Hero = 1,
		E_Equipment = 2,
		E_Genius = 3,
		E_Evolution = 4
	}

	public static E_NewUnlockType GetTypeByString(string _str)
	{
		E_NewUnlockType result = E_NewUnlockType.E_None;
		switch (_str)
		{
		case "unlockHero":
			result = E_NewUnlockType.E_Hero;
			break;
		case "unlockEquipment":
			result = E_NewUnlockType.E_Equipment;
			break;
		case "unlockGeniusButton":
			result = E_NewUnlockType.E_Genius;
			break;
		case "unlockEvolutionButton":
			result = E_NewUnlockType.E_Evolution;
			break;
		}
		return result;
	}
}

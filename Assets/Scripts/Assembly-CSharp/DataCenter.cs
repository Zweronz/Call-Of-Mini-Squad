public class DataCenter
{
	private static DataCenter instance;

	private DataDevice dataDevice;

	private DataConf dataConf;

	private DataSave dataSave;

	private DataState dataState;

	private UserData userData;

	private DataStore dataStore;

	public DataCenter()
	{
		dataDevice = new DataDevice();
		dataConf = new DataConf();
		dataSave = new DataSave();
		dataState = new DataState();
		userData = new UserData();
		dataStore = new DataStore();
	}

	public static DataDevice Device()
	{
		return Instance().GetDataDevice();
	}

	public static DataConf Conf()
	{
		return Instance().GetDataConf();
	}

	public static DataSave Save()
	{
		return Instance().GetDataSave();
	}

	public static DataState State()
	{
		return Instance().GetDataState();
	}

	public static UserData User()
	{
		return Instance().GetUserData();
	}

	public static DataStore Store()
	{
		return Instance().GetDataStore();
	}

	public static void ReloadConf()
	{
		Instance().DoReloadConf();
	}

	private static DataCenter Instance()
	{
		if (instance == null)
		{
			instance = new DataCenter();
		}
		return instance;
	}

	public DataDevice GetDataDevice()
	{
		return dataDevice;
	}

	public DataConf GetDataConf()
	{
		return dataConf;
	}

	public DataSave GetDataSave()
	{
		return dataSave;
	}

	public DataState GetDataState()
	{
		return dataState;
	}

	public UserData GetUserData()
	{
		return userData;
	}

	public DataStore GetDataStore()
	{
		return dataStore;
	}

	public void DoReloadConf()
	{
		dataConf = null;
		dataConf = new DataConf();
	}
}

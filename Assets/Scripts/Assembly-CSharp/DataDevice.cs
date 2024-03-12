public class DataDevice
{
	private string m_accountId;

	private string m_accountToken;

	private string m_gameCenterAccountId;

	public string GetDeviceToken()
	{
		return "device_chengpu02";
	}

	public string GetAccountId()
	{
		return m_accountId;
	}

	public void SetAccountId(string accountId)
	{
		m_accountId = accountId;
	}

	public string GetAccountToken()
	{
		return m_accountToken;
	}

	public void SetAccountToken(string accountToken)
	{
		m_accountToken = accountToken;
	}

	public string GetGameCenterToken()
	{
		return "gamecenter_chengpu01";
	}

	public string GetGameCenterAccountId()
	{
		return m_gameCenterAccountId;
	}

	public void SetGameCenterAccountId(string accountId)
	{
		m_gameCenterAccountId = accountId;
	}
}

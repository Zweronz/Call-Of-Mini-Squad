using UnityEngine;

public class UtilAccountData : MonoBehaviour
{
	public class TAccountInfo
	{
		public string deviceid = string.Empty;

		public string email = string.Empty;

		public string password = string.Empty;

		public string uuid = string.Empty;
	}

	private string m_devicedid = string.Empty;

	private string m_email = string.Empty;

	private string m_password = string.Empty;

	private string m_uuid = string.Empty;

	public TAccountInfo deviceAccountInfo;

	public string deviceid
	{
		get
		{
			return m_devicedid;
		}
		set
		{
			m_devicedid = value;
		}
	}

	public string email
	{
		get
		{
			return m_email;
		}
		set
		{
			m_email = value;
			SaveAccountInfo();
		}
	}

	public string password
	{
		get
		{
			return m_password;
		}
		set
		{
			m_password = value;
			SaveAccountInfo();
		}
	}

	public string uuid
	{
		get
		{
			return m_uuid;
		}
		set
		{
			if (uuid != value)
			{
				DataCenter.Save().loginCode = "0";
			}
			m_uuid = value;
			SaveAccountInfo();
		}
	}

	private void Awake()
	{
		GetFromData();
	}

	public string GetUUID()
	{
		string deviceUniqueIdentifier = SystemInfo.deviceUniqueIdentifier;
		if (deviceUniqueIdentifier == string.Empty)
		{
			deviceUniqueIdentifier = SystemInfo.deviceUniqueIdentifier;
		}
		return deviceUniqueIdentifier;
	}

	public void GetFromData()
	{
		m_devicedid = GetUUID();
		if (PlayerPrefs.HasKey("AccountEmail"))
		{
			m_email = PlayerPrefs.GetString("AccountEmail");
		}
		if (PlayerPrefs.HasKey("AccountPassword"))
		{
			m_password = PlayerPrefs.GetString("AccountPassword");
		}
		if (PlayerPrefs.HasKey("AccountTId"))
		{
			m_uuid = PlayerPrefs.GetString("AccountTId");
		}
	}

	public void SaveAccountInfo()
	{
		PlayerPrefs.SetString("AccountEmail", email);
		PlayerPrefs.SetString("AccountPassword", password);
		PlayerPrefs.SetString("AccountTId", uuid);
		PlayerPrefs.Save();
	}
}

using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Xml;
using UnityEngine;

public class VersionValidationScript
{
	public class ServerInfo
	{
		public string m_strServerDomainName = string.Empty;

		public string m_strServerReserveIP = string.Empty;

		public string m_strAccountServerDomainNameAndPort = string.Empty;

		public string m_strChatServerDomainNameAndPort = string.Empty;

		public IPAddress[] m_ipAddress;

		private List<int> m_iServerPort = new List<int>();

		private int currentUsedPortIndex;

		public ServerInfo(string domainName, string ip, string accountDomainName, string chatDomainName, List<int> port)
		{
			m_strServerDomainName = domainName;
			m_strServerReserveIP = ip;
			m_iServerPort = port;
			m_strAccountServerDomainNameAndPort = accountDomainName;
			m_strChatServerDomainNameAndPort = chatDomainName;
		}

		public int GetPort()
		{
			int result = 8008;
			if (m_iServerPort.Count > 0)
			{
				if (currentUsedPortIndex >= m_iServerPort.Count)
				{
					currentUsedPortIndex = 0;
				}
				result = m_iServerPort[currentUsedPortIndex];
				currentUsedPortIndex++;
			}
			return result;
		}
	}

	public class OtherInfo
	{
		public string m_noticContent = string.Empty;
	}

	private static VersionValidationScript instance;

	public float m_fVersion = -1f;

	public List<ServerInfo> m_serverInfo = new List<ServerInfo>();

	public OtherInfo m_otherInfo;

	public static VersionValidationScript Instance()
	{
		if (instance == null)
		{
			instance = new VersionValidationScript();
		}
		return instance;
	}

	public void LoadData(string input)
	{
		string empty = string.Empty;
		try
		{
			empty = Decrypt(input);
			Debug.LogWarning("decrypt 1" + empty);
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(empty);
			XmlElement documentElement = xmlDocument.DocumentElement;
			m_fVersion = float.Parse(((XmlElement)documentElement.GetElementsByTagName("AppVersion").Item(0)).GetAttribute("Value"));
			XmlElement xml = (XmlElement)documentElement.GetElementsByTagName("Server").Item(0);
			m_serverInfo.Add(LoadServerInformation(xml));
			XmlElement xml2 = (XmlElement)documentElement.GetElementsByTagName("TestServer").Item(0);
			m_serverInfo.Add(LoadServerInformation(xml2));
			XmlElement xml3 = (XmlElement)documentElement.GetElementsByTagName("OtherInformation").Item(0);
			m_otherInfo = LoadOtherInfomations(xml3);
		}
		catch (Exception)
		{
		}
	}

	public string GetURL(ref bool bNeedUpgrade)
	{
		string result = string.Empty;
		ServerInfo serverInfo = null;
		if (m_fVersion == 1.01f)
		{
			serverInfo = m_serverInfo[0];
		}
		else if (1.01f > m_fVersion)
		{
			serverInfo = m_serverInfo[1];
		}
		else
		{
			bNeedUpgrade = true;
		}
		if (serverInfo != null)
		{
			result = "http://" + serverInfo.m_strServerDomainName + ":" + serverInfo.GetPort() + "/gameapi/ds2.do";
		}
		return result;
	}

	public string GetAccountURL()
	{
		string result = string.Empty;
		ServerInfo serverInfo = null;
		if (m_fVersion == 1.01f)
		{
			serverInfo = m_serverInfo[0];
		}
		else if (1.01f > m_fVersion)
		{
			serverInfo = m_serverInfo[1];
		}
		if (serverInfo != null)
		{
			result = "http://" + serverInfo.m_strAccountServerDomainNameAndPort + "/gameapi/ta.do";
		}
		return result;
	}

	public string GetChatURL()
	{
		string result = string.Empty;
		ServerInfo serverInfo = null;
		if (m_fVersion == 1.01f)
		{
			serverInfo = m_serverInfo[0];
		}
		else if (1.01f > m_fVersion)
		{
			serverInfo = m_serverInfo[1];
		}
		if (serverInfo != null)
		{
			result = "http://" + serverInfo.m_strChatServerDomainNameAndPort + "/gameapi/ds2.do";
		}
		return result;
	}

	private string Decrypt(string input_data)
	{
		string empty = string.Empty;
		byte[] data = Convert.FromBase64String(input_data);
		string s = "[V@F#G@UL].#980>";
		byte[] bytes = XXTEAUtils.Decrypt(data, Encoding.ASCII.GetBytes(s));
		return Encoding.UTF8.GetString(bytes);
	}

	private IPAddress[] GetHostAddress(ServerInfo si)
	{
		IPAddress[] array = new IPAddress[1];
		try
		{
			IPHostEntry hostEntry = Dns.GetHostEntry(si.m_strServerDomainName);
			array = hostEntry.AddressList;
			if (array.Length < 1)
			{
				array.SetValue(IPAddress.Parse(si.m_strServerReserveIP), 0);
			}
			return array;
		}
		catch (Exception)
		{
			array.SetValue(IPAddress.Parse(si.m_strServerReserveIP), 0);
			return array;
		}
	}

	private ServerInfo LoadServerInformation(XmlElement xml)
	{
		ServerInfo serverInfo = null;
		XmlElement xmlElement = (XmlElement)xml.GetElementsByTagName("ServerInfo").Item(0);
		string attribute = xmlElement.GetAttribute("DomainName");
		string attribute2 = xmlElement.GetAttribute("AccountDomainNameAndPort");
		string attribute3 = xmlElement.GetAttribute("ChatDomainNameAndPort");
		string attribute4 = xmlElement.GetAttribute("ReserveIP");
		List<int> list = new List<int>();
		string[] array = xmlElement.GetAttribute("Port").Split('|');
		string[] array2 = array;
		foreach (string s in array2)
		{
			int item = int.Parse(s);
			list.Add(item);
		}
		return new ServerInfo(attribute, attribute4, attribute2, attribute3, list);
	}

	private OtherInfo LoadOtherInfomations(XmlElement xml)
	{
		OtherInfo otherInfo = null;
		XmlElement xmlElement = (XmlElement)xml.GetElementsByTagName("NoticeInfo").Item(0);
		string attribute = xmlElement.GetAttribute("Content");
		otherInfo = new OtherInfo();
		otherInfo.m_noticContent = attribute;
		return otherInfo;
	}
}

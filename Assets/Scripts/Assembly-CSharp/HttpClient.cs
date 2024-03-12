using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class HttpClient
{
	private class ServerInfo
	{
		public string server;

		public string url;

		public float timeout;

		public string key;

		public ServerInfo(string server, string url, float timeout, string key)
		{
			this.server = server;
			this.url = url;
			this.timeout = timeout;
			this.key = key;
		}
	}

	private class HttpTask
	{
		private static int count;

		private int m_taskId;

		private string m_server;

		private string m_url;

		private string m_key;

		private string m_action;

		private string m_request;

		private string m_gameObjectName;

		private string m_componentName;

		private string m_functionName;

		private string m_param;

		private HttpRequestHandle.RequestType m_requestType;

		private float m_timeout;

		private byte[] m_requestData;

		private byte[] m_responseData;

		private bool m_complete;

		private WWW m_www;

		private bool keepAlive;

		private int m_result;

		private string m_response;

		public HttpTask(int taskId, string server, string url, float timeout, string key, string action, string request, string gameObjectName, string componentName, string functionName, string param, bool keepAlive = true)
		{
			m_taskId = taskId;
			m_server = server;
			m_url = url;
			m_timeout = timeout;
			m_key = key;
			m_action = action;
			m_request = request;
			m_gameObjectName = gameObjectName;
			m_componentName = componentName;
			m_functionName = functionName;
			m_param = param;
			this.keepAlive = keepAlive;
			if (timeout < 0f)
			{
				m_timeout = -1f;
			}
			else
			{
				m_timeout = Time.realtimeSinceStartup + timeout;
			}
			if (m_key != null && m_key.Length > 0)
			{
				m_requestData = XXTEAUtils.Encrypt(Encoding.UTF8.GetBytes(m_request), Encoding.UTF8.GetBytes(m_key));
			}
			else
			{
				m_requestData = Encoding.UTF8.GetBytes(m_request);
			}
			m_responseData = null;
			m_complete = false;
			m_www = null;
			m_result = -1;
			m_response = null;
		}

		public HttpTask(int taskId, string server, string url, float timeout, string key, string action, HttpRequestHandle.RequestType requestType, string request, string param, bool keepAlive = true)
		{
			m_taskId = taskId;
			m_server = server;
			m_url = url;
			m_timeout = timeout;
			m_key = key;
			m_action = action;
			m_request = request;
			m_param = param;
			m_requestType = requestType;
			this.keepAlive = keepAlive;
			if (timeout < 0f)
			{
				m_timeout = -1f;
			}
			else
			{
				m_timeout = Time.realtimeSinceStartup + timeout;
			}
			if (m_key != null && m_key.Length > 0)
			{
				m_requestData = XXTEAUtils.Encrypt(Encoding.UTF8.GetBytes(m_request), Encoding.UTF8.GetBytes(m_key));
			}
			else
			{
				m_requestData = Encoding.UTF8.GetBytes(m_request);
			}
			m_complete = false;
			m_result = -1;
			m_response = null;
			m_responseData = null;
		}

		public void Start()
		{
			try
			{
				m_www = new WWW(m_url + "?action=" + m_action, m_requestData);
			}
			catch
			{
				Complete(-1);
			}
		}

		public void Stop()
		{
			m_www = null;
		}

		public bool Run()
		{
			if (m_timeout > 0f && m_timeout < Time.realtimeSinceStartup)
			{
				Complete(-6);
			}
			if (!m_complete && m_www.isDone)
			{
				m_complete = true;
				if (m_www.error != null)
				{
					m_result = -4;
				}
				else
				{
					m_result = 0;
					m_responseData = m_www.bytes;
					if (m_key != null && m_key.Length > 0)
					{
						try
						{
							byte[] array = XXTEAUtils.Decrypt(m_responseData, Encoding.UTF8.GetBytes(m_key));
							if (array != null)
							{
								m_response = Encoding.UTF8.GetString(array);
							}
							else
							{
								m_response = string.Empty;
							}
						}
						catch (Exception)
						{
							string empty = string.Empty;
							if (m_www.bytes != null)
							{
								empty = "DataLen:" + m_www.bytes.Length;
							}
							m_response = string.Empty;
						}
					}
					else
					{
						m_response = Encoding.UTF8.GetString(m_responseData);
					}
				}
			}
			return m_complete;
		}

		public void FireCallback()
		{
			try
			{
				HttpRequestHandle.instance.OnRequest(m_taskId, m_result, m_server, m_requestType, m_action, m_response, m_param);
			}
			catch
			{
			}
		}

		private void Complete(int result)
		{
			m_result = result;
			m_complete = true;
		}
	}

	private static HttpClient m_instance;

	private Dictionary<string, ServerInfo> m_serverInfoMap;

	private int m_taskId;

	private Dictionary<int, HttpTask> m_httpTaskMap;

	private HttpClient()
	{
		m_serverInfoMap = new Dictionary<string, ServerInfo>();
		m_taskId = 1;
		m_httpTaskMap = new Dictionary<int, HttpTask>();
	}

	public static HttpClient Instance()
	{
		if (m_instance == null)
		{
			m_instance = new HttpClient();
		}
		return m_instance;
	}

	public void AddServer(string server, string url, float timeout, string key)
	{
		ServerInfo serverInfo = new ServerInfo(server, url, timeout, key);
		m_serverInfoMap[serverInfo.server] = serverInfo;
	}

	public int SendRequest(string server, string action, string data, string gameObjectName, string componentName, string functionName, string param, bool keepAlive = true)
	{
		ServerInfo serverInfo = m_serverInfoMap[server];
		int taskId = m_taskId;
		m_taskId++;
		HttpTask httpTask = new HttpTask(taskId, serverInfo.server, serverInfo.url, serverInfo.timeout, serverInfo.key, action, data, gameObjectName, componentName, functionName, param);
		httpTask.Start();
		m_httpTaskMap[taskId] = httpTask;
		return taskId;
	}

	public int SendRequest(string server, HttpRequestHandle.RequestType requestType, string action, string data, string param)
	{
		ServerInfo serverInfo = m_serverInfoMap[server];
		int taskId = m_taskId;
		m_taskId++;
		HttpTask httpTask = new HttpTask(taskId, serverInfo.server, serverInfo.url, serverInfo.timeout, serverInfo.key, action, requestType, data, param);
		httpTask.Start();
		m_httpTaskMap[taskId] = httpTask;
		return taskId;
	}

	public void HandleResponse()
	{
		if (m_httpTaskMap.Count <= 0)
		{
			return;
		}
		List<int> list = new List<int>();
		foreach (KeyValuePair<int, HttpTask> item in m_httpTaskMap)
		{
			int key = item.Key;
			HttpTask value = item.Value;
			if (value.Run())
			{
				list.Add(key);
			}
		}
		for (int i = 0; i < list.Count; i++)
		{
			int key2 = list[i];
			HttpTask httpTask = m_httpTaskMap[key2];
			m_httpTaskMap.Remove(key2);
			httpTask.Stop();
			httpTask.FireCallback();
			httpTask = null;
		}
	}

	public void CancelTask(int taskId)
	{
		if (m_httpTaskMap.ContainsKey(taskId))
		{
			HttpTask httpTask = m_httpTaskMap[taskId];
			m_httpTaskMap.Remove(taskId);
			httpTask.Stop();
			httpTask = null;
		}
	}
}

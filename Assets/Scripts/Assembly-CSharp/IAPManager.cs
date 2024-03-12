using System.Collections;
using UnityEngine;

public class IAPManager : MonoBehaviour
{
	protected enum kPingState
	{
		None = 0,
		Pinging = 1,
		Success = 2,
		Fail = 3
	}

	protected enum kPurchaseState
	{
		None = 0,
		Ping = 1,
		Purchase = 2
	}

	public delegate void OnIAPPurchaseSuccess(string sIAPKey, string sIdentifier, string sReceipt);

	public delegate void OnEvent();

	protected static IAPManager m_Instance;

	protected OnIAPPurchaseSuccess m_OnIAPPurchaseSuccess;

	protected OnEvent m_OnIAPPurchaseFailed;

	protected OnEvent m_OnIAPPurchaseCancel;

	protected OnEvent m_OnIAPPurchaseNetError;

	protected string m_sCurIAPKey = string.Empty;

	protected kPingState m_PingState;

	protected kPurchaseState m_PurchaseState;

	public static IAPManager Instance()
	{
		if (m_Instance == null)
		{
			GameObject gameObject = new GameObject("_IAPManager");
			gameObject.transform.position = Vector3.zero;
			gameObject.transform.rotation = Quaternion.identity;
			Object.DontDestroyOnLoad(gameObject);
			m_Instance = gameObject.AddComponent<IAPManager>();
		}
		return m_Instance;
	}

	private void Awake()
	{
		m_PingState = kPingState.None;
		m_PurchaseState = kPurchaseState.None;
	}

	private void Start()
	{
	}

	private void Update()
	{
		Update(Time.deltaTime);
	}

	public bool IsCanPurchase()
	{
		return m_PurchaseState == kPurchaseState.None;
	}

	public void Purchase(string sIAPKey, OnIAPPurchaseSuccess onsuccess, OnEvent onfailed, OnEvent oncancel, OnEvent onneterror)
	{
		if (m_PurchaseState == kPurchaseState.None)
		{
			m_PurchaseState = kPurchaseState.Ping;
			m_sCurIAPKey = sIAPKey;
			m_OnIAPPurchaseSuccess = onsuccess;
			m_OnIAPPurchaseFailed = onfailed;
			m_OnIAPPurchaseCancel = oncancel;
			m_OnIAPPurchaseNetError = onneterror;
			StartCoroutine(TestPingApple());
		}
	}

	protected IEnumerator TestPingApple()
	{
		m_PingState = kPingState.Pinging;
		WWW www = new WWW("http://www.apple.com/?rand=" + Random.Range(10, 99999));
		yield return www;
		if (www.error != null)
		{
			m_PingState = kPingState.Fail;
		}
		else
		{
			m_PingState = kPingState.Success;
		}
	}

	protected void Update(float deltaTime)
	{
		if (m_PurchaseState == kPurchaseState.None)
		{
			return;
		}
		if (m_PurchaseState == kPurchaseState.Ping)
		{
			if (m_PingState != kPingState.Pinging)
			{
				if (m_PingState == kPingState.Success)
				{
					m_PingState = kPingState.None;
					IAPPlugin.NowPurchaseProduct(m_sCurIAPKey, "1");
					m_PurchaseState = kPurchaseState.Purchase;
				}
				else if (m_PingState == kPingState.Fail)
				{
					m_PingState = kPingState.None;
					OnPurchaseNetError();
				}
			}
		}
		else
		{
			if (m_PurchaseState != kPurchaseState.Purchase)
			{
				return;
			}
			int purchaseStatus = IAPPlugin.GetPurchaseStatus();
			if (purchaseStatus != 0)
			{
				if (purchaseStatus == 1)
				{
					OnPurchaseSuccess(m_sCurIAPKey);
				}
				else if (purchaseStatus == -2)
				{
					OnPurchaseCancel();
				}
				else if (purchaseStatus < 0)
				{
					OnPurchaseFailed();
				}
			}
		}
	}

	public void OnPurchaseSuccess(string sIAPKey)
	{
		if (m_OnIAPPurchaseSuccess != null)
		{
			m_OnIAPPurchaseSuccess(sIAPKey, IAPPlugin.GetTransactionIdentifier(), IAPPlugin.GetTransactionReceipt());
		}
		m_PurchaseState = kPurchaseState.None;
	}

	public void OnPurchaseFailed()
	{
		if (m_OnIAPPurchaseFailed != null)
		{
			m_OnIAPPurchaseFailed();
		}
		m_PurchaseState = kPurchaseState.None;
	}

	public void OnPurchaseCancel()
	{
		if (m_OnIAPPurchaseCancel != null)
		{
			m_OnIAPPurchaseCancel();
		}
		m_PurchaseState = kPurchaseState.None;
	}

	public void OnPurchaseNetError()
	{
		if (m_OnIAPPurchaseNetError != null)
		{
			m_OnIAPPurchaseNetError();
		}
		m_PurchaseState = kPurchaseState.None;
	}
}

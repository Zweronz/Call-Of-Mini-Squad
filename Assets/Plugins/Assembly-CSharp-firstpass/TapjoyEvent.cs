using UnityEngine;

public class TapjoyEvent
{
	private string myGuid;

	private string myName;

	private string myParameter;

	private bool isContentAvailable;

	private bool isContentReady;

	private ITapjoyEvent myCallback;

	public TapjoyEvent(string eventName, ITapjoyEvent callback)
		: this(eventName, null, callback)
	{
	}

	public TapjoyEvent(string eventName, string eventParameter, ITapjoyEvent callback)
	{
		myName = eventName;
		myParameter = eventParameter;
		myCallback = callback;
		myGuid = TapjoyPlugin.CreateEvent(this, eventName, eventParameter);
		Debug.Log(string.Format("C#: Event {0} created with GUID:{1} with Param:{2}", myName, myGuid, myParameter));
	}

	public void Send()
	{
		Debug.Log(string.Format("C#: Sending event {0} ", myName));
		TapjoyPlugin.SendEvent(myGuid);
	}

	public void Show()
	{
		TapjoyPlugin.ShowEvent(myGuid);
		isContentAvailable = false;
		isContentReady = false;
	}

	public void EnableAutoPresent(bool autoPresent)
	{
		TapjoyPlugin.EnableEventAutoPresent(myGuid, autoPresent);
	}

	public void EnablePreload(bool preload)
	{
		TapjoyPlugin.EnableEventPreload(myGuid, preload);
	}

	public bool IsContentAvailable()
	{
		return isContentAvailable;
	}

	public bool IsContentReady()
	{
		return isContentReady;
	}

	public string GetName()
	{
		return myName;
	}

	public void TriggerSendEventSucceeded(bool contentIsAvailable)
	{
		isContentAvailable = contentIsAvailable;
		Debug.Log("C#: TriggerSendEventSucceeded");
		myCallback.SendEventSucceeded(this, contentIsAvailable);
	}

	public void TriggerContentIsReady(int status)
	{
		Debug.Log("C#: TriggerContentIsReady with status" + status);
		isContentReady = true;
		myCallback.ContentIsReady(this, status);
	}

	public void TriggerSendEventFailed(string errorMsg)
	{
		Debug.Log("C#: TriggerSendEventFailed");
		myCallback.SendEventFailed(this, errorMsg);
	}

	public void TriggerContentDidAppear()
	{
		Debug.Log("C#: TriggerContentDidAppear");
		myCallback.ContentDidAppear(this);
	}

	public void TriggerContentDidDisappear()
	{
		Debug.Log("C#: TriggerContentDidDisappear");
		myCallback.ContentDidDisappear(this);
	}

	public void TriggerDidRequestAction(int type, string identifier, int quantity)
	{
		Debug.Log("C#: TriggerDidRequestAction");
		TapjoyEventRequest tapjoyEventRequest = new TapjoyEventRequest(myGuid, type, identifier, quantity);
		myCallback.DidRequestAction(this, tapjoyEventRequest);
	}
}

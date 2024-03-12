public class TapjoyEventRequest
{
	public const int TYPE_IN_APP_PURCHASE = 1;

	public const int TYPE_VIRTUAL_GOOD = 2;

	public const int TYPE_CURRENCY = 3;

	public const int TYPE_NAVIGATION = 4;

	private string eventGuid;

	public int type;

	public string identifier;

	public int quantity;

	public TapjoyEventRequest(string eventGuid, int type, string identifier, int quantity)
	{
		this.eventGuid = eventGuid;
		this.type = type;
		this.identifier = identifier;
		this.quantity = quantity;
	}

	public void EventRequestCompleted()
	{
		TapjoyPlugin.EventRequestCompleted(eventGuid);
	}

	public void EventRequestCancelled()
	{
		TapjoyPlugin.EventRequestCancelled(eventGuid);
	}
}

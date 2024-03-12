using UnityEngine;

public class UtilUIDragRoation : MonoBehaviour
{
	public GameObject eventReceiver;

	public string callWhenFinished = string.Empty;

	private Vector2 lastDragDelta = Vector2.zero;

	public void OnDrag(Vector2 delta)
	{
		lastDragDelta = delta;
	}

	public void OnDragEnd()
	{
		if (eventReceiver != null && !string.IsNullOrEmpty(callWhenFinished))
		{
			eventReceiver.SendMessage(callWhenFinished, lastDragDelta, SendMessageOptions.DontRequireReceiver);
		}
	}
}

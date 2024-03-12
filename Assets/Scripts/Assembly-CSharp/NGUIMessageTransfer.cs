using UnityEngine;

public class NGUIMessageTransfer : MonoBehaviour
{
	public GameObject target;

	public bool bHover;

	public bool bPress;

	private void OnHover(bool isOver)
	{
		if (bHover)
		{
			target.SendMessage("OnHover", isOver, SendMessageOptions.DontRequireReceiver);
		}
	}

	private void OnPress(bool pressed)
	{
		if (bPress)
		{
			target.SendMessage("bPress", pressed, SendMessageOptions.DontRequireReceiver);
		}
	}
}

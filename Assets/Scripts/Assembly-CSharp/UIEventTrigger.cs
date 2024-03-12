using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Event Trigger")]
public class UIEventTrigger : MonoBehaviour
{
	public static UIEventTrigger current;

	public List<EventDelegate> onHoverOver = new List<EventDelegate>();

	public List<EventDelegate> onHoverOut = new List<EventDelegate>();

	public List<EventDelegate> onPress = new List<EventDelegate>();

	public List<EventDelegate> onRelease = new List<EventDelegate>();

	public List<EventDelegate> onSelect = new List<EventDelegate>();

	public List<EventDelegate> onDeselect = new List<EventDelegate>();

	public List<EventDelegate> onClick = new List<EventDelegate>();

	public List<EventDelegate> onDoubleClick = new List<EventDelegate>();

	private void OnHover(bool isOver)
	{
		current = this;
		if (isOver)
		{
			EventDelegate.Execute(onHoverOver);
		}
		else
		{
			EventDelegate.Execute(onHoverOut);
		}
		current = null;
	}

	private void OnPress(bool pressed)
	{
		current = this;
		if (pressed)
		{
			EventDelegate.Execute(onPress);
		}
		else
		{
			EventDelegate.Execute(onRelease);
		}
		current = null;
	}

	private void OnSelect(bool selected)
	{
		current = this;
		if (selected)
		{
			EventDelegate.Execute(onSelect);
		}
		else
		{
			EventDelegate.Execute(onDeselect);
		}
		current = null;
	}

	private void OnClick()
	{
		current = this;
		EventDelegate.Execute(onClick);
		current = null;
	}

	private void OnDoubleClick()
	{
		current = this;
		EventDelegate.Execute(onDoubleClick);
		current = null;
	}
}

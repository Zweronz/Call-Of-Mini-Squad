using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Button")]
public class UIButton : UIButtonColor
{
	public static UIButton current;

	public Color disabledColor = Color.grey;

	public bool dragHighlight;

	public List<EventDelegate> onClick = new List<EventDelegate>();

	public virtual bool isEnabled
	{
		get
		{
			if (!base.enabled)
			{
				return false;
			}
			Collider collider = base.GetComponent<Collider>();
			return (bool)collider && collider.enabled;
		}
		set
		{
			Collider collider = base.GetComponent<Collider>();
			if (collider != null)
			{
				collider.enabled = value;
			}
			else
			{
				base.enabled = value;
			}
			UpdateColor(value, false);
		}
	}

	protected override void OnEnable()
	{
		if (isEnabled)
		{
			if (mStarted)
			{
				if (UICamera.currentScheme == UICamera.ControlScheme.Controller)
				{
					OnHover(UICamera.selectedObject == base.gameObject);
				}
				else if (UICamera.currentScheme == UICamera.ControlScheme.Mouse)
				{
					OnHover(UICamera.hoveredObject == base.gameObject);
				}
				else
				{
					UpdateColor(true, false);
				}
			}
		}
		else
		{
			UpdateColor(false, true);
		}
	}

	protected override void OnHover(bool isOver)
	{
		if (isEnabled)
		{
			base.OnHover(isOver);
		}
	}

	protected override void OnPress(bool isPressed)
	{
		if (isEnabled)
		{
			base.OnPress(isPressed);
		}
	}

	protected override void OnDragOver()
	{
		if (isEnabled && (dragHighlight || UICamera.currentTouch.pressed == base.gameObject))
		{
			base.OnDragOver();
		}
	}

	protected override void OnDragOut()
	{
		if (isEnabled && (dragHighlight || UICamera.currentTouch.pressed == base.gameObject))
		{
			base.OnDragOut();
		}
	}

	protected override void OnSelect(bool isSelected)
	{
		if (isEnabled)
		{
			base.OnSelect(isSelected);
		}
	}

	protected virtual void OnClick()
	{
		if (isEnabled)
		{
			current = this;
			EventDelegate.Execute(onClick);
			current = null;
		}
	}

	public void UpdateColor(bool shouldBeEnabled, bool immediate)
	{
		if ((bool)tweenTarget)
		{
			if (!mStarted)
			{
				mStarted = true;
				Init();
			}
			Color color = ((!shouldBeEnabled) ? disabledColor : base.defaultColor);
			TweenColor tweenColor = TweenColor.Begin(tweenTarget, 0.15f, color);
			if (tweenColor != null && immediate)
			{
				tweenColor.value = color;
				tweenColor.enabled = false;
			}
		}
	}
}

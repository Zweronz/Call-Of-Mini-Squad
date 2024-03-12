using System;
using System.Collections.Generic;
using AnimationOrTween;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Toggle")]
[ExecuteInEditMode]
public class UIToggle : UIWidgetContainer
{
	public static BetterList<UIToggle> list = new BetterList<UIToggle>();

	public static UIToggle current;

	public int group;

	public UIWidget activeSprite;

	public Animation activeAnimation;

	public bool startsActive;

	public bool instantTween;

	public bool optionCanBeNone;

	public List<EventDelegate> onChange = new List<EventDelegate>();

	[HideInInspector]
	[SerializeField]
	private Transform radioButtonRoot;

	[HideInInspector]
	[SerializeField]
	private UISprite checkSprite;

	[HideInInspector]
	[SerializeField]
	private Animation checkAnimation;

	[HideInInspector]
	[SerializeField]
	private GameObject eventReceiver;

	[HideInInspector]
	[SerializeField]
	private string functionName = "OnActivate";

	[SerializeField]
	[HideInInspector]
	private bool startsChecked;

	private bool mIsActive = true;

	private bool mStarted;

	public bool value
	{
		get
		{
			return mIsActive;
		}
		set
		{
			if (group == 0 || value || optionCanBeNone || !mStarted)
			{
				Set(value);
			}
		}
	}

	[Obsolete("Use 'value' instead")]
	public bool isChecked
	{
		get
		{
			return value;
		}
		set
		{
			this.value = value;
		}
	}

	public static UIToggle GetActiveToggle(int group)
	{
		for (int i = 0; i < list.size; i++)
		{
			UIToggle uIToggle = list[i];
			if (uIToggle != null && uIToggle.group == group && uIToggle.mIsActive)
			{
				return uIToggle;
			}
		}
		return null;
	}

	private void OnEnable()
	{
		list.Add(this);
	}

	private void OnDisable()
	{
		list.Remove(this);
	}

	private void Start()
	{
		if (startsChecked)
		{
			startsChecked = false;
			startsActive = true;
		}
		mIsActive = !startsActive;
		mStarted = true;
		Set(startsActive);
	}

	private void OnClick()
	{
		if (base.enabled)
		{
			value = !value;
		}
	}

	private void Set(bool state)
	{
		if (!mStarted)
		{
			mIsActive = state;
			startsActive = state;
			if (activeSprite != null)
			{
				activeSprite.alpha = ((!state) ? 0f : 1f);
			}
		}
		else
		{
			if (mIsActive == state)
			{
				return;
			}
			if (group != 0 && state)
			{
				int num = 0;
				int size = list.size;
				while (num < size)
				{
					UIToggle uIToggle = list[num];
					if (uIToggle != this && uIToggle.group == group)
					{
						uIToggle.Set(false);
					}
					if (list.size != size)
					{
						size = list.size;
						num = 0;
					}
					else
					{
						num++;
					}
				}
			}
			mIsActive = state;
			if (activeSprite != null)
			{
				if (instantTween)
				{
					activeSprite.alpha = ((!mIsActive) ? 0f : 1f);
				}
				else
				{
					TweenAlpha.Begin(activeSprite.gameObject, 0.15f, (!mIsActive) ? 0f : 1f);
				}
			}
			current = this;
			if (EventDelegate.IsValid(onChange))
			{
				EventDelegate.Execute(onChange);
			}
			else if (eventReceiver != null && !string.IsNullOrEmpty(functionName))
			{
				eventReceiver.SendMessage(functionName, mIsActive, SendMessageOptions.DontRequireReceiver);
			}
			current = null;
			if (activeAnimation != null)
			{
				ActiveAnimation.Play(activeAnimation, state ? Direction.Forward : Direction.Reverse);
			}
		}
	}
}

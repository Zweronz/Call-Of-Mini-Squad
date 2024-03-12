using UnityEngine;

[AddComponentMenu("Game/UI/Key Binding")]
public class UIKeyBinding : MonoBehaviour
{
	public enum Action
	{
		PressAndClick = 0,
		Select = 1
	}

	public enum Modifier
	{
		None = 0,
		Shift = 1,
		Control = 2,
		Alt = 3
	}

	public KeyCode keyCode;

	public Modifier modifier;

	public Action action;

	private bool mIgnoreUp;

	private bool mIsInput;

	private void Start()
	{
		UIInput component = GetComponent<UIInput>();
		mIsInput = component != null;
		if (component != null)
		{
			EventDelegate.Add(component.onSubmit, OnSubmit);
		}
	}

	private void OnSubmit()
	{
		if (UICamera.currentKey == keyCode && IsModifierActive())
		{
			mIgnoreUp = true;
		}
	}

	private bool IsModifierActive()
	{
		if (modifier == Modifier.None)
		{
			return true;
		}
		if (modifier == Modifier.Alt)
		{
			if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
			{
				return true;
			}
		}
		else if (modifier == Modifier.Control)
		{
			if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
			{
				return true;
			}
		}
		else if (modifier == Modifier.Shift && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
		{
			return true;
		}
		return false;
	}

	private void Update()
	{
		if (keyCode == KeyCode.None || !IsModifierActive())
		{
			return;
		}
		if (action == Action.PressAndClick)
		{
			if (!UICamera.inputHasFocus)
			{
				UICamera.currentTouch = UICamera.controller;
				UICamera.currentScheme = UICamera.ControlScheme.Controller;
				UICamera.currentTouch.current = base.gameObject;
				if (Input.GetKeyDown(keyCode))
				{
					UICamera.Notify(base.gameObject, "OnPress", true);
				}
				if (Input.GetKeyUp(keyCode))
				{
					UICamera.Notify(base.gameObject, "OnPress", false);
					UICamera.Notify(base.gameObject, "OnClick", null);
				}
				UICamera.currentTouch.current = null;
			}
		}
		else
		{
			if (action != Action.Select || !Input.GetKeyUp(keyCode))
			{
				return;
			}
			if (mIsInput)
			{
				if (!mIgnoreUp && !UICamera.inputHasFocus)
				{
					UICamera.selectedObject = base.gameObject;
				}
				mIgnoreUp = false;
			}
			else
			{
				UICamera.selectedObject = base.gameObject;
			}
		}
	}
}

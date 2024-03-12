using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Play Sound")]
public class UIPlaySound : MonoBehaviour
{
	public enum Trigger
	{
		OnClick = 0,
		OnMouseOver = 1,
		OnMouseOut = 2,
		OnPress = 3,
		OnRelease = 4,
		Custom = 5
	}

	public AudioClip audioClip;

	public Trigger trigger;

	private bool mIsOver;

	[Range(0f, 1f)]
	public float volume = 1f;

	[Range(0f, 2f)]
	public float pitch = 1f;

	private void OnHover(bool isOver)
	{
		if (!DataCenter.Save().PlaySound)
		{
			return;
		}
		if (trigger == Trigger.OnMouseOver)
		{
			if (mIsOver == isOver)
			{
				return;
			}
			mIsOver = isOver;
		}
		if (base.enabled && ((isOver && trigger == Trigger.OnMouseOver) || (!isOver && trigger == Trigger.OnMouseOut)))
		{
			NGUITools.PlaySound(audioClip, volume, pitch);
		}
	}

	private void OnPress(bool isPressed)
	{
		if (!DataCenter.Save().PlaySound)
		{
			return;
		}
		if (trigger == Trigger.OnPress)
		{
			if (mIsOver == isPressed)
			{
				return;
			}
			mIsOver = isPressed;
		}
		if (base.enabled && ((isPressed && trigger == Trigger.OnPress) || (!isPressed && trigger == Trigger.OnRelease)))
		{
			NGUITools.PlaySound(audioClip, volume, pitch);
		}
	}

	private void OnClick()
	{
		if (DataCenter.Save().PlaySound && base.enabled && trigger == Trigger.OnClick)
		{
			NGUITools.PlaySound(audioClip, volume, pitch);
		}
	}

	private void OnSelect(bool isSelected)
	{
		if (DataCenter.Save().PlaySound && base.enabled && (!isSelected || UICamera.currentScheme == UICamera.ControlScheme.Controller))
		{
			OnHover(isSelected);
		}
	}

	public void Play()
	{
		if (DataCenter.Save().PlaySound)
		{
			NGUITools.PlaySound(audioClip, volume, pitch);
		}
	}
}

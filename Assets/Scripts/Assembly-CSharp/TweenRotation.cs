using System;
using UnityEngine;

[AddComponentMenu("NGUI/Tween/Tween Rotation")]
public class TweenRotation : UITweener
{
	public Vector3 from;

	public Vector3 to;

	private Transform mTrans;

	public Transform cachedTransform
	{
		get
		{
			if (mTrans == null)
			{
				mTrans = base.transform;
			}
			return mTrans;
		}
	}

	[Obsolete("Use 'value' instead")]
	public Quaternion rotation
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

	public Quaternion value
	{
		get
		{
			return cachedTransform.localRotation;
		}
		set
		{
			cachedTransform.localRotation = value;
		}
	}

	protected override void OnUpdate(float factor, bool isFinished)
	{
		value = Quaternion.Slerp(Quaternion.Euler(from), Quaternion.Euler(to), factor);
	}

	public static TweenRotation Begin(GameObject go, float duration, Quaternion rot)
	{
		TweenRotation tweenRotation = UITweener.Begin<TweenRotation>(go, duration);
		tweenRotation.from = tweenRotation.value.eulerAngles;
		tweenRotation.to = rot.eulerAngles;
		if (duration <= 0f)
		{
			tweenRotation.Sample(1f, true);
			tweenRotation.enabled = false;
		}
		return tweenRotation;
	}

	[ContextMenu("Set 'From' to current value")]
	public override void SetStartToCurrentValue()
	{
		from = value.eulerAngles;
	}

	[ContextMenu("Set 'To' to current value")]
	public override void SetEndToCurrentValue()
	{
		to = value.eulerAngles;
	}

	[ContextMenu("Assume value of 'From'")]
	private void SetCurrentValueToStart()
	{
		value = Quaternion.Euler(from);
	}

	[ContextMenu("Assume value of 'To'")]
	private void SetCurrentValueToEnd()
	{
		value = Quaternion.Euler(to);
	}
}

using System;
using UnityEngine;

[AddComponentMenu("NGUI/Tween/Tween Position")]
public class TweenPosition : UITweener
{
	public Vector3 from;

	public Vector3 to;

	[HideInInspector]
	public bool worldSpace;

	private Transform mTrans;

	private UIRect mRect;

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
	public Vector3 position
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

	public Vector3 value
	{
		get
		{
			return (!worldSpace) ? cachedTransform.localPosition : cachedTransform.position;
		}
		set
		{
			if (mRect == null || !mRect.isAnchored || worldSpace)
			{
				if (worldSpace)
				{
					cachedTransform.position = value;
				}
				else
				{
					cachedTransform.localPosition = value;
				}
			}
			else
			{
				value -= cachedTransform.localPosition;
				NGUIMath.MoveRect(mRect, value.x, value.y);
			}
		}
	}

	private void Awake()
	{
		mRect = GetComponent<UIRect>();
		float x = ((from.x == -9999f) ? ((float)(-Screen.width / 2)) : ((from.x != 9999f) ? from.x : ((float)(Screen.width / 2))));
		float y = ((from.y == -9999f) ? ((float)(-Screen.height / 2)) : ((from.y != 9999f) ? from.y : ((float)(Screen.height / 2))));
		float x2 = ((to.x == -9999f) ? ((float)(-Screen.width / 2)) : ((to.x != 9999f) ? to.x : ((float)(Screen.width / 2))));
		float y2 = ((to.y == -9999f) ? ((float)(-Screen.height / 2)) : ((to.y != 9999f) ? to.y : ((float)(Screen.height / 2))));
		from = new Vector3(x, y, from.z);
		to = new Vector3(x2, y2, to.z);
	}

	protected override void OnUpdate(float factor, bool isFinished)
	{
		value = from * (1f - factor) + to * factor;
	}

	public static TweenPosition Begin(GameObject go, float duration, Vector3 pos)
	{
		TweenPosition tweenPosition = UITweener.Begin<TweenPosition>(go, duration);
		tweenPosition.from = tweenPosition.value;
		tweenPosition.to = pos;
		if (duration <= 0f)
		{
			tweenPosition.Sample(1f, true);
			tweenPosition.enabled = false;
		}
		return tweenPosition;
	}

	[ContextMenu("Set 'From' to current value")]
	public override void SetStartToCurrentValue()
	{
		from = value;
	}

	[ContextMenu("Set 'To' to current value")]
	public override void SetEndToCurrentValue()
	{
		to = value;
	}

	[ContextMenu("Assume value of 'From'")]
	private void SetCurrentValueToStart()
	{
		value = from;
	}

	[ContextMenu("Assume value of 'To'")]
	private void SetCurrentValueToEnd()
	{
		value = to;
	}
}

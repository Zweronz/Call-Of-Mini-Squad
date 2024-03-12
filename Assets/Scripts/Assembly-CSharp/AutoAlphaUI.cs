using UnityEngine;

public class AutoAlphaUI : MonoBehaviour
{
	public TweenAlpha tweenAlpha;

	public void Start()
	{
		tweenAlpha = base.gameObject.GetComponent<TweenAlpha>();
	}

	public void Show(float durationTime = 2f, float fromA = 0f, float toA = 1f)
	{
		Do(durationTime, fromA, toA);
	}

	public void Hide(float durationTime = 2f, float fromA = 1f, float toA = 0f)
	{
		Do(durationTime, fromA, toA);
	}

	protected void Do(float durationTime, float fromA, float toA)
	{
		tweenAlpha = base.gameObject.GetComponent<TweenAlpha>();
		if (tweenAlpha != null)
		{
			Object.Destroy(tweenAlpha);
			tweenAlpha = null;
		}
		tweenAlpha = base.gameObject.AddComponent<TweenAlpha>();
		tweenAlpha.to = toA;
		tweenAlpha.from = fromA;
		tweenAlpha.duration = durationTime;
	}
}

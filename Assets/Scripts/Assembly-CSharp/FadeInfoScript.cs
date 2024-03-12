using UnityEngine;

public class FadeInfoScript : MonoBehaviour
{
	private static FadeInfoScript mInstance;

	private TweenAlpha tweenAlpha;

	public Transform targetCompent;

	public bool bFadeIn;

	public bool bFadeOut;

	public static FadeInfoScript Instance
	{
		get
		{
			if (mInstance == null)
			{
				mInstance = GameObject.Find("Fade Anchor").GetComponent<FadeInfoScript>();
			}
			return mInstance;
		}
	}

	private void Awake()
	{
		if (targetCompent != null)
		{
			tweenAlpha = targetCompent.GetComponent<TweenAlpha>();
		}
		else
		{
			tweenAlpha = base.transform.Find("Panel").Find("Sprite").GetComponent<TweenAlpha>();
		}
		tweenAlpha.style = UITweener.Style.Once;
		tweenAlpha.delay = 0f;
		tweenAlpha.enabled = false;
		Object.DontDestroyOnLoad(base.gameObject);
	}

	public void SubFinishEvent(GameObject eventReceiver, EventDelegate eventDelgate)
	{
		if ((bool)tweenAlpha)
		{
			tweenAlpha.eventReceiver = eventReceiver;
			EventDelegate.Add(tweenAlpha.onFinished, eventDelgate);
		}
	}

	public void SubFinishEvent(GameObject eventReceiver, string functionName)
	{
		if ((bool)tweenAlpha)
		{
			tweenAlpha.eventReceiver = eventReceiver;
			tweenAlpha.callWhenFinished = functionName;
		}
	}

	public void FadeIn(float durationTime = 1f)
	{
		if ((bool)tweenAlpha)
		{
			tweenAlpha.ResetToBeginning();
			tweenAlpha.duration = durationTime;
			tweenAlpha.value = 1f;
			tweenAlpha.from = 1f;
			tweenAlpha.to = 0f;
			tweenAlpha.enabled = true;
		}
	}

	public void FadeOut(float durationTime = 1f)
	{
		if ((bool)tweenAlpha)
		{
			tweenAlpha.ResetToBeginning();
			tweenAlpha.duration = durationTime;
			tweenAlpha.value = 0f;
			tweenAlpha.from = 0f;
			tweenAlpha.to = 1f;
			tweenAlpha.enabled = true;
		}
	}

	public void Update()
	{
		if (bFadeIn)
		{
			bFadeIn = false;
			FadeIn();
		}
		if (bFadeOut)
		{
			bFadeOut = false;
			FadeOut();
		}
	}
}

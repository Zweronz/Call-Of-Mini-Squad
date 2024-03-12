using UnityEngine;

public class ZoomInfoScript : MonoBehaviour
{
	private TweenScale tweenScale;

	public Transform targetCompent;

	public bool bZoomIn;

	public bool bZoomOut;

	private void Awake()
	{
		if (targetCompent == null)
		{
			targetCompent = base.transform;
		}
		tweenScale = base.transform.GetComponent<TweenScale>();
		tweenScale.style = UITweener.Style.Once;
		tweenScale.delay = 0f;
		tweenScale.enabled = false;
	}

	public void ZoomIn(float durationTime = 1f)
	{
		if (!tweenScale)
		{
			Object.DestroyImmediate(tweenScale);
			tweenScale = targetCompent.gameObject.AddComponent<TweenScale>();
			return;
		}
		tweenScale.ResetToBeginning();
		tweenScale.duration = durationTime;
		tweenScale.from = new Vector3(1f, 1f, 1f);
		tweenScale.to = new Vector3(2f, 2f, 1f);
		tweenScale.PlayForward();
	}

	public void ZoomOut(float durationTime = 1f)
	{
		if (!tweenScale)
		{
			Object.DestroyImmediate(tweenScale);
			tweenScale = targetCompent.gameObject.AddComponent<TweenScale>();
			return;
		}
		tweenScale.ResetToBeginning();
		tweenScale.duration = durationTime;
		tweenScale.from = new Vector3(2f, 2f, 1f);
		tweenScale.to = new Vector3(1f, 1f, 1f);
		tweenScale.PlayForward();
	}

	public void Update()
	{
		if (bZoomIn)
		{
			bZoomIn = false;
			ZoomIn();
		}
		if (bZoomOut)
		{
			bZoomOut = false;
			ZoomOut();
		}
	}
}

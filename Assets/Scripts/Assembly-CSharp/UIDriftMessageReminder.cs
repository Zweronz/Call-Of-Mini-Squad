using UnityEngine;

public class UIDriftMessageReminder : MonoBehaviour
{
	public TweenAlpha tweenAlpha;

	public TweenPosition tweenPos;

	public UILabel label;

	private float startPosYOfScreenHeightPercent = 0.3f;

	private float endPosYOfScreenHeightPercent = 0.5f;

	private float driftPosAndFadeInDurationSec = 0.5f;

	private float continueSec = 1.5f;

	private float fadeOutSec = 0.5f;

	private float startPosY;

	private float endPosY;

	private float fStartTime = -1f;

	private void Awake()
	{
		startPosY = 0f - (float)Screen.height / 2f + startPosYOfScreenHeightPercent * (float)Screen.height;
		endPosY = 0f - (float)Screen.height / 2f + endPosYOfScreenHeightPercent * (float)Screen.height;
	}

	private void Update()
	{
		if (fStartTime >= 0f && !tweenPos.enabled && Time.time - fStartTime >= continueSec)
		{
			FadeOut();
			fStartTime = -1f;
		}
	}

	public void UpdateLabel(string str)
	{
		label.text = str;
	}

	public void SetLabelSize(int size)
	{
		label.fontSize = size;
	}

	public void Do()
	{
		fStartTime = Time.time;
		if (!base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(true);
		}
		DriftPosAndFadeIn();
	}

	protected void DriftPosAndFadeIn()
	{
		tweenAlpha.enabled = true;
		tweenAlpha.ResetToBeginning();
		tweenPos.enabled = true;
		tweenPos.ResetToBeginning();
		tweenPos.from = new Vector3(0f, startPosY, 0f);
		tweenPos.to = new Vector3(0f, endPosY, 0f);
		tweenPos.duration = driftPosAndFadeInDurationSec;
		tweenAlpha.value = 0f;
		tweenAlpha.from = 0f;
		tweenAlpha.to = 1f;
		tweenAlpha.duration = driftPosAndFadeInDurationSec;
	}

	protected void FadeOut()
	{
		tweenAlpha.enabled = true;
		tweenAlpha.ResetToBeginning();
		tweenAlpha.value = 1f;
		tweenAlpha.from = 1f;
		tweenAlpha.to = 0f;
		tweenAlpha.duration = fadeOutSec;
	}
}

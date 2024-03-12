using UnityEngine;

public class UtilUICaricature : MonoBehaviour
{
	public UITexture[] textures;

	private void Start()
	{
		for (int i = 0; i < textures.Length; i++)
		{
			if (i == 0)
			{
				textures[i].alpha = 1f;
			}
			else
			{
				textures[i].alpha = 0f;
			}
		}
		TargetFadeOut(textures[0].gameObject, "Texture1FadeOutFinished");
	}

	private void TargetFadeIn(GameObject go, string onFinishEvent)
	{
		TweenAlpha tweenAlpha = go.GetComponent<TweenAlpha>();
		if (tweenAlpha == null)
		{
			tweenAlpha = go.AddComponent<TweenAlpha>();
		}
		tweenAlpha.ResetToBeginning();
		tweenAlpha.duration = 2f;
		tweenAlpha.value = 1f;
		tweenAlpha.from = 1f;
		tweenAlpha.to = 0f;
		tweenAlpha.enabled = true;
		tweenAlpha.eventReceiver = base.gameObject;
		tweenAlpha.callWhenFinished = onFinishEvent;
	}

	private void TargetFadeOut(GameObject go, string onFinishEvent)
	{
		TweenAlpha tweenAlpha = go.GetComponent<TweenAlpha>();
		if (tweenAlpha == null)
		{
			tweenAlpha = go.AddComponent<TweenAlpha>();
		}
		tweenAlpha.ResetToBeginning();
		tweenAlpha.duration = 3f;
		tweenAlpha.value = 0f;
		tweenAlpha.from = 0f;
		tweenAlpha.to = 1f;
		tweenAlpha.enabled = true;
		tweenAlpha.eventReceiver = base.gameObject;
		tweenAlpha.callWhenFinished = onFinishEvent;
	}

	private void Texture1FadeOutFinished()
	{
		TargetFadeIn(textures[0].gameObject, "Texture1FadeInFinished");
	}

	private void Texture1FadeInFinished()
	{
		TargetFadeOut(textures[1].gameObject, "Texture2FadeOutFinished");
	}

	private void Texture2FadeOutFinished()
	{
		TargetFadeIn(textures[1].gameObject, "Texture3FadeInFinished");
	}

	private void Texture3FadeInFinished()
	{
		TargetFadeOut(textures[2].gameObject, "Texture3FadeOutFinished");
	}

	private void Texture3FadeOutFinished()
	{
		SceneManager.Instance.SwitchScene("UIBase");
	}
}

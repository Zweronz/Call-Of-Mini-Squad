using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[AddComponentMenu("AudioEffect/AudioEffectEx AuxFade")]
public class TAudioAuxFade : ITAudioLimit
{
	public delegate void OnFadeOutDegelate();

	public int fadeInTime;

	public int fadeOutTime;

	public bool autoFadeout;

	private float m_time_fadeout;

	public override bool isCanPlay
	{
		get
		{
			return true;
		}
	}

	private IEnumerator AutoFadeOut(AudioClip clip)
	{
		float len = clip.length;
		float time = (float)fadeOutTime * 0.001f;
		if (len > time)
		{
			float time_fadeout = Time.realtimeSinceStartup;
			float time_dis = len - time;
			while (Time.realtimeSinceStartup - time_fadeout <= time_dis)
			{
				yield return 0;
			}
		}
		StartCoroutine(FadeOut(null));
	}

	public IEnumerator FadeIn()
	{
		if (fadeInTime <= 0)
		{
			yield break;
		}
		float volumOri = base.GetComponent<AudioSource>().volume;
		float volumSpd = volumOri / ((float)fadeInTime * 0.001f);
		base.GetComponent<AudioSource>().volume = 0f;
		float volumTime = Time.realtimeSinceStartup;
		while (true)
		{
			float volum2 = base.GetComponent<AudioSource>().volume;
			volum2 += volumSpd * (Time.realtimeSinceStartup - volumTime);
			volumTime = Time.realtimeSinceStartup;
			if (volum2 > volumOri)
			{
				break;
			}
			base.GetComponent<AudioSource>().volume = volum2;
			yield return 0;
		}
		base.GetComponent<AudioSource>().volume = volumOri;
	}

	public IEnumerator FadeOut(OnFadeOutDegelate onFadeOutDegelate)
	{
		if (fadeOutTime <= 0)
		{
			yield break;
		}
		float volumOri = base.GetComponent<AudioSource>().volume;
		float volumSpd = volumOri / ((float)fadeOutTime * 0.001f);
		float volumTime = Time.realtimeSinceStartup;
		while (true)
		{
			float volum2 = base.GetComponent<AudioSource>().volume;
			volum2 -= volumSpd * (Time.realtimeSinceStartup - volumTime);
			volumTime = Time.realtimeSinceStartup;
			if (volum2 < 0f)
			{
				break;
			}
			base.GetComponent<AudioSource>().volume = volum2;
			yield return 0;
		}
		base.GetComponent<AudioSource>().volume = 0f;
		TAudioManager.instance.StopSound(base.GetComponent<AudioSource>());
		if (onFadeOutDegelate != null)
		{
			onFadeOutDegelate();
		}
	}

	public override void OnAudioTrigger(AudioClip clip)
	{
		StopAllCoroutines();
		StartCoroutine(FadeIn());
		if (autoFadeout && fadeOutTime > 0)
		{
			StartCoroutine(AutoFadeOut(clip));
		}
	}
}

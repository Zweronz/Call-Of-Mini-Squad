using System;
using UnityEngine;

[AddComponentMenu("AudioEffect/AudioEffect Together")]
public class TAudioEffectTogether : ITAudioEvent
{
	private struct DelayAndIndex
	{
		public int index;

		public float deltaTime;

		public DelayAndIndex(int index, float deltaTime)
		{
			this.index = index;
			this.deltaTime = deltaTime;
		}
	}

	public GameObject[] audioEffectRefs;

	public float maxDelayTime;

	public bool useDelayPlay;

	[HideInInspector]
	public float[] delayTimeArray;

	private ITAudioEvent[] audioEvts = new ITAudioEvent[0];

	private ITAudioLimit[] audioLimits = new ITAudioLimit[0];

	private float playTime;

	private bool isPlay;

	private int lastPlayIndex = -1;

	private bool m_awake;

	private DelayAndIndex[] delayAndIndex;

	public override bool isPlaying
	{
		get
		{
			ITAudioEvent[] array = audioEvts;
			foreach (ITAudioEvent iTAudioEvent in array)
			{
				if (iTAudioEvent.isPlaying)
				{
					return true;
				}
			}
			return false;
		}
	}

	public override bool isLoop
	{
		get
		{
			ITAudioEvent[] array = audioEvts;
			foreach (ITAudioEvent iTAudioEvent in array)
			{
				if (iTAudioEvent.isLoop)
				{
					return true;
				}
			}
			return false;
		}
	}

	private void Awake()
	{
		audioLimits = GetComponents<ITAudioLimit>();
		audioEvts = new ITAudioEvent[audioEffectRefs.Length];
		for (int i = 0; i < audioEffectRefs.Length; i++)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(audioEffectRefs[i]) as GameObject;
			gameObject.transform.parent = base.transform;
			gameObject.transform.localPosition = Vector3.zero;
			audioEvts[i] = gameObject.GetComponent<ITAudioEvent>();
		}
		delayAndIndex = new DelayAndIndex[delayTimeArray.Length];
		for (int j = 0; j < delayAndIndex.Length; j++)
		{
			delayAndIndex[j] = new DelayAndIndex(j, delayTimeArray[j]);
		}
		Array.Sort(delayAndIndex, Compare);
		m_awake = true;
	}

	private void Update()
	{
		if (!isPlay)
		{
			return;
		}
		float num = Time.realtimeSinceStartup - playTime;
		for (int i = lastPlayIndex + 1; i < delayAndIndex.Length; i++)
		{
			float num2 = delayAndIndex[i].deltaTime * maxDelayTime * 0.001f;
			if (num > num2)
			{
				audioEvts[delayAndIndex[i].index].Trigger();
				lastPlayIndex = i;
			}
		}
		if (num > maxDelayTime * 0.001f)
		{
			isPlay = false;
		}
	}

	private void SendTriggerEvent(AudioClip clip)
	{
		ITAudioLimit[] array = audioLimits;
		foreach (ITAudioLimit iTAudioLimit in array)
		{
			iTAudioLimit.OnAudioTrigger(clip);
		}
	}

	public override void Trigger()
	{
		if (!m_awake)
		{
		}
		if (audioEvts.Length == 0)
		{
			return;
		}
		ITAudioLimit[] array = audioLimits;
		foreach (ITAudioLimit iTAudioLimit in array)
		{
			if (!iTAudioLimit.isCanPlay)
			{
				return;
			}
		}
		if (useDelayPlay)
		{
			playTime = Time.realtimeSinceStartup;
			isPlay = true;
			lastPlayIndex = -1;
		}
		else
		{
			ITAudioEvent[] array2 = audioEvts;
			foreach (ITAudioEvent iTAudioEvent in array2)
			{
				iTAudioEvent.Trigger();
			}
		}
		SendTriggerEvent(null);
	}

	public override void Stop()
	{
		isPlay = false;
		ITAudioEvent[] array = audioEvts;
		foreach (ITAudioEvent iTAudioEvent in array)
		{
			iTAudioEvent.Stop();
		}
	}

	private static int Compare(DelayAndIndex l, DelayAndIndex r)
	{
		if (l.deltaTime < r.deltaTime)
		{
			return -1;
		}
		if (l.deltaTime > r.deltaTime)
		{
			return 1;
		}
		return 0;
	}
}

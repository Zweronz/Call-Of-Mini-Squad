using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[AddComponentMenu("AudioEffect/AudioEffect Random")]
public class TAudioEffectRandom : ITAudioEvent
{
	public enum LoopMode
	{
		Default = 0,
		SingleLoop = 1,
		MultiLoop = 2
	}

	public bool isSfx = true;

	public AudioClip[] audioClips;

	public float[] probability;

	public float volumOffset;

	public float pitchOffset;

	public LoopMode loopMode;

	public bool cutoff;

	public int delayTime;

	public bool autoDestroy;

	public bool randomStartPosition;

	public int maxIntervalTime;

	public int intervalRangeMin;

	public int intervalRangeMax;

	public int delatLimit = 1000;

	private static Dictionary<string, int> s_random_index = new Dictionary<string, int>();

	private static Dictionary<string, float> s_play_interval = new Dictionary<string, float>();

	private ITAudioLimit[] m_audioLimits = new ITAudioLimit[0];

	private int m_lastRandomIndex = -1;

	private float m_volumBase;

	private float m_pitchBase;

	private float nullProbability = -1f;

	private bool m_isPlaying;

	private bool m_awake;

	private float m_playingTime;

	public int currentPlayIndex
	{
		get
		{
			return m_lastRandomIndex;
		}
	}

	public override bool isPlaying
	{
		get
		{
			return m_isPlaying;
		}
	}

	public override bool isLoop
	{
		get
		{
			if (loopMode == LoopMode.Default)
			{
				return base.GetComponent<AudioSource>().loop;
			}
			return true;
		}
	}

	private void Awake()
	{
		m_audioLimits = GetComponents<ITAudioLimit>();
		m_volumBase = base.GetComponent<AudioSource>().volume;
		m_pitchBase = base.GetComponent<AudioSource>().pitch;
		if (probability.Length > 0)
		{
			float num = 0f;
			float[] array = probability;
			foreach (float num2 in array)
			{
				num += num2;
			}
			if (num < 0.999f)
			{
				nullProbability = 1f - num;
			}
		}
		if (audioClips.Length == 1 && loopMode == LoopMode.MultiLoop)
		{
			loopMode = LoopMode.Default;
		}
		m_awake = true;
	}

	private void OnDestroy()
	{
		if (TAudioManager.checkInstance)
		{
			Stop();
		}
	}

	private IEnumerator TriggerDelay(float time)
	{
		float time_trigger = Time.realtimeSinceStartup;
		while (Time.realtimeSinceStartup - time_trigger < time)
		{
			yield return 0;
		}
		Trigger(true);
	}

	private IEnumerator PlayOver(float time)
	{
		float time_over = Time.realtimeSinceStartup;
		while (Time.realtimeSinceStartup - time_over < time)
		{
			yield return 0;
		}
		m_isPlaying = false;
		if (autoDestroy)
		{
			Object.Destroy(base.gameObject);
		}
	}

	private void SendTriggerEvent(AudioClip clip)
	{
		ITAudioLimit[] audioLimits = m_audioLimits;
		foreach (ITAudioLimit iTAudioLimit in audioLimits)
		{
			iTAudioLimit.OnAudioTrigger(clip);
		}
	}

	public override void Trigger()
	{
		Trigger(false);
	}

	private void Trigger(bool delay)
	{
		if (!m_awake)
		{
		}
		if (audioClips.Length == 0)
		{
			return;
		}
		string key = "AudioRandomIndex_" + base.name;
		ITAudioLimit[] audioLimits = m_audioLimits;
		foreach (ITAudioLimit iTAudioLimit in audioLimits)
		{
			if (!iTAudioLimit.isCanPlay)
			{
				return;
			}
		}
		if (loopMode == LoopMode.Default && !delay)
		{
			float num = (float)delayTime * 0.001f;
			if (maxIntervalTime > 0)
			{
				float num2 = 0f;
				if (s_play_interval.ContainsKey(key))
				{
					float num3 = s_play_interval[key];
					if (Time.realtimeSinceStartup - num3 > (float)maxIntervalTime)
					{
						num2 = Time.realtimeSinceStartup + num;
						s_play_interval[key] = num2;
					}
					else if (Time.realtimeSinceStartup - num3 < (float)intervalRangeMin * 0.001f)
					{
						num2 = num3 + Random.Range((float)intervalRangeMin * 0.001f, (float)intervalRangeMax * 0.001f);
						num = num2 - Time.realtimeSinceStartup;
						if (num < (float)delatLimit * 0.001f)
						{
							s_play_interval[key] = num2;
						}
					}
					else
					{
						num2 = Time.realtimeSinceStartup + num;
						s_play_interval[key] = num2;
					}
				}
				else
				{
					num2 = Time.realtimeSinceStartup + num;
					s_play_interval.Add(key, num2);
				}
			}
			if (num > 0.001f)
			{
				if (num < (float)delatLimit * 0.001f)
				{
					StartCoroutine(TriggerDelay(num));
				}
				return;
			}
		}
		bool flag = false;
		if (probability.Length == 0)
		{
			if (audioClips.Length == 1)
			{
				m_lastRandomIndex = 0;
				flag = true;
			}
			else
			{
				int num4 = Random.Range(0, 1000);
				if (s_random_index.ContainsKey(key))
				{
					m_lastRandomIndex = s_random_index[key];
				}
				if (m_lastRandomIndex == -1)
				{
					num4 %= audioClips.Length;
					m_lastRandomIndex = num4;
				}
				else
				{
					num4 %= audioClips.Length - 1;
					m_lastRandomIndex = (m_lastRandomIndex + num4 + 1) % audioClips.Length;
				}
				flag = true;
			}
		}
		else
		{
			if (s_random_index.ContainsKey(key))
			{
				m_lastRandomIndex = s_random_index[key];
			}
			float num5 = 0f;
			for (int j = 0; j < probability.Length; j++)
			{
				if (j != m_lastRandomIndex)
				{
					num5 += probability[j];
				}
			}
			if (nullProbability > 0.001f)
			{
				num5 += nullProbability;
			}
			float num6 = Random.Range(0f, num5);
			if (nullProbability > 0.001f)
			{
				num5 -= nullProbability;
				if (num6 > num5)
				{
					m_lastRandomIndex = -1;
					flag = true;
				}
			}
			if (!flag)
			{
				for (int num7 = probability.Length - 1; num7 >= 0; num7--)
				{
					if (num7 != m_lastRandomIndex)
					{
						num5 -= probability[num7];
						if (num6 > num5)
						{
							m_lastRandomIndex = num7;
							flag = true;
							break;
						}
					}
				}
			}
		}
		if (flag)
		{
			if (s_random_index.ContainsKey(key))
			{
				s_random_index[key] = m_lastRandomIndex;
			}
			else
			{
				s_random_index.Add(key, m_lastRandomIndex);
			}
		}
		if (m_lastRandomIndex == -1)
		{
			return;
		}
		AudioClip audioClip = audioClips[m_lastRandomIndex];
		if (!(null != audioClip) || TAudioManager.instance == null)
		{
			return;
		}
		base.GetComponent<AudioSource>().volume = Mathf.Clamp01(Random.Range(m_volumBase - volumOffset, m_volumBase + volumOffset));
		base.GetComponent<AudioSource>().pitch = Mathf.Clamp(Random.Range(m_pitchBase / (1f + pitchOffset), m_pitchBase * (1f + pitchOffset)), 0.01f, 3f);
		if (loopMode == LoopMode.Default)
		{
			if (isSfx)
			{
				TAudioManager.instance.PlaySound(base.GetComponent<AudioSource>(), audioClip, base.GetComponent<AudioSource>().loop, cutoff);
			}
			else
			{
				TAudioManager.instance.PlayMusic(base.GetComponent<AudioSource>(), audioClip, base.GetComponent<AudioSource>().loop, cutoff);
			}
			if (randomStartPosition)
			{
				base.GetComponent<AudioSource>().time = Random.Range(0f, audioClip.length);
			}
			if (!base.GetComponent<AudioSource>().loop)
			{
				StopAllCoroutines();
				StartCoroutine(PlayOver(audioClip.length / base.GetComponent<AudioSource>().pitch));
			}
			SendTriggerEvent(audioClip);
		}
		else if (loopMode == LoopMode.MultiLoop)
		{
			if (isSfx)
			{
				TAudioManager.instance.PlaySound(base.GetComponent<AudioSource>(), audioClip, true, true);
			}
			else
			{
				TAudioManager.instance.PlayMusic(base.GetComponent<AudioSource>(), audioClip, true, true);
			}
			if (!m_isPlaying && randomStartPosition)
			{
				base.GetComponent<AudioSource>().time = Random.Range(0f, audioClip.length);
			}
			StopAllCoroutines();
			StartCoroutine(TriggerDelay(audioClip.length / base.GetComponent<AudioSource>().pitch));
			SendTriggerEvent(audioClip);
		}
		else if (loopMode == LoopMode.SingleLoop)
		{
			if (isSfx)
			{
				TAudioManager.instance.PlaySound(base.GetComponent<AudioSource>(), audioClip, true);
			}
			else
			{
				TAudioManager.instance.PlayMusic(base.GetComponent<AudioSource>(), audioClip, true);
			}
			if (!m_isPlaying && randomStartPosition)
			{
				base.GetComponent<AudioSource>().time = Random.Range(0f, audioClip.length);
			}
			SendTriggerEvent(audioClip);
		}
		m_isPlaying = true;
	}

	public override void Stop()
	{
		m_isPlaying = false;
		if (loopMode == LoopMode.MultiLoop)
		{
			StopAllCoroutines();
		}
		else if (loopMode == LoopMode.Default && !base.GetComponent<AudioSource>().loop)
		{
			StopAllCoroutines();
		}
		if (isSfx)
		{
			TAudioManager.instance.StopSound(base.GetComponent<AudioSource>());
		}
		else
		{
			TAudioManager.instance.StopMusic(base.GetComponent<AudioSource>());
		}
	}

	public static void ClearMemory()
	{
		s_random_index.Clear();
		s_play_interval.Clear();
	}
}

using UnityEngine;

[AddComponentMenu("AudioEffect/AudioEffect Sequence")]
[RequireComponent(typeof(AudioSource))]
public class TAudioEffectSequence : ITAudioEvent
{
	public bool isSfx = true;

	public AudioClip[] audioClips;

	public float deltaTime = 0.2f;

	private ITAudioLimit[] m_audioLimits = new ITAudioLimit[0];

	private int m_lastPlayIndex = -1;

	private float m_triggerTime;

	private bool m_isTimeout = true;

	private bool m_awake;

	public override bool isPlaying
	{
		get
		{
			return false;
		}
	}

	public override bool isLoop
	{
		get
		{
			return false;
		}
	}

	private void Awake()
	{
		m_audioLimits = GetComponents<ITAudioLimit>();
		m_awake = true;
	}

	private void Update()
	{
		if (!m_isTimeout && Time.realtimeSinceStartup - m_triggerTime > deltaTime)
		{
			m_isTimeout = true;
		}
	}

	private void OnDestroy()
	{
		if (TAudioManager.checkInstance)
		{
			Stop();
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
		if (!m_awake)
		{
		}
		if (audioClips.Length == 0)
		{
			return;
		}
		ITAudioLimit[] audioLimits = m_audioLimits;
		foreach (ITAudioLimit iTAudioLimit in audioLimits)
		{
			if (!iTAudioLimit.isCanPlay)
			{
				return;
			}
		}
		if (deltaTime < 0f)
		{
			m_lastPlayIndex = Mathf.Min(m_lastPlayIndex + 1, audioClips.Length - 1);
		}
		else
		{
			m_lastPlayIndex = ((!m_isTimeout) ? Mathf.Min(m_lastPlayIndex + 1, audioClips.Length - 1) : 0);
			m_isTimeout = false;
			m_triggerTime = Time.realtimeSinceStartup;
		}
		AudioClip audioClip = audioClips[m_lastPlayIndex];
		if (null != audioClip)
		{
			if (isSfx)
			{
				TAudioManager.instance.PlaySound(base.GetComponent<AudioSource>(), audioClip, base.GetComponent<AudioSource>().loop);
			}
			else
			{
				TAudioManager.instance.PlayMusic(base.GetComponent<AudioSource>(), audioClip, base.GetComponent<AudioSource>().loop);
			}
			SendTriggerEvent(audioClip);
		}
	}

	public override void Stop()
	{
		if (!(TAudioManager.instance == null))
		{
			if (isSfx)
			{
				TAudioManager.instance.StopSound(base.GetComponent<AudioSource>());
			}
			else
			{
				TAudioManager.instance.StopMusic(base.GetComponent<AudioSource>());
			}
		}
	}
}

using System.Collections;
using UnityEngine;

public class EffectAudioBehaviour : TAudioController
{
	public string[] m_AudioName;

	public float[] m_DelayTime;

	public bool m_playOnEnable;

	public bool m_DeleteWhenEnd;

	private bool m_hasStart;

	public void Awake()
	{
		if (m_DelayTime == null || m_DelayTime.Length == 0)
		{
			m_DelayTime = new float[m_AudioName.Length];
			for (int i = 0; i < m_DelayTime.Length; i++)
			{
				m_DelayTime[i] = 0f;
			}
		}
	}

	public void Start()
	{
	}

	public void OnEnable()
	{
		if (m_playOnEnable)
		{
			PlaySFX();
		}
	}

	public void OnDestroy()
	{
	}

	public void Update()
	{
	}

	private IEnumerator PlaySfxDelay(int index)
	{
		yield return new WaitForSeconds(m_DelayTime[index]);
		PlayAudio(m_AudioName[index]);
	}

	public void PlaySFX()
	{
		for (int i = 0; i < m_AudioName.Length; i++)
		{
			StartCoroutine(PlaySfxDelay(i));
		}
	}

	public void StopSFX()
	{
		StopAllCoroutines();
		for (int i = 0; i < m_AudioName.Length; i++)
		{
			StopAudio(m_AudioName[i]);
		}
	}
}

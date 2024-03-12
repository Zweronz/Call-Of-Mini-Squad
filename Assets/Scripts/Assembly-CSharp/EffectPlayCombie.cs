using UnityEngine;

public class EffectPlayCombie : MonoBehaviour
{
	public bool lockDirection = true;

	private EffectControl[] m_effectControls;

	private EffectAudioBehaviour audioEvents;

	public void Awake()
	{
		m_effectControls = base.gameObject.GetComponentsInChildren<EffectControl>();
		audioEvents = base.gameObject.GetComponentInChildren<EffectAudioBehaviour>();
	}

	public void Start()
	{
	}

	public void Update()
	{
		if (lockDirection)
		{
			base.transform.rotation = Quaternion.identity;
		}
	}

	public virtual void StartEmit(bool playAudio = true)
	{
		if (m_effectControls != null)
		{
			for (int i = 0; i < m_effectControls.Length; i++)
			{
				m_effectControls[i].StartEmit();
			}
		}
		base.gameObject.SetActive(true);
		if (playAudio && DataCenter.Save().PlaySound && audioEvents != null)
		{
			audioEvents.PlaySFX();
		}
	}

	public virtual void StopEmit()
	{
		if (m_effectControls != null)
		{
			for (int i = 0; i < m_effectControls.Length; i++)
			{
				m_effectControls[i].StopEmit();
			}
		}
		if (DataCenter.Save().PlaySound && audioEvents != null)
		{
			audioEvents.StopSFX();
		}
		base.gameObject.SetActive(false);
	}
}

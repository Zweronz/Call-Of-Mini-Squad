using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioTalkManager : MonoBehaviour
{
	private const ushort playProbabilityDeath = 100;

	private const ushort playProbabilityHurt = 100;

	private const ushort playProbabilityFire = 100;

	private const ushort playProbabilityKill = 10;

	private const ushort playProbabilityLife = 100;

	private const ushort playProbabilitySelect = 100;

	private const ushort playProbabilitySkill = 80;

	private bool m_bPlaySkill;

	private float m_playSkillTimer;

	public AudioClip[] death;

	public AudioClip[] hurt;

	public AudioClip[] fire;

	public AudioClip[] kill;

	public AudioClip[] life;

	public AudioClip[] select;

	public AudioClip[] skill;

	private void Update()
	{
		if (m_bPlaySkill)
		{
			m_playSkillTimer += Time.deltaTime;
			if (m_playSkillTimer >= 0.7f)
			{
				m_bPlaySkill = false;
				m_playSkillTimer = 0f;
				PlaySkill(true);
			}
		}
	}

	public void PlayDeath()
	{
		if (DataCenter.Save().PlaySound)
		{
			float num = Random.Range(0, 100);
			if (num < 100f)
			{
				int num2 = Random.Range(0, death.Length);
				TAudioManager.instance.PlayTalk(base.GetComponent<AudioSource>(), death[num2], false, true);
			}
		}
	}

	public void PlayHurt()
	{
		if (DataCenter.Save().PlaySound)
		{
			float num = Random.Range(0, 100);
			if (num < 100f)
			{
				int num2 = Random.Range(0, hurt.Length);
				TAudioManager.instance.PlayTalk(base.GetComponent<AudioSource>(), hurt[num2], false, true);
			}
		}
	}

	public void PlayFire()
	{
		if (DataCenter.Save().PlaySound)
		{
			float num = Random.Range(0, 100);
			if (num < 100f)
			{
				int num2 = Random.Range(0, fire.Length);
				TAudioManager.instance.PlayTalk(base.GetComponent<AudioSource>(), fire[num2], false, true);
			}
		}
	}

	public void PlayKill()
	{
		if (DataCenter.Save().PlaySound)
		{
			float num = Random.Range(0, 100);
			if (num < 10f)
			{
				int num2 = Random.Range(0, kill.Length);
				TAudioManager.instance.PlayTalk(base.GetComponent<AudioSource>(), kill[num2], false, true);
			}
		}
	}

	public void PlayLife()
	{
		if (DataCenter.Save().PlaySound)
		{
			float num = Random.Range(0, 100);
			if (num < 100f)
			{
				int num2 = Random.Range(0, life.Length);
				TAudioManager.instance.PlayTalk(base.GetComponent<AudioSource>(), life[num2], false, true);
			}
		}
	}

	public void PlaySelect()
	{
		if (DataCenter.Save().PlaySound)
		{
			float num = Random.Range(0, 100);
			if (num < 100f)
			{
				int num2 = Random.Range(0, select.Length);
				TAudioManager.instance.PlayTalk(base.GetComponent<AudioSource>(), select[num2], false, true);
			}
		}
	}

	public void PlaySkill(bool play = false)
	{
		if (!DataCenter.Save().PlaySound)
		{
			return;
		}
		if (!play)
		{
			float num = Random.Range(0, 100);
			if (num < 80f)
			{
				m_bPlaySkill = true;
			}
		}
		else
		{
			m_bPlaySkill = false;
			int num2 = Random.Range(0, skill.Length);
			TAudioManager.instance.PlayTalk(base.GetComponent<AudioSource>(), skill[num2], false, true);
		}
	}
}

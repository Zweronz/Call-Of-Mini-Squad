using UnityEngine;

public class UITeamModelAnimationManager : MonoBehaviour
{
	private string m_defaultAnimName;

	private string m_showAnimName;

	private bool m_bPlayShowAnim;

	private bool m_bAnimLoop;

	public void SetAnimationInfos(string defaultAnimaName, string showAnimName)
	{
		m_defaultAnimName = defaultAnimaName;
		m_showAnimName = showAnimName;
	}

	public void PlayDefaultAnim(bool loop)
	{
		string defaultAnimName = TryGetAnimationName(m_defaultAnimName);
		base.gameObject.GetComponent<Animation>()[defaultAnimName].wrapMode = ((!loop) ? WrapMode.Once : WrapMode.Loop);
		base.gameObject.GetComponent<Animation>().Play(defaultAnimName);
		m_bPlayShowAnim = false;
		m_bAnimLoop = loop;
	}

	public void PlayShowAnim(bool loop)
	{
		string showAnimName = TryGetAnimationName(m_showAnimName);
		base.gameObject.GetComponent<Animation>()[showAnimName].wrapMode = ((!loop) ? WrapMode.Once : WrapMode.Loop);
		base.gameObject.GetComponent<Animation>().Play(showAnimName);
		m_bPlayShowAnim = true;
		m_bAnimLoop = loop;
	}

	private string TryGetAnimationName(string animation)
	{
		//thanks jeremy
		for (int i = -1; i < 11; i++)
		{
			if (base.gameObject.GetComponent<Animation>()[animation + (i == -1 ? "" : "_" + i)] != null)
			{
				return animation + (i == -1 ? "" : "_" + i);
			}
		}

		return animation;
	}

	private void Update()
	{
		if (m_bPlayShowAnim && !base.GetComponent<Animation>().IsPlaying(m_showAnimName) && !m_bAnimLoop)
		{
			PlayDefaultAnim(true);
		}
	}
}

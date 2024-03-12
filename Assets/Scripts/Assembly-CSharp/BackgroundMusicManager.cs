using UnityEngine;

public class BackgroundMusicManager : MonoBehaviour
{
	public enum MusicType
	{
		UI_BG = 0,
		BGM_Channel = 1,
		BGM_Laboratory = 2,
		BGM_Ruin = 3,
		BGM_BOSS = 4,
		BGM_Victory = 5,
		BGM_Failure = 6
	}

	private static BackgroundMusicManager sInstance;

	private TAudioController m_LastAudioController;

	private TAudioController m_CurrentAudioController;

	private string m_LastAudioName = string.Empty;

	private string m_curAudioName = string.Empty;

	public static BackgroundMusicManager Instance()
	{
		return sInstance;
	}

	private void Awake()
	{
		sInstance = this;
		Object.DontDestroyOnLoad(base.gameObject);
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void PlayBackgroundMusic(MusicType type)
	{
		string text = "BGM_Theme";
		switch (type)
		{
		case MusicType.UI_BG:
			text = "BGM_Theme";
			break;
		case MusicType.BGM_Channel:
			text = "BGM_Channel";
			break;
		case MusicType.BGM_Laboratory:
			text = "BGM_Laboratory";
			break;
		case MusicType.BGM_Ruin:
			text = "BGM_Ruin";
			break;
		case MusicType.BGM_Victory:
			text = "BGM_Victory";
			break;
		case MusicType.BGM_Failure:
			text = "BGM_Failure";
			break;
		case MusicType.BGM_BOSS:
			text = "BGM_BOSS";
			break;
		}
		if (m_LastAudioName == text)
		{
			if (!(m_CurrentAudioController != null))
			{
				return;
			}
			Object.Destroy(m_CurrentAudioController.gameObject);
			m_CurrentAudioController = null;
			if (m_LastAudioController != null)
			{
				m_curAudioName = m_LastAudioName;
				m_CurrentAudioController = m_LastAudioController;
				m_LastAudioController = null;
				m_LastAudioName = string.Empty;
			}
			if (!(m_CurrentAudioController != null) || !DataCenter.Save().PlayMusic)
			{
				return;
			}
			Transform transform = m_CurrentAudioController.transform.Find("Audio/" + text);
			if (transform != null)
			{
				AudioSource component = transform.GetComponent<AudioSource>();
				if (component != null)
				{
					component.Play();
				}
			}
			else
			{
				m_CurrentAudioController.PlayAudio(text);
			}
		}
		else if (m_curAudioName != text)
		{
			GameObject gameObject = new GameObject(text);
			gameObject.transform.parent = base.transform;
			gameObject.transform.localPosition = Vector3.zero;
			if (m_LastAudioController != null)
			{
				Object.Destroy(m_LastAudioController.gameObject);
			}
			if (m_CurrentAudioController != null)
			{
				if (m_curAudioName == "Mus_intro")
				{
					Object.Destroy(m_CurrentAudioController.gameObject);
				}
				else
				{
					Transform transform2 = m_CurrentAudioController.transform.Find("Audio/" + m_curAudioName);
					if (transform2 != null)
					{
						AudioSource component2 = transform2.GetComponent<AudioSource>();
						if (component2 != null)
						{
							component2.Pause();
						}
					}
				}
				m_LastAudioName = m_curAudioName;
				m_LastAudioController = m_CurrentAudioController;
				m_curAudioName = text;
			}
			m_curAudioName = text;
			m_CurrentAudioController = gameObject.AddComponent<TAudioController>();
			if (DataCenter.Save().PlayMusic)
			{
				m_CurrentAudioController.PlayAudio(text);
			}
		}
		else
		{
			if (!(m_CurrentAudioController != null) || !DataCenter.Save().PlayMusic)
			{
				return;
			}
			Transform transform3 = m_CurrentAudioController.transform.Find("Audio/" + text);
			if (transform3 != null)
			{
				AudioSource component3 = transform3.GetComponent<AudioSource>();
				if (component3 != null)
				{
					component3.Play();
				}
			}
			else
			{
				m_CurrentAudioController.PlayAudio(text);
			}
		}
	}

	public void PlayBackgroundMusic(string musicFileName)
	{
		if (m_LastAudioName == musicFileName)
		{
			if (!(m_CurrentAudioController != null))
			{
				return;
			}
			Object.Destroy(m_CurrentAudioController.gameObject);
			m_CurrentAudioController = null;
			if (m_LastAudioController != null)
			{
				m_curAudioName = m_LastAudioName;
				m_CurrentAudioController = m_LastAudioController;
				m_LastAudioController = null;
				m_LastAudioName = string.Empty;
			}
			if (!(m_CurrentAudioController != null) || !DataCenter.Save().PlayMusic)
			{
				return;
			}
			Transform transform = m_CurrentAudioController.transform.Find("Audio/" + musicFileName);
			if (transform != null)
			{
				AudioSource component = transform.GetComponent<AudioSource>();
				if (component != null)
				{
					component.Play();
				}
			}
			else
			{
				m_CurrentAudioController.PlayAudio(musicFileName);
			}
		}
		else if (m_curAudioName != musicFileName)
		{
			GameObject gameObject = new GameObject(musicFileName);
			gameObject.transform.parent = base.transform;
			gameObject.transform.localPosition = Vector3.zero;
			if (m_LastAudioController != null)
			{
				Object.Destroy(m_LastAudioController.gameObject);
			}
			if (m_CurrentAudioController != null)
			{
				if (m_curAudioName == "Mus_intro")
				{
					Object.Destroy(m_CurrentAudioController.gameObject);
				}
				else
				{
					Transform transform2 = m_CurrentAudioController.transform.Find("Audio/" + m_curAudioName);
					if (transform2 != null)
					{
						AudioSource component2 = transform2.GetComponent<AudioSource>();
						if (component2 != null)
						{
							component2.Pause();
						}
					}
				}
				m_LastAudioName = m_curAudioName;
				m_LastAudioController = m_CurrentAudioController;
				m_curAudioName = musicFileName;
			}
			m_curAudioName = musicFileName;
			m_CurrentAudioController = gameObject.AddComponent<TAudioController>();
			if (DataCenter.Save().PlayMusic)
			{
				m_CurrentAudioController.PlayAudio(musicFileName);
			}
		}
		else
		{
			if (!(m_CurrentAudioController != null) || !DataCenter.Save().PlayMusic)
			{
				return;
			}
			Transform transform3 = m_CurrentAudioController.transform.Find("Audio/" + musicFileName);
			if (transform3 != null)
			{
				AudioSource component3 = transform3.GetComponent<AudioSource>();
				if (component3 != null)
				{
					component3.Play();
				}
			}
			else
			{
				m_CurrentAudioController.PlayAudio(musicFileName);
			}
		}
	}

	public void StopBG()
	{
		m_CurrentAudioController.StopAudio(m_curAudioName);
	}
}

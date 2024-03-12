using UnityEngine;

[RequireComponent(typeof(TAudioController))]
public class AudioControllerEx : MonoBehaviour
{
	public bool isElite;

	protected TAudioController m_AudioController;

	private void Awake()
	{
		m_AudioController = GetComponent<TAudioController>();
	}

	public void PlayAudioWithCondition(string sName)
	{
		if (!(m_AudioController == null) && !(base.transform.root == null))
		{
			if (isElite)
			{
				sName += "_Elite";
			}
			m_AudioController.PlayAudio(sName);
		}
	}
}

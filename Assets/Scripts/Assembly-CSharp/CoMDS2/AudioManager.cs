using System.Collections.Generic;
using UnityEngine;

namespace CoMDS2
{
	public class AudioManager
	{
		private Character m_object;

		private Dictionary<string, ITAudioEvent> m_audios;

		private AudioManager()
		{
		}

		public AudioManager(Character callBack)
		{
			m_object = callBack;
			m_audios = new Dictionary<string, ITAudioEvent>();
			Transform transform = m_object.GetTransform().Find("Audios");
			if (transform == null)
			{
				return;
			}
			for (int i = 0; i < transform.childCount; i++)
			{
				GameObject gameObject = transform.GetChild(i).gameObject;
				ITAudioEvent componentInChildren = gameObject.GetComponentInChildren<ITAudioEvent>();
				if (componentInChildren != null)
				{
					m_audios.Add(gameObject.name, componentInChildren);
				}
			}
		}

		public void PlayAudio(string name)
		{
			if (m_audios.ContainsKey(name))
			{
				m_audios[name].Trigger();
			}
		}

		public void StopAudio(string name)
		{
			if (m_audios.ContainsKey(name))
			{
				m_audios[name].Stop();
			}
		}
	}
}

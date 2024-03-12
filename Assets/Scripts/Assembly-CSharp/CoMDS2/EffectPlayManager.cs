using System.Collections.Generic;
using UnityEngine;

namespace CoMDS2
{
	public class EffectPlayManager
	{
		private DS2ActiveObject m_object;

		private Dictionary<string, EffectControl> m_effects;

		private Dictionary<string, Transform> m_bindObject;

		private EffectPlayManager()
		{
		}

		public EffectPlayManager(DS2ActiveObject callBack)
		{
			m_object = callBack;
			m_effects = new Dictionary<string, EffectControl>();
			m_bindObject = new Dictionary<string, Transform>();
			Transform transform = m_object.GetTransform().Find("Effects");
			if (transform == null)
			{
				return;
			}
			for (int i = 0; i < transform.childCount; i++)
			{
				GameObject gameObject = transform.GetChild(i).gameObject;
				EffectControl componentInChildren = gameObject.GetComponentInChildren<EffectControl>();
				if (componentInChildren != null)
				{
					componentInChildren.StopEmit();
					m_effects.Add(gameObject.name, componentInChildren);
				}
				m_bindObject.Add(gameObject.name, gameObject.transform);
			}
		}

		public void PlayEffect(string name, bool bind = true)
		{
			if (m_effects.ContainsKey(name))
			{
				if (!bind)
				{
					m_effects[name].Root.transform.parent = BattleBufferManager.s_effectObjectRoot.transform;
					m_effects[name].Root.transform.position = m_bindObject[name].position;
					m_effects[name].Root.transform.rotation = m_bindObject[name].rotation;
					m_effects[name].Root.transform.localScale = m_bindObject[name].localScale;
				}
				m_effects[name].gameObject.SetActive(true);
				m_effects[name].StartEmit();
			}
		}

		public void PlayEffect(string name, Vector3 position, bool worldPosition = false)
		{
			if (m_effects.ContainsKey(name))
			{
				m_effects[name].transform.position = position;
				if (worldPosition)
				{
					m_effects[name].transform.transform.parent = BattleBufferManager.s_effectObjectRoot.transform;
				}
				m_effects[name].gameObject.SetActive(true);
				m_effects[name].StartEmit();
			}
		}

		public void StopEffect(string name)
		{
			if (m_effects.ContainsKey(name))
			{
				m_effects[name].StopEmit();
			}
		}

		public EffectControl GetEffectControl(string name)
		{
			if (m_effects.ContainsKey(name))
			{
				return m_effects[name];
			}
			return null;
		}
	}
}

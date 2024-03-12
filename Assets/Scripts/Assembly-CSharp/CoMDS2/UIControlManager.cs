using System.Collections.Generic;
using UnityEngine;

namespace CoMDS2
{
	public class UIControlManager
	{
		private static Dictionary<int, GameObject> m_controlMap;

		private static UIControlManager instance;

		private GameObjectBuffer m_enemyHpBarBuffer;

		public static UIControlManager Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new UIControlManager();
					m_controlMap = new Dictionary<int, GameObject>();
				}
				return instance;
			}
		}

		private UIControlManager()
		{
		}

		public void AddControl(int id, GameObject control)
		{
			if (!m_controlMap.ContainsKey(id))
			{
				m_controlMap.Add(id, control);
			}
		}

		public GameObject GetControl(int id)
		{
			if (m_controlMap.ContainsKey(id))
			{
				return m_controlMap[id];
			}
			return null;
		}

		public void Clear()
		{
			m_controlMap.Clear();
			if (m_enemyHpBarBuffer != null)
			{
				m_enemyHpBarBuffer.Dispose();
				m_enemyHpBarBuffer = null;
			}
		}

		public void AddEnemyHpBarToBuffer(GameObject go)
		{
			if (m_enemyHpBarBuffer == null)
			{
				m_enemyHpBarBuffer = new GameObjectBuffer(30);
			}
			m_enemyHpBarBuffer.AddObj(go);
		}

		public GameObject GetEnemyHpBarFromBuffer()
		{
			return m_enemyHpBarBuffer.GetObject();
		}
	}
}

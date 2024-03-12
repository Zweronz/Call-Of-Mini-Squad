using System.Collections.Generic;
using UnityEngine;

namespace CoMDS2
{
	public class NodeGroup
	{
		private List<DS2Object> m_objectNodeList = new List<DS2Object>();

		private List<EnemySpawnNode> m_spawnPointNodeList = new List<EnemySpawnNode>();

		private bool m_enable = true;

		public Vector3 position { get; set; }

		public Bounds bounds { get; set; }

		public bool Enable
		{
			get
			{
				return m_enable;
			}
			set
			{
				m_enable = value;
			}
		}

		public void AddNode(DS2Object obj)
		{
			m_objectNodeList.Add(obj);
		}

		public void RemoveNode(DS2Object obj)
		{
			if (m_objectNodeList.Contains(obj))
			{
				m_objectNodeList.Remove(obj);
			}
		}

		public void AddNode(EnemySpawnNode spawnPoint)
		{
			m_spawnPointNodeList.Add(spawnPoint);
		}

		public void RemoveNode(EnemySpawnNode spawnPoint)
		{
			if (m_spawnPointNodeList.Contains(spawnPoint))
			{
				m_spawnPointNodeList.Remove(spawnPoint);
			}
		}

		public void Update()
		{
			if (!m_enable)
			{
				return;
			}
			foreach (DS2Object objectNode in m_objectNodeList)
			{
				objectNode.Update(Time.deltaTime);
			}
			foreach (EnemySpawnNode spawnPointNode in m_spawnPointNodeList)
			{
				spawnPointNode.Update();
				spawnPointNode.TeleportObject();
			}
		}

		public EnemySpawnNode[] GetSpawnPointNodeList()
		{
			return m_spawnPointNodeList.ToArray();
		}

		public Vector3 GetRandomSpawnPointPosition()
		{
			if (m_spawnPointNodeList.Count == 0)
			{
			}
			int index = Random.Range(0, m_spawnPointNodeList.Count);
			return m_spawnPointNodeList[index].position;
		}
	}
}

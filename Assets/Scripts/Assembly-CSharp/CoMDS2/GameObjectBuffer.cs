using UnityEngine;

namespace CoMDS2
{
	public class GameObjectBuffer
	{
		private GameObject[] m_buff;

		private int m_iIndex;

		private int m_iSearchIndex;

		private int m_iSize;

		public int Size
		{
			get
			{
				return m_iSize;
			}
		}

		public GameObjectBuffer(int count)
		{
			m_iSize = count;
			m_buff = new GameObject[count];
		}

		public void AddObj(GameObject obj)
		{
			if (m_iIndex < m_iSize)
			{
				m_buff[m_iIndex++] = obj;
			}
		}

		public GameObject GetObject()
		{
			GameObject result = null;
			for (int i = m_iSearchIndex + 1; i < m_iSize; i++)
			{
				if (m_buff[i] == null || !m_buff[i].activeInHierarchy)
				{
					m_iSearchIndex = i;
					return m_buff[i];
				}
			}
			for (int j = 0; j < m_iSearchIndex + 1; j++)
			{
				if (m_buff[j] == null || !m_buff[j].activeInHierarchy)
				{
					m_iSearchIndex = j;
					return m_buff[j];
				}
			}
			return result;
		}

		public GameObject[] GetBuffer()
		{
			return m_buff;
		}

		public void Dispose()
		{
			for (int i = 0; i < m_iSize; i++)
			{
				Object.Destroy(m_buff[i]);
				m_buff[i] = null;
			}
			m_buff = null;
		}
	}
}

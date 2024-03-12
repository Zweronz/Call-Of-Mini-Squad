using System;
using UnityEngine;

namespace CoMDS2
{
	public class DS2ObjectBuffer
	{
		private DS2Object[] m_buff;

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

		public DS2ObjectBuffer(int count)
		{
			m_iSize = count;
			m_buff = new DS2Object[count];
		}

		public void AddObj(DS2Object obj)
		{
			m_buff[m_iIndex++] = obj;
		}

		public DS2Object GetObject()
		{
			DS2Object result = null;
			for (int i = m_iSearchIndex + 1; i < m_iSize; i++)
			{
				if (m_buff[i].GetGameObject() == null || !m_buff[i].GetGameObject().activeInHierarchy)
				{
					m_iSearchIndex = i;
					return m_buff[i];
				}
			}
			for (int j = 0; j < m_iSearchIndex + 1; j++)
			{
				if (m_buff[j].GetGameObject() == null || !m_buff[j].GetGameObject().activeInHierarchy)
				{
					m_iSearchIndex = j;
					return m_buff[j];
				}
			}
			return result;
		}

		public void AddBuffer(DS2ObjectBuffer buffer, int maxSize = -1)
		{
			int num = m_iSize + buffer.Size;
			if (maxSize != -1)
			{
				if (maxSize <= m_iSize)
				{
					return;
				}
				num = Mathf.Min(maxSize, m_iSize + buffer.Size);
			}
			DS2Object[] array = new DS2Object[num];
			Array.ConstrainedCopy(m_buff, 0, array, 0, m_iSize);
			DS2Object[] buffer2 = buffer.GetBuffer();
			Array.ConstrainedCopy(buffer2, 0, array, m_iSize, num - m_iSize);
			m_iSize = num;
			m_buff = null;
			m_buff = array;
		}

		public DS2Object[] GetBuffer()
		{
			return m_buff;
		}

		public void Dispose()
		{
			for (int i = 0; i < m_iSize; i++)
			{
				m_buff[i].Destroy(true);
				m_buff[i] = null;
			}
			m_buff = null;
		}
	}
}

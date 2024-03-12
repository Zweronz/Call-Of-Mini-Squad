using System;
using System.Collections.Generic;
using UnityEngine;

namespace CoMDS2
{
	public class AllySeat
	{
		private NumberSection<float>[] m_sections;

		private int seatCount;

		private List<int> m_seats = new List<int>();

		public void Init(int count)
		{
			m_sections = null;
			seatCount = count;
			if (m_sections == null)
			{
				m_sections = new NumberSection<float>[count];
				float num = 360 / m_sections.Length;
				for (int i = 0; i < m_sections.Length; i++)
				{
					float l = (float)i * num;
					float r = (float)(i + 1) * num;
					m_sections[i] = new NumberSection<float>(l, r);
				}
			}
			m_seats.Clear();
			for (int j = 0; j < count; j++)
			{
				m_seats.Add(j);
			}
		}

		public void AddSeat(int seatid)
		{
			if (!m_seats.Contains(seatid))
			{
				m_seats.Add(seatid);
			}
		}

		public Vector3 GetSeatAndPosition(ref int curSeat, bool searchInCurrSeat = false)
		{
			if (m_seats.Count == 0)
			{
				Init(seatCount);
			}
			Vector3 result;
			if (searchInCurrSeat)
			{
				float num = UnityEngine.Random.Range(m_sections[curSeat].left, m_sections[curSeat].right);
				result = new Vector3(Mathf.Cos(num * ((float)Math.PI / 180f)), 0f, Mathf.Sin(num * ((float)Math.PI / 180f)));
			}
			else
			{
				int index = UnityEngine.Random.Range(0, m_seats.Count);
				int num2 = m_seats[index];
				float num3 = UnityEngine.Random.Range(m_sections[num2].left, m_sections[num2].right);
				result = new Vector3(Mathf.Cos(num3 * ((float)Math.PI / 180f)), 0f, Mathf.Sin(num3 * ((float)Math.PI / 180f)));
				AddSeat(curSeat);
				curSeat = num2;
				m_seats.Remove(num2);
			}
			return result;
		}
	}
}

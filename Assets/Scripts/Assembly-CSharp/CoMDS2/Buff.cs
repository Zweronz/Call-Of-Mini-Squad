using System.Collections.Generic;
using UnityEngine;

namespace CoMDS2
{
	public class Buff
	{
		public enum AffectType
		{
			Hp = 0,
			MoveSpeed = 1,
			SkillAddHp = 2,
			ReduceDamage = 3,
			AddDamage = 4,
			Poison = 5,
			AddATK = 6,
			Exacerbate = 7,
			Knell = 8,
			Confuse = 9,
			ReduceATK = 10,
			Bleeding = 11,
			HitRate = 12,
			SpecialAttributeHp = 13,
			Miasma = 14,
			Defense = 15,
			ShootRange = 16,
			AtkPrequency = 17
		}

		public enum CalcType
		{
			General = 0,
			Percentage = 1
		}

		private CalcType m_calcType;

		public float m_value;

		private float m_time;

		private float m_timer;

		private float m_interal;

		private float m_interalTimer;

		private float m_extraValue;

		public List<GameObject> effectCallBack = new List<GameObject>();

		private int m_probability = 100;

		public Defined.EFFECT_TYPE effect = Defined.EFFECT_TYPE.NONE;

		public Transform effect_bindTransform;

		public AffectType affectType { get; set; }

		public float preValue { get; set; }

		public Character belong { get; set; }

		public Character creater { get; set; }

		public int Probability
		{
			get
			{
				float num = m_probability;
				if ((m_value < 0f || affectType == AffectType.AddDamage || affectType == AffectType.ReduceATK) && (belong.objectType == Defined.OBJECT_TYPE.OBJECT_TYPE_PLAYER || belong.objectType == Defined.OBJECT_TYPE.OBJECT_TYPE_ALLY))
				{
					Player player = (Player)belong;
					if (player.HasTalent(TeamSpecialAttribute.TeamAttributeType.Nanotech))
					{
						float num2 = DataCenter.Conf().m_teamAttributeNanotech.reduceDebuffPercent[player.teamData.talents[TeamSpecialAttribute.TeamAttributeType.Nanotech] - 1];
						num -= num * num2;
					}
				}
				return (int)num;
			}
			set
			{
				m_probability = value;
			}
		}

		public Buff(AffectType affectType, float value, float interal, float time)
		{
			this.affectType = affectType;
			m_value = value;
			m_time = time;
			m_interal = interal;
			m_timer = 0f;
			m_probability = 100;
		}

		public Buff(AffectType affectType, float value, float interal, float time, CalcType calcType, float extraValue)
		{
			this.affectType = affectType;
			m_value = value;
			m_time = time;
			m_calcType = calcType;
			m_interal = interal;
			m_extraValue = extraValue;
			m_timer = 0f;
			m_probability = 100;
		}

		public bool isMilitate()
		{
			m_interalTimer += Time.deltaTime;
			m_timer += Time.deltaTime;
			if (m_interalTimer >= m_interal)
			{
				m_interalTimer -= m_interal;
				return true;
			}
			return false;
		}

		public bool isOver()
		{
			if (m_time == -1f)
			{
				return false;
			}
			if (m_timer >= m_time)
			{
				return true;
			}
			return false;
		}

		public float CalcValue(float value)
		{
			switch (m_calcType)
			{
			case CalcType.General:
				value += m_value;
				break;
			case CalcType.Percentage:
				value += value * m_value * 0.01f;
				break;
			}
			return value;
		}

		public int CalcValue(int value)
		{
			switch (m_calcType)
			{
			case CalcType.General:
				value += (int)(m_value + m_extraValue);
				break;
			case CalcType.Percentage:
				value += (int)((float)value * m_value + m_extraValue);
				break;
			}
			return value;
		}

		public float CalcValue(float value, float maxValue)
		{
			switch (m_calcType)
			{
			case CalcType.General:
				value += (float)(int)(m_value + m_extraValue);
				break;
			case CalcType.Percentage:
				value += (float)(int)(maxValue * m_value + m_extraValue);
				break;
			}
			return value;
		}

		public void Relieve()
		{
			m_value = preValue;
		}

		public float GetValue()
		{
			return m_value;
		}

		public float GetTime()
		{
			return m_time;
		}

		public float GetTimer()
		{
			return m_timer;
		}

		public void Init()
		{
			m_timer = 0f;
			m_interalTimer = 0f;
		}
	}
}

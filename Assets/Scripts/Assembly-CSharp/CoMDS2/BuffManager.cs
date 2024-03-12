using System.Collections.Generic;
using UnityEngine;

namespace CoMDS2
{
	public class BuffManager : IBuffManager
	{
		private Dictionary<Buff.AffectType, Buff> m_buffs = new Dictionary<Buff.AffectType, Buff>();

		private List<Buff.AffectType> m_removeBuffs = new List<Buff.AffectType>();

		private Character m_character;

		private Dictionary<Buff.AffectType, int> m_buffAccumulateCount = new Dictionary<Buff.AffectType, int>();

		private BuffManager()
		{
		}

		public BuffManager(Character character)
		{
			m_character = character;
			m_buffs.Clear();
			m_removeBuffs.Clear();
		}

		public void AddBuff(Buff buff, int accumulateMaxCount = -1)
		{
			if (m_buffs == null)
			{
				m_buffs = new Dictionary<Buff.AffectType, Buff>();
				m_removeBuffs = new List<Buff.AffectType>();
				m_buffAccumulateCount = new Dictionary<Buff.AffectType, int>();
			}
			buff.belong = m_character;
			if (!buff.belong.Alive())
			{
				return;
			}
			int num = Random.Range(0, 100);
			if (num > buff.Probability)
			{
				return;
			}
			buff.Init();
			if (m_buffAccumulateCount.ContainsKey(buff.affectType))
			{
				if (accumulateMaxCount != -1)
				{
					if (m_buffAccumulateCount[buff.affectType] >= accumulateMaxCount)
					{
						m_buffs[buff.affectType].Init();
						return;
					}
					Dictionary<Buff.AffectType, int> buffAccumulateCount;
					Dictionary<Buff.AffectType, int> dictionary = (buffAccumulateCount = m_buffAccumulateCount);
					Buff.AffectType affectType;
					Buff.AffectType key = (affectType = buff.affectType);
					int num2 = buffAccumulateCount[affectType];
					dictionary[key] = num2 + 1;
				}
			}
			else
			{
				m_buffAccumulateCount.Add(buff.affectType, 1);
				if (buff.effect != Defined.EFFECT_TYPE.NONE)
				{
					buff.effectCallBack.Add(BattleBufferManager.Instance.GenerateEffectFromBuffer(buff.effect, m_character.GetTransform().position, -1f, buff.effect_bindTransform));
				}
			}
			switch (buff.affectType)
			{
			case Buff.AffectType.MoveSpeed:
				if (m_buffs.ContainsKey(buff.affectType))
				{
					if (accumulateMaxCount != -1)
					{
						float num5 = m_character.baseAttribute.moveSpeed * buff.GetValue();
						m_buffs[buff.affectType].preValue += num5;
						m_buffs[buff.affectType].Init();
						m_character.MoveSpeed += num5;
					}
					else if (buff.GetValue() > m_buffs[buff.affectType].GetValue())
					{
						RemoveBuff(buff.affectType);
						buff.preValue = m_character.baseAttribute.moveSpeed * buff.GetValue();
						m_character.MoveSpeed += buff.preValue;
					}
					else
					{
						m_buffs[buff.affectType].Init();
					}
				}
				else
				{
					buff.preValue = m_character.baseAttribute.moveSpeed * buff.GetValue();
					m_character.MoveSpeed += buff.preValue;
				}
				break;
			case Buff.AffectType.SkillAddHp:
				BattleBufferManager.Instance.GenerateEffectFromBuffer(Defined.EFFECT_TYPE.EFFECT_ENERGYSHIELD_BEGIN, m_character.GetTransform().position, -1f, m_character.GetTransform());
				break;
			case Buff.AffectType.ReduceDamage:
				if (m_buffs.ContainsKey(buff.affectType))
				{
					if (accumulateMaxCount != -1)
					{
						m_buffs[buff.affectType].Init();
						m_buffs[buff.affectType].preValue += buff.GetValue();
						m_character.reduceDamage += buff.GetValue();
					}
					else if (buff.GetValue() > m_buffs[buff.affectType].GetValue())
					{
						RemoveBuff(buff.affectType);
						buff.preValue = buff.GetValue();
						m_character.reduceDamage += buff.GetValue();
					}
					else
					{
						m_buffs[buff.affectType].Init();
					}
				}
				else
				{
					buff.preValue = buff.GetValue();
					m_character.reduceDamage += buff.GetValue();
				}
				if (m_character.reduceDamage > 0f)
				{
					BattleBufferManager.Instance.GenerateEffectFromBuffer(Defined.EFFECT_TYPE.EFFECT_ENERGYSHIELD_COVER, m_character.m_effectPoint.position, buff.GetTime(), m_character.m_effectPoint);
				}
				break;
			case Buff.AffectType.AddDamage:
				if (m_buffs.ContainsKey(buff.affectType))
				{
					if (accumulateMaxCount != -1)
					{
						m_buffs[buff.affectType].Init();
						m_buffs[buff.affectType].preValue += buff.GetValue();
						m_character.addDamage += buff.GetValue();
					}
					else if (buff.GetValue() > m_buffs[buff.affectType].GetValue())
					{
						RemoveBuff(buff.affectType);
						buff.preValue = buff.GetValue();
						m_character.addDamage += buff.GetValue();
					}
					else
					{
						m_buffs[buff.affectType].Init();
					}
				}
				else
				{
					buff.preValue = buff.GetValue();
					m_character.addDamage += buff.GetValue();
				}
				break;
			case Buff.AffectType.Hp:
				if (m_buffs.ContainsKey(buff.affectType))
				{
					if (accumulateMaxCount != -1)
					{
						m_buffs[buff.affectType].m_value += buff.GetValue();
					}
					else if (buff.GetValue() > m_buffs[buff.affectType].GetValue())
					{
						RemoveBuff(buff.affectType);
					}
					else
					{
						m_buffs[buff.affectType].Init();
					}
				}
				break;
			case Buff.AffectType.Poison:
			case Buff.AffectType.Bleeding:
				if (m_buffs.ContainsKey(buff.affectType))
				{
					if (accumulateMaxCount != -1)
					{
						m_buffs[buff.affectType].m_value += buff.GetValue();
					}
					else if (buff.GetValue() > m_buffs[buff.affectType].GetValue())
					{
						RemoveBuff(buff.affectType);
					}
					else
					{
						m_buffs[buff.affectType].Init();
					}
				}
				else
				{
					m_character.ChangeColor((buff.affectType != Buff.AffectType.Bleeding) ? Util.s_color_posion : Util.s_color_bleeding);
					if (buff.affectType == Buff.AffectType.Bleeding)
					{
						buff.effectCallBack.Add(BattleBufferManager.Instance.GenerateEffectFromBuffer(Defined.EFFECT_TYPE.laceration_DOT, m_character.m_effectPoint.position, -1f, m_character.m_effectPoint));
					}
				}
				break;
			case Buff.AffectType.AddATK:
				if (m_buffs.ContainsKey(buff.affectType))
				{
					if (accumulateMaxCount != -1)
					{
						float num4 = m_character.baseAttribute.damage.left * buff.GetValue();
						m_buffs[buff.affectType].preValue += num4;
						m_buffs[buff.affectType].Init();
						m_character.hitInfo.damage = new NumberSection<float>(m_character.hitInfo.damage.left + num4, m_character.hitInfo.damage.right + num4);
						m_buffs[buff.affectType].Init();
					}
					else if (buff.GetValue() > m_buffs[buff.affectType].GetValue())
					{
						RemoveBuff(buff.affectType);
						buff.preValue = m_character.baseAttribute.damage.left * buff.GetValue();
						m_character.hitInfo.damage = new NumberSection<float>(m_character.hitInfo.damage.left + buff.preValue, m_character.hitInfo.damage.right + buff.preValue);
					}
					else
					{
						m_buffs[buff.affectType].Init();
					}
				}
				else
				{
					buff.preValue = m_character.baseAttribute.damage.left * buff.GetValue();
					m_character.hitInfo.damage = new NumberSection<float>(m_character.hitInfo.damage.left + buff.preValue, m_character.hitInfo.damage.right + buff.preValue);
				}
				break;
			case Buff.AffectType.Exacerbate:
				if (m_buffs.ContainsKey(buff.affectType))
				{
					m_buffs[buff.affectType].Init();
				}
				else
				{
					m_character.ChangeColor(Util.s_color_curse);
				}
				BattleBufferManager.Instance.GenerateEffectFromBuffer(Defined.EFFECT_TYPE.CURSE, m_character.GetTransform().position, -1f, m_character.GetTransform());
				break;
			case Buff.AffectType.Knell:
				if (!m_buffs.ContainsKey(buff.affectType))
				{
					buff.effectCallBack.Add(BattleBufferManager.Instance.GenerateEffectFromBuffer(Defined.EFFECT_TYPE.KNELL, m_character.GetTransform().position, 0f, m_character.GetTransform()));
				}
				break;
			case Buff.AffectType.Confuse:
				if (!m_buffs.ContainsKey(buff.affectType))
				{
					buff.preValue = m_character.hp;
					m_character.hp = (int)buff.GetValue();
					GameBattle.m_instance.AddObjToBetrayList(m_character);
					m_character.GetTransform().localScale *= 1.5f;
					buff.effectCallBack.Add(BattleBufferManager.Instance.GenerateEffectFromBuffer(Defined.EFFECT_TYPE.CONFUSE, m_character.GetTransform().position, -1f, m_character.GetTransform()));
				}
				break;
			case Buff.AffectType.ReduceATK:
				if (m_buffs.ContainsKey(buff.affectType))
				{
					if (accumulateMaxCount != -1)
					{
						float num17 = m_character.baseAttribute.damage.left * buff.GetValue();
						m_buffs[buff.affectType].preValue += num17;
						m_buffs[buff.affectType].Init();
						m_character.hitInfo.damage = new NumberSection<float>(m_character.hitInfo.damage.left - num17, m_character.hitInfo.damage.right - num17);
					}
					else if (buff.GetValue() > m_buffs[buff.affectType].GetValue())
					{
						RemoveBuff(buff.affectType);
						buff.preValue = m_character.baseAttribute.damage.left * buff.GetValue();
						m_character.hitInfo.damage = new NumberSection<float>(m_character.hitInfo.damage.left - buff.preValue, m_character.hitInfo.damage.right - buff.preValue);
					}
					else
					{
						m_buffs[buff.affectType].Init();
					}
				}
				else
				{
					buff.preValue = m_character.baseAttribute.damage.left * buff.GetValue();
					m_character.hitInfo.damage = new NumberSection<float>(m_character.hitInfo.damage.left - buff.preValue, m_character.hitInfo.damage.right - buff.preValue);
					buff.effectCallBack.Add(BattleBufferManager.Instance.GenerateEffectFromBuffer(Defined.EFFECT_TYPE.WEAKNESS, m_character.m_effectPointUpHead.position, -1f, m_character.m_effectPointUpHead));
				}
				break;
			case Buff.AffectType.HitRate:
				if (m_buffs.ContainsKey(buff.affectType))
				{
					if (accumulateMaxCount != -1)
					{
						float num6 = (int)m_character.hitInfo.hitRate;
						m_character.hitRate = (ushort)m_buffs[buff.affectType].CalcValue((int)m_character.hitInfo.hitRate, (int)m_character.hitRate);
						float num7 = (float)(int)m_character.hitRate - num6;
						m_buffs[buff.affectType].preValue += num7;
						m_buffs[buff.affectType].Init();
					}
					else if (buff.GetValue() > m_buffs[buff.affectType].GetValue())
					{
						RemoveBuff(buff.affectType);
						float num8 = (int)m_character.hitInfo.hitRate;
						m_character.hitRate = (ushort)m_buffs[buff.affectType].CalcValue((int)m_character.hitInfo.hitRate, (int)m_character.hitRate);
						float preValue = (float)(int)m_character.hitRate - num8;
						m_buffs[buff.affectType].preValue = preValue;
					}
					else
					{
						m_buffs[buff.affectType].Init();
					}
				}
				else
				{
					m_buffs.Add(buff.affectType, buff);
					float num9 = (int)m_character.hitInfo.hitRate;
					m_character.hitRate = (ushort)m_buffs[buff.affectType].CalcValue((int)m_character.hitInfo.hitRate, (int)m_character.hitRate);
					float preValue2 = (float)(int)m_character.hitRate - num9;
					m_buffs[buff.affectType].preValue = preValue2;
				}
				break;
			case Buff.AffectType.SpecialAttributeHp:
				if (m_buffs.ContainsKey(buff.affectType))
				{
					if (accumulateMaxCount != -1)
					{
						m_buffs[buff.affectType].m_value += buff.GetValue();
					}
					else if (buff.GetValue() > m_buffs[buff.affectType].GetValue())
					{
						RemoveBuff(buff.affectType);
					}
					else
					{
						m_buffs[buff.affectType].Init();
					}
				}
				break;
			case Buff.AffectType.Miasma:
				if (null == BattleBufferManager.Instance.m_miasmaUI)
				{
					BattleBufferManager.Instance.m_miasmaUI = Object.Instantiate(Resources.Load("Game/MiasmaUI")) as GameObject;
					BattleBufferManager.Instance.m_miasmaCoverOn = BattleBufferManager.Instance.m_miasmaUI.transform.Find("Camera/CoverOn").gameObject.GetComponentInChildren<EffectControl>();
					BattleBufferManager.Instance.m_miasmaCoverEnd = BattleBufferManager.Instance.m_miasmaUI.transform.Find("Camera/CoverEnd").gameObject.GetComponentInChildren<EffectControl>();
					BattleBufferManager.Instance.m_miasmaCoverOn.Root.SetActive(true);
					BattleBufferManager.Instance.m_miasmaCoverEnd.gameObject.SetActive(false);
				}
				else
				{
					BattleBufferManager.Instance.m_miasmaUI.SetActive(true);
					BattleBufferManager.Instance.m_miasmaCoverOn.Root.SetActive(true);
					BattleBufferManager.Instance.m_miasmaCoverEnd.gameObject.SetActive(false);
				}
				break;
			case Buff.AffectType.Defense:
			{
				if (m_character.objectType != 0)
				{
					return;
				}
				Player player = (Player)m_character;
				if (m_buffs.ContainsKey(buff.affectType))
				{
					if (accumulateMaxCount != -1)
					{
						m_buffs[buff.affectType].Init();
						float num18 = buff.CalcValue(player.m_def, player.m_def);
						float num19 = num18 - player.m_def;
						m_buffs[buff.affectType].preValue += num19;
						player.m_def += num19;
					}
					else if (Mathf.Abs(buff.GetValue()) > Mathf.Abs(m_buffs[buff.affectType].GetValue()))
					{
						RemoveBuff(buff.affectType);
						float num20 = buff.CalcValue(player.m_def, player.m_def);
						float num22 = (buff.preValue = num20 - player.m_def);
						player.m_def += num22;
					}
					else
					{
						m_buffs[buff.affectType].Init();
					}
				}
				else
				{
					float num23 = buff.CalcValue(player.m_def, player.m_def);
					float num25 = (buff.preValue = num23 - player.m_def);
					player.m_def += num25;
				}
				break;
			}
			case Buff.AffectType.ShootRange:
				if (m_character.m_weapon == null)
				{
					return;
				}
				if (m_buffs.ContainsKey(buff.affectType))
				{
					if (accumulateMaxCount != -1)
					{
						m_buffs[buff.affectType].Init();
						float num10 = buff.CalcValue(m_character.shootRange, m_character.shootRange);
						float num11 = num10 - m_character.shootRange;
						m_buffs[buff.affectType].preValue += num11;
						m_character.shootRange += num11;
					}
					else if (Mathf.Abs(buff.GetValue()) > Mathf.Abs(m_buffs[buff.affectType].GetValue()))
					{
						RemoveBuff(buff.affectType);
						float num12 = buff.CalcValue(m_character.shootRange, m_character.shootRange);
						float num13 = num12 - m_character.shootRange;
						m_buffs[buff.affectType].preValue += num13;
						m_character.shootRange += num13;
					}
					else
					{
						m_buffs[buff.affectType].Init();
					}
				}
				else
				{
					float num14 = buff.CalcValue(m_character.shootRange, m_character.shootRange);
					float num16 = (buff.preValue = num14 - m_character.shootRange);
					m_character.shootRange += num16;
				}
				break;
			case Buff.AffectType.AtkPrequency:
				if (m_buffs.ContainsKey(buff.affectType))
				{
					if (accumulateMaxCount != -1)
					{
						float num3 = m_character.baseAttribute.attackFrequence * buff.GetValue();
						m_buffs[buff.affectType].preValue += num3;
						m_buffs[buff.affectType].Init();
						m_character.AtkFrequency += num3;
					}
					else if (buff.GetValue() > m_buffs[buff.affectType].GetValue())
					{
						RemoveBuff(buff.affectType);
						buff.preValue = m_character.baseAttribute.attackFrequence * buff.GetValue();
						m_character.AtkFrequency += buff.preValue;
					}
					else
					{
						m_buffs[buff.affectType].Init();
					}
				}
				else
				{
					buff.preValue = m_character.baseAttribute.attackFrequence * buff.GetValue();
					m_character.AtkFrequency += buff.preValue;
				}
				break;
			}
			if (!m_buffs.ContainsKey(buff.affectType))
			{
				m_buffs.Add(buff.affectType, buff);
			}
		}

		public bool HasBuff(Buff.AffectType affectType)
		{
			return m_buffs.ContainsKey(affectType);
		}

		public Buff GetBuff(Buff.AffectType affectType)
		{
			return m_buffs[affectType];
		}

		private void AddBuffToRemoveList(Buff.AffectType affectType)
		{
			if (m_removeBuffs == null)
			{
				m_removeBuffs = new List<Buff.AffectType>();
			}
			if (!m_removeBuffs.Contains(affectType))
			{
				m_removeBuffs.Add(affectType);
			}
		}

		public void RemoveBuff(Buff.AffectType affectType)
		{
			if (!m_buffs.ContainsKey(affectType))
			{
				return;
			}
			foreach (GameObject item in m_buffs[affectType].effectCallBack)
			{
				if (item != null)
				{
					item.transform.parent = BattleBufferManager.s_effectObjectRoot.transform;
					item.SetActive(false);
				}
			}
			m_buffs[affectType].effectCallBack.Clear();
			switch (affectType)
			{
			case Buff.AffectType.MoveSpeed:
				m_character.MoveSpeed -= m_buffs[affectType].preValue;
				break;
			case Buff.AffectType.SkillAddHp:
				BattleBufferManager.Instance.GenerateEffectFromBuffer(Defined.EFFECT_TYPE.EFFECT_ENERGYSHIELD_BEGIN, m_character.GetTransform().position, -1f, m_character.GetTransform());
				break;
			case Buff.AffectType.ReduceDamage:
				m_character.reduceDamage -= m_buffs[affectType].preValue;
				break;
			case Buff.AffectType.AddDamage:
				m_character.addDamage -= m_buffs[affectType].preValue;
				break;
			case Buff.AffectType.Poison:
			case Buff.AffectType.Bleeding:
				m_character.ResetColor();
				break;
			case Buff.AffectType.AddATK:
				m_character.hitInfo.damage = new NumberSection<float>(m_character.hitInfo.damage.left - m_buffs[affectType].preValue, m_character.hitInfo.damage.right - m_buffs[affectType].preValue);
				break;
			case Buff.AffectType.Exacerbate:
				m_character.ResetColor();
				break;
			case Buff.AffectType.Confuse:
				if (m_buffs[affectType].creater.objectType == Defined.OBJECT_TYPE.OBJECT_TYPE_ENEMY)
				{
					Enemy enemy = (Enemy)m_buffs[affectType].creater;
					if (enemy.Alive())
					{
						enemy.elegyTarget = null;
					}
				}
				m_character.hp = (int)m_buffs[affectType].preValue;
				GameBattle.m_instance.AddObjToBetrayList(m_character);
				m_character.GetTransform().localScale /= 1.5f;
				break;
			case Buff.AffectType.ReduceATK:
				m_character.hitInfo.damage = new NumberSection<float>(m_character.hitInfo.damage.left + m_buffs[affectType].preValue, m_character.hitInfo.damage.right + m_buffs[affectType].preValue);
				break;
			case Buff.AffectType.HitRate:
				m_character.hitRate = (ushort)((float)(int)m_character.hitRate - m_buffs[affectType].preValue);
				break;
			case Buff.AffectType.Miasma:
				if (null != BattleBufferManager.Instance.m_miasmaUI)
				{
					BattleBufferManager.Instance.m_miasmaCoverOn.Root.SetActive(false);
					BattleBufferManager.Instance.m_miasmaCoverEnd.gameObject.SetActive(true);
				}
				break;
			case Buff.AffectType.Defense:
			{
				Player player = (Player)m_character;
				player.m_def -= m_buffs[affectType].preValue;
				break;
			}
			case Buff.AffectType.ShootRange:
				m_character.shootRange -= m_buffs[affectType].preValue;
				break;
			case Buff.AffectType.AtkPrequency:
				m_character.AtkFrequency -= m_buffs[affectType].preValue;
				break;
			}
			m_buffs.Remove(affectType);
			if (m_buffAccumulateCount.ContainsKey(affectType))
			{
				m_buffAccumulateCount.Remove(affectType);
			}
		}

		public void RemoveAllBuffs()
		{
			foreach (Buff.AffectType key in m_buffs.Keys)
			{
				AddBuffToRemoveList(key);
			}
			foreach (Buff.AffectType removeBuff in m_removeBuffs)
			{
				RemoveBuff(removeBuff);
			}
			m_buffs.Clear();
			m_removeBuffs.Clear();
		}

		public void UpdateBuffs()
		{
			foreach (Buff value in m_buffs.Values)
			{
				if (value.isMilitate())
				{
					switch (value.affectType)
					{
					case Buff.AffectType.Hp:
					{
						int hp2 = m_character.hp;
						int num3 = (int)value.CalcValue(m_character.hp, m_character.hpMax);
						int num4 = num3 - hp2;
						m_character.hp = num3;
						if (!m_character.Alive())
						{
							m_character.OnDeath();
							return;
						}
						m_character.OnHurt(true);
						if (EffectNumManager.instance != null)
						{
							EffectNumManager.instance.GenerageEffectNum(EffectNumber.EffectNumType.Damge, num4, m_character.m_effectPoint.position);
						}
						break;
					}
					case Buff.AffectType.SkillAddHp:
					{
						int hp3 = m_character.hp;
						int num5 = (int)value.CalcValue(m_character.hp, m_character.hpMax);
						int num6 = num5 - hp3;
						m_character.hp = num5;
						if (EffectNumManager.instance != null)
						{
							EffectNumManager.instance.GenerageEffectNum(EffectNumber.EffectNumType.Heal, num6, m_character.m_effectPoint.position);
						}
						BattleBufferManager.Instance.GenerateEffectFromBuffer(Defined.EFFECT_TYPE.EFFECT_ADD_HP, m_character.GetTransform().position, 0f, m_character.GetTransform());
						break;
					}
					case Buff.AffectType.Poison:
					case Buff.AffectType.Bleeding:
					{
						int hp4 = m_character.hp;
						int num7 = (int)value.CalcValue(m_character.hp, m_character.hpMax);
						int num8 = num7 - hp4;
						m_character.hp = num7;
						if (!m_character.Alive())
						{
							m_character.OnDeath();
							return;
						}
						m_character.OnHurt(true);
						if (EffectNumManager.instance != null)
						{
							EffectNumManager.instance.GenerageEffectNum(EffectNumber.EffectNumType.Damge, num8, m_character.m_effectPoint.position);
						}
						BattleBufferManager.Instance.GenerateEffectFromBuffer(Defined.EFFECT_TYPE.POISON, m_character.GetTransform().position, -1f, m_character.GetTransform());
						break;
					}
					case Buff.AffectType.Knell:
						if (!value.creater.Alive())
						{
							AddBuffToRemoveList(value.affectType);
						}
						else if (value.isOver())
						{
							m_character.hp = 0;
							m_character.OnDeath();
							return;
						}
						break;
					case Buff.AffectType.SpecialAttributeHp:
					{
						int hp = m_character.hp;
						int num = (int)value.CalcValue(m_character.hp, m_character.hpMax);
						int num2 = num - hp;
						m_character.hp = num;
						if (num2 > 0)
						{
							if (EffectNumManager.instance != null)
							{
								EffectNumManager.instance.GenerageEffectNum(EffectNumber.EffectNumType.Heal, num2, m_character.m_effectPoint.position);
							}
							break;
						}
						if (!m_character.Alive())
						{
							m_character.OnDeath();
							return;
						}
						m_character.OnHurt(true);
						if (EffectNumManager.instance != null)
						{
							EffectNumManager.instance.GenerageEffectNum(EffectNumber.EffectNumType.Damge, num2, m_character.m_effectPoint.position);
						}
						break;
					}
					}
				}
				if (value.isOver())
				{
					AddBuffToRemoveList(value.affectType);
				}
			}
			foreach (Buff.AffectType removeBuff in m_removeBuffs)
			{
				RemoveBuff(removeBuff);
			}
			m_removeBuffs.Clear();
		}
	}
}

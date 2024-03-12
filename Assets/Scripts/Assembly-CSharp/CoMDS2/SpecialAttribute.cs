using System.Collections.Generic;

namespace CoMDS2
{
	public class SpecialAttribute
	{
		public enum AttributeType
		{
			Random = 0,
			Reinforcement = 1,
			Mania = 2,
			Rapid = 3,
			Dodge = 4,
			Decay = 5,
			Harden = 6,
			Rifrigerate = 7,
			Electric = 8,
			Variation = 9,
			Hellfire = 10,
			Strengthen = 11,
			Endure = 12,
			DeadSpawnBomb = 13,
			Bloodsucking = 14,
			RangeWeaken = 15,
			Territory = 16,
			StrickBack = 17,
			InvalidSkill = 18,
			PoisonClaw = 19,
			Exacerbate = 20,
			Knell = 21,
			Elegy = 22,
			Vampire = 23,
			Grind = 24,
			Split = 25,
			Tornado = 26,
			God = 27,
			PusBlood = 28,
			Bomb = 29,
			LiquidNitrogen = 30,
			Summon = 31,
			LifeLink = 32,
			Count = 33
		}

		public enum SpecialAttributeEffectType
		{
			Strong = 0,
			Manic = 1,
			Rapid = 2,
			Avoid = 3,
			Monsters_01 = 4,
			Retain1 = 5,
			Monsters_02 = 6,
			Burst_D_01 = 7,
			Leakage = 8,
			Health_Bomb = 9,
			Monsters_12_01 = 10,
			Monsters_Vampire_0 = 11,
			Tornado = 12,
			SporePusBlood = 13,
			SporeBomb = 14,
			SporeLiquidNitrogen = 15,
			Relentless = 16,
			LifeLink = 17,
			Monsters_14_01 = 18,
			Monsters_16 = 19,
			Monsters_17 = 20,
			Monsters_18 = 21,
			Monsters_19 = 22,
			Small_halo_B = 23,
			Small_halo_G = 24,
			Small_halo_P = 25,
			Small_halo_Y = 26,
			Monsters_04 = 27,
			HellFirePower = 28
		}

		public enum SpecialAttribteTriggerType
		{
			Once = 0,
			Always = 1,
			BeHit = 2,
			Attack = 3
		}

		public class SpecialAttributeGameData
		{
			public AttributeType type;

			public Character creater;

			public int uudi;

			public SpecialAttributeGameData(AttributeType type, Character creater)
			{
				this.type = type;
				this.creater = creater;
			}
		}

		public class SpecialAttributeEffectData
		{
			public SpecialAttributeEffectType type;

			public int bufferCount;
		}

		public class SpecialAttributeData
		{
			public AttributeType type;

			public string name;

			public string description;

			public List<SpecialAttributeEffectData> effects;
		}

		public class EnemySpecialAttributeData : SpecialAttributeData
		{
		}

		public class SpecialAttributeReinforcement : SpecialAttributeData
		{
			public float damage;
		}

		public class SpecialAttributeMania : SpecialAttributeData
		{
			public ushort proCrit;

			public float critDamage;
		}

		public class SpecialAttributeRapid : SpecialAttributeData
		{
			public float attFrequency;

			public float addMoveSpeedPercent;
		}

		public class SpecialAttributeDodge : SpecialAttributeData
		{
			public ushort proDodge;
		}

		public class SpecialAttributeDecay : SpecialAttributeData
		{
			public float reduceHpPercent;

			public float frequency;
		}

		public class SpecialAttributeHarden : SpecialAttributeData
		{
			public float reduceDamage;
		}

		public class SpecialAttributeRifrigerate : SpecialAttributeData
		{
			public float reduceSpeed;

			public float frozenTime;

			public ushort probability;
		}

		public class SpecialAttributeElectric : SpecialAttributeData
		{
			public ushort probability;

			public float reduceDamage;
		}

		public class SpecialAttributeVariation : SpecialAttributeData
		{
			public float damage;

			public float attFrequency;

			public float addMoveSpeedPercent;
		}

		public class SpecialAttributeHellfire : SpecialAttributeData
		{
			public float frequency;
		}

		public class SpecialAttributeStrengthen : SpecialAttributeData
		{
			public float hpMax;
		}

		public class SpecialAttributeEndure : SpecialAttributeData
		{
			public ushort probability;

			public float duration;
		}

		public class SpecialAttributeDeadSpawnBomb : SpecialAttributeData
		{
			public ushort probability;
		}

		public class SpecialAttributeBloodsucking : SpecialAttributeData
		{
			public float addSpeedPercent;

			public float damage;

			public float duration;

			public ushort accumulate;
		}

		public class SpecialAttributeRangeWeaken : SpecialAttributeData
		{
			public float reduceDamage;
		}

		public class SpecialAttributeRangeTerritory : SpecialAttributeData
		{
			public float critDamage;

			public float normalDamage;
		}

		public class SpecialAttributeStrickBack : SpecialAttributeData
		{
			public float bombDamage;

			public float frozenTime;

			public ushort probability;
		}

		public class SpecialAttributeInvalidSkill : SpecialAttributeData
		{
		}

		public class SpecialAttributePoisonClaw : SpecialAttributeData
		{
			public ushort probability;

			public float duration;

			public NumberSection<float> damage;
		}

		public class SpecialAttributeExacerbate : SpecialAttributeData
		{
			public ushort probability;

			public float duration;

			public NumberSection<float> damage;
		}

		public class SpecialAttributeKnell : SpecialAttributeData
		{
			public ushort probability;
		}

		public class SpecialAttributeElegy : SpecialAttributeData
		{
			public float frequency;

			public float hp;
		}

		public class SpecialAttributeVampire : SpecialAttributeData
		{
		}

		public class SpecialAttributeGrind : SpecialAttributeData
		{
			public float reduceAtt;

			public float reduceMoveSpeed;

			public float duration;
		}

		public class SpecialAttributeSplit : SpecialAttributeData
		{
			public float hp;

			public float newHp;
		}

		public class SpecialAttributeTornado : SpecialAttributeData
		{
			public float attFrequency;

			public NumberSection<float> damage;
		}

		public class SpecialAttributeGod : SpecialAttributeData
		{
			public float frequency;

			public float duration;
		}

		public class SpecialAttributePusBlood : SpecialAttributeData
		{
			public float frequency;

			public float damage;

			public float poisonDuration;

			public float poisonFrequency;
		}

		public class SpecialAttributeBomb : SpecialAttributeData
		{
			public float frequency;

			public float damage;
		}

		public class SpecialAttributeLiquidNitrogen : SpecialAttributeData
		{
			public float frequency;

			public float damage;

			public float frozenTime;
		}

		public class SpecialAttributeSummon : SpecialAttributeData
		{
			public float frequency;

			public ushort zombie;

			public ushort zombieBomb;

			public float addSpeedPercent;
		}

		public class SpecialAttributeLifeLink : SpecialAttributeData
		{
		}

		private static int s_uudiCounter;

		public static Dictionary<int, SpecialAttributeGameData> s_enemySpecailAttributesCentre;

		public static void Init()
		{
			s_enemySpecailAttributesCentre = null;
			s_enemySpecailAttributesCentre = new Dictionary<int, SpecialAttributeGameData>();
			s_uudiCounter = 0;
		}

		public static void Refresh()
		{
		}

		public static int AddEnemySpecailAttribute(SpecialAttributeGameData attrData)
		{
			if (s_enemySpecailAttributesCentre == null)
			{
				s_enemySpecailAttributesCentre = new Dictionary<int, SpecialAttributeGameData>();
				s_uudiCounter = 0;
			}
			attrData.uudi = s_uudiCounter;
			s_uudiCounter++;
			s_enemySpecailAttributesCentre.Add(attrData.uudi, attrData);
			return attrData.uudi;
		}

		public static void RemoveEnemySpecialAttribute(int uuid)
		{
			if (s_enemySpecailAttributesCentre.ContainsKey(uuid))
			{
				s_enemySpecailAttributesCentre.Remove(uuid);
			}
		}

		public static int ApplyUUID()
		{
			return ++s_uudiCounter;
		}
	}
}

using System.Collections.Generic;

namespace CoMDS2
{
	public class TeamSpecialAttribute
	{
		public enum TeamAttributeType
		{
			Reinforcement = 0,
			UrgentTreatment = 1,
			SpecialTraining = 2,
			Armsmaster = 3,
			Biobomb = 4,
			LinkedLives = 5,
			LivingCells = 6,
			Mania = 7,
			VolatileBomb = 8,
			ForceField = 9,
			Prayer = 10,
			ProlongedFirepower = 11,
			Nanotech = 12,
			SealingTechnique = 13,
			Stun = 14,
			Freeze = 15,
			Splash = 16,
			ShadowWalk = 17,
			AimedShot = 18,
			Resolve = 19,
			Resurrection = 20,
			RelentlessRage = 21,
			Vampire = 22,
			Cooling = 23,
			Executioner = 24
		}

		public enum TeamAttributeEvolveType
		{
			Broken = 0,
			Weak = 1,
			MagneticField = 2,
			EndBattle = 3,
			StrikeBack = 4,
			Stealth = 5,
			Shortsightedness = 6,
			Thorns = 7,
			Dumdum = 8,
			Blessed = 9
		}

		public class TeamAttributeData
		{
			public TeamAttributeType type;

			public string name;

			public string iconFileName;

			public string description;

			public List<SpecialAttribute.SpecialAttributeEffectData> effects;
		}

		public class TeamAttributeReinforcement : TeamAttributeData
		{
			public float[] damage;
		}

		public class TeamAttributeUrgentTreatment : TeamAttributeData
		{
			public float frequency;

			public float[] hpPercent;
		}

		public class TeamAttributeSpecialTraining : TeamAttributeData
		{
			public float[] hpMaxPercent;
		}

		public class TeamAttributeArmsmaster : TeamAttributeData
		{
			public float[] clipPercent;

			public float[] cdPercent;
		}

		public class TeamAttributeBiobomb : TeamAttributeData
		{
			public ushort[] proBomb;
		}

		public class TeamAttributeLinkedLives : TeamAttributeData
		{
			public ushort[] proShareDamage;
		}

		public class TeamAttributeLivingCells : TeamAttributeData
		{
			public ushort[] proResHp;

			public float resHpPercent;
		}

		public class TeamAttributeMania : TeamAttributeData
		{
			public ushort[] proCrit;

			public float[] critDamagePercent;
		}

		public class TeamAttributeVolatileBomb : TeamAttributeData
		{
			public ushort[] proBomb;

			public float[] stunTime;
		}

		public class TeamAttributeForceField : TeamAttributeData
		{
			public ushort[] proReduceDamage;

			public float reduceDamagePercent;
		}

		public class TeamAttributePrayer : TeamAttributeData
		{
			public float[] reduceDamagePercent;
		}

		public class TeamAttributeProlongedFirepower : TeamAttributeData
		{
			public float[] skillEmplacementTimePercent;
		}

		public class TeamAttributeNanotech : TeamAttributeData
		{
			public float[] reduceDebuffPercent;
		}

		public class TeamAttributeSealingTechnique : TeamAttributeData
		{
			public ushort[] proCd;
		}

		public class TeamAttributeStun : TeamAttributeData
		{
			public ushort[] proStun;

			public float stunTime;
		}

		public class TeamAttributeFreeze : TeamAttributeData
		{
			public ushort[] proReduceMoveSpeedAndAttack;

			public float reduceMoveSpeedAndAttackPercent;

			public float time;
		}

		public class TeamAttributeAimedShot : TeamAttributeData
		{
			public ushort[] proIgnoreDefence;
		}

		public class TeamAttributeShadowWalk : TeamAttributeData
		{
			public ushort[] proDodge;
		}

		public class TeamAttributeSplash : TeamAttributeData
		{
			public ushort[] probability;
		}

		public class TeamAttributeResolve : TeamAttributeData
		{
			public float hpReducePercent;

			public float[] attackPercent;
		}

		public class TeamAttributeResurrection : TeamAttributeData
		{
			public ushort[] proAvoidDead;

			public float resHpPercent;
		}

		public class TeamAttributeRelentlessRage : TeamAttributeData
		{
			public float[] attackPercent;

			public float[] proCrit;
		}

		public class TeamAttributeVampire : TeamAttributeData
		{
			public ushort[] proResHp;

			public float resHpPercent;
		}

		public class TeamAttributeCooling : TeamAttributeData
		{
			public ushort[] proNoCd;
		}

		public class TeamAttributeExecutioner : TeamAttributeData
		{
			public float[] probability;

			public int critTimes;
		}

		public class TeamAttributeEvolveData
		{
			public TeamAttributeEvolveType type;

			public string name;

			public string iconFileName;

			public float[] probability;

			public float[] percent;

			public float time;

			public string description;

			public List<SpecialAttribute.SpecialAttributeEffectData> effects;
		}
	}
}

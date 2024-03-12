using System.Collections.Generic;
using UnityEngine;

namespace CoMDS2
{
	public class HitInfo
	{
		public NumberSection<float> damage;

		public float repelTime;

		public NumberSection<float> repelDistance;

		public Vector3 hitPoint;

		public Vector3 repelDirection;

		public DS2ActiveObject source;

		public float percentDamage;

		public NumberSection<float> deadRepelDistance;

		public DeadSpawnInfo deadSpawnInfo;

		public DeadSpawnInfo hitSpawnInfo;

		public List<Buff> buffs;

		public float critRate;

		public float critDamage;

		public ushort hitRate;

		public ushort stabRate;

		public bool bSkill;

		public bool bViolent;

		public Defined.EFFECT_TYPE hitEffect;

		public Dictionary<Defined.SPECIAL_HIT_TYPE, SpecialHitInfo> specialHit;

		public int hitStrength;

		public HitInfo()
		{
			damage = new NumberSection<float>(0f, 0f);
			percentDamage = 0f;
			repelTime = 0.2f;
			repelDistance = new NumberSection<float>(0f, 0f);
			repelDirection = default(Vector3);
			deadRepelDistance = null;
			specialHit = new Dictionary<Defined.SPECIAL_HIT_TYPE, SpecialHitInfo>();
			deadSpawnInfo = null;
			hitSpawnInfo = null;
			buffs = new List<Buff>();
			buffs.Clear();
			critRate = 0f;
			critDamage = 0f;
			hitRate = 100;
			stabRate = 0;
			bSkill = false;
			bViolent = false;
			hitEffect = Defined.EFFECT_TYPE.NONE;
			hitStrength = 1;
		}

		public HitInfo(HitInfo hitInfo)
		{
			damage = hitInfo.damage;
			repelTime = hitInfo.repelTime;
			repelDistance = hitInfo.repelDistance;
			hitPoint = hitInfo.hitPoint;
			repelDirection = hitInfo.repelDirection;
			source = hitInfo.source;
			percentDamage = hitInfo.percentDamage;
			deadRepelDistance = hitInfo.deadRepelDistance;
			deadSpawnInfo = hitInfo.deadSpawnInfo;
			hitSpawnInfo = hitInfo.hitSpawnInfo;
			buffs = hitInfo.buffs;
			critRate = hitInfo.critRate;
			critDamage = hitInfo.critDamage;
			hitRate = hitInfo.hitRate;
			stabRate = hitInfo.stabRate;
			bSkill = hitInfo.bSkill;
			hitEffect = hitInfo.hitEffect;
			bViolent = hitInfo.bViolent;
			specialHit = new Dictionary<Defined.SPECIAL_HIT_TYPE, SpecialHitInfo>();
			foreach (Defined.SPECIAL_HIT_TYPE key in hitInfo.specialHit.Keys)
			{
				specialHit.Add(key, hitInfo.specialHit[key]);
			}
			hitStrength = hitInfo.hitStrength;
		}

		public void Copy(HitInfo hitInfo)
		{
			damage = hitInfo.damage;
			repelTime = hitInfo.repelTime;
			repelDistance = hitInfo.repelDistance;
			hitPoint = hitInfo.hitPoint;
			repelDirection = hitInfo.repelDirection;
			source = hitInfo.source;
			percentDamage = hitInfo.percentDamage;
			deadRepelDistance = hitInfo.deadRepelDistance;
			deadSpawnInfo = hitInfo.deadSpawnInfo;
			hitSpawnInfo = hitInfo.hitSpawnInfo;
			buffs = hitInfo.buffs;
			critRate = hitInfo.critRate;
			critDamage = hitInfo.critDamage;
			hitRate = hitInfo.hitRate;
			stabRate = hitInfo.stabRate;
			bSkill = hitInfo.bSkill;
			hitEffect = hitInfo.hitEffect;
			bViolent = hitInfo.bViolent;
			specialHit = new Dictionary<Defined.SPECIAL_HIT_TYPE, SpecialHitInfo>();
			foreach (Defined.SPECIAL_HIT_TYPE key in hitInfo.specialHit.Keys)
			{
				specialHit.Add(key, hitInfo.specialHit[key]);
			}
			hitStrength = hitInfo.hitStrength;
		}

		public void AddSpecialHit(Defined.SPECIAL_HIT_TYPE type, SpecialHitInfo specialHitInfo)
		{
			if (specialHit.ContainsKey(type))
			{
				if (specialHit[type].disposable)
				{
					specialHit[type] = specialHitInfo;
				}
			}
			else
			{
				specialHit.Add(type, specialHitInfo);
			}
		}
	}
}

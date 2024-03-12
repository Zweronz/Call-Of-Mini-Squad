using System.Collections.Generic;
using UnityEngine;

namespace CoMDS2
{
	public class SporeExplode : DS2ActiveObject
	{
		private HitInfo m_hitInfo;

		private DS2ActiveObject m_bulletHitObj;

		private SporeExplodeCollider m_colliderX;

		private SporeExplodeCollider m_colliderZ;

		private AIState.AIPhase m_explodePhase;

		private float m_explodeTime;

		private List<DS2ActiveObject> m_explodeHitList = new List<DS2ActiveObject>();

		public SporeExplode()
		{
			base.objectType = Defined.OBJECT_TYPE.OBJECT_TYPE_OTHERS;
		}

		public override void Initialize(GameObject prefab, string name, Vector3 position, Quaternion rotation, int layer)
		{
			base.Initialize(prefab, name, position, rotation, layer);
			base.clique = Clique.Computer;
			SporeExplodeCollider[] componentsInChildren = GetTransform().GetComponentsInChildren<SporeExplodeCollider>();
			SporeExplodeCollider[] array = componentsInChildren;
			foreach (SporeExplodeCollider sporeExplodeCollider in array)
			{
				if (sporeExplodeCollider.type == SporeExplodeCollider.ColliderType.X)
				{
					m_colliderX = sporeExplodeCollider;
				}
				else if (sporeExplodeCollider.type == SporeExplodeCollider.ColliderType.Z)
				{
					m_colliderZ = sporeExplodeCollider;
				}
				sporeExplodeCollider.sporeExplode = this;
				sporeExplodeCollider.gameObject.SetActive(false);
			}
		}

		public void Explode(bool cross_explode, HitInfo hitInfo, Vector3 position, DS2ActiveObject bullet_hit_obj)
		{
			m_explodeHitList.Clear();
			m_hitInfo = new HitInfo(hitInfo);
			m_hitInfo.repelTime = 0f;
			m_hitInfo.repelDistance = new NumberSection<float>(0f, 0f);
			m_bulletHitObj = bullet_hit_obj;
			GetTransform().position = position;
			GetTransform().rotation = Quaternion.AngleAxis(Random.Range(0f, 360f), Vector3.up);
			GetGameObject().SetActive(true);
			if (cross_explode)
			{
				effectPlayManager.PlayEffect("Attack_02");
				m_colliderX.gameObject.SetActive(true);
				m_colliderZ.gameObject.SetActive(true);
			}
			else
			{
				effectPlayManager.PlayEffect("Attack");
				m_colliderZ.gameObject.SetActive(true);
			}
			m_explodePhase = AIState.AIPhase.Enter;
			m_explodeTime = 0f;
			GameBattle.m_instance.AddObjToInteractObjectList(this);
		}

		public override void Update(float deltaTime)
		{
			if (!GetGameObject().activeInHierarchy)
			{
				return;
			}
			m_explodeTime += deltaTime;
			switch (m_explodePhase)
			{
			case AIState.AIPhase.Enter:
				if (!(m_explodeTime > 0.15f))
				{
					break;
				}
				m_colliderX.gameObject.SetActive(false);
				m_colliderZ.gameObject.SetActive(false);
				foreach (DS2ActiveObject explodeHit in m_explodeHitList)
				{
					IFighter fighter = explodeHit.GetFighter();
					if (fighter != null && fighter.OnHit(m_hitInfo).isHit && base.hitInfo.hitEffect != Defined.EFFECT_TYPE.NONE && explodeHit.m_effectPoint != null)
					{
						BattleBufferManager.Instance.GenerateEffectFromBuffer(m_hitInfo.hitEffect, explodeHit.m_effectPoint.position);
					}
				}
				m_explodePhase = AIState.AIPhase.Update;
				break;
			case AIState.AIPhase.Update:
				if (m_explodeTime > 2f)
				{
					m_explodePhase = AIState.AIPhase.Exit;
					effectPlayManager.StopEffect("Attack");
					effectPlayManager.StopEffect("Attack_02");
				}
				break;
			case AIState.AIPhase.Exit:
				m_explodeHitList.Clear();
				m_bulletHitObj = null;
				GetGameObject().SetActive(false);
				GameBattle.m_instance.AddToInteractObjectNeedDeleteList(this);
				break;
			}
		}

		public void AddHitObject(DS2ActiveObject obj)
		{
			if (obj != m_bulletHitObj && !m_explodeHitList.Contains(obj))
			{
				m_explodeHitList.Add(obj);
			}
		}
	}
}

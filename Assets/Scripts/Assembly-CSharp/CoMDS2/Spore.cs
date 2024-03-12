using UnityEngine;

namespace CoMDS2
{
	public class Spore : DS2ActiveObject
	{
		public enum SporeType
		{
			PusBlood = 0,
			Bomb = 1,
			LiquidNitrogen = 2
		}

		private SporeType m_type;

		private float m_timer;

		private float m_time;

		private Character m_creator;

		private Spore()
		{
		}

		public Spore(SporeType type)
		{
			m_type = type;
			base.objectType = Defined.OBJECT_TYPE.OBJECT_TYPE_OTHERS;
		}

		public override void Initialize(GameObject prefab, string name, Vector3 position, Quaternion rotation, int layer)
		{
			base.Initialize(prefab, name, position, rotation, layer);
			GetTransform().parent = BattleBufferManager.s_activeObjectRoot.transform;
			if (m_type == SporeType.Bomb)
			{
				base.hitInfo.repelDistance = new NumberSection<float>(2f, 4f);
			}
			else if (m_type == SporeType.LiquidNitrogen)
			{
				SpecialHitInfo specialHitInfo = new SpecialHitInfo();
				specialHitInfo.time = DataCenter.Conf().m_specialAttributeLiquidNitrogen.frozenTime;
				specialHitInfo.disposable = false;
				base.hitInfo.AddSpecialHit(Defined.SPECIAL_HIT_TYPE.FROZEN, specialHitInfo);
			}
			AIState aIState = new AIState(this, "Idle");
			aIState.SetCustomFunc(OnIdle);
			AddAIState(aIState.name, aIState);
			SetDefaultAIState(aIState);
			SwitchFSM(aIState);
		}

		public void SetDamage(float damage)
		{
			base.hitInfo.damage = new NumberSection<float>(damage, damage);
		}

		protected override void LoadResources()
		{
			base.LoadResources();
			switch (m_type)
			{
			case SporeType.Bomb:
				BattleBufferManager.Instance.CreateEffectBufferByType(Defined.EFFECT_TYPE.EFFECT_BOMB_1, 1);
				break;
			case SporeType.LiquidNitrogen:
				BattleBufferManager.Instance.CreateEffectBufferByType(Defined.EFFECT_TYPE.BURST_D_01, 1);
				break;
			case SporeType.PusBlood:
				BattleBufferManager.Instance.CreateEffectBufferByType(Defined.EFFECT_TYPE.BURST_B_01, 1);
				break;
			}
		}

		public void SetActive(bool active, Character creater)
		{
			if (active)
			{
				m_creator = creater;
				GetGameObject().SetActive(active);
				base.clique = m_creator.clique;
				SwitchFSM(GetAIState("Idle"));
			}
			else
			{
				GetGameObject().SetActive(active);
			}
		}

		public override HitResultInfo OnHit(HitInfo hitInfo)
		{
			HitResultInfo hitResultInfo = new HitResultInfo();
			hitResultInfo.isHit = true;
			return hitResultInfo;
		}

		public void OnIdle(AIState.AIPhase phase)
		{
			switch (phase)
			{
			case AIState.AIPhase.Enter:
				m_timer = 0f;
				break;
			case AIState.AIPhase.Update:
				m_timer += Time.deltaTime;
				if (m_timer >= 5f)
				{
					DoBomb();
					Destroy();
				}
				break;
			}
		}

		public void OnBomb(AIState.AIPhase phase)
		{
			switch (phase)
			{
			case AIState.AIPhase.Enter:
				m_timer = 0f;
				break;
			case AIState.AIPhase.Update:
				m_timer += Time.deltaTime;
				if (!(m_timer >= 3f))
				{
				}
				break;
			}
		}

		public void DoBomb()
		{
			switch (m_type)
			{
			case SporeType.Bomb:
				BattleBufferManager.Instance.GenerateEffectFromBuffer(Defined.EFFECT_TYPE.EFFECT_BOMB_1, GetTransform().position, 0f);
				break;
			case SporeType.LiquidNitrogen:
				BattleBufferManager.Instance.GenerateEffectFromBuffer(Defined.EFFECT_TYPE.BURST_D_01, GetTransform().position, 5f);
				break;
			case SporeType.PusBlood:
				BattleBufferManager.Instance.GenerateEffectFromBuffer(Defined.EFFECT_TYPE.BURST_B_01, GetTransform().position, 5f);
				break;
			}
			int layerMask = ((base.clique != 0) ? 1536 : 526336);
			Collider[] array = Physics.OverlapSphere(GetTransform().position, 2f, layerMask);
			Collider[] array2 = array;
			foreach (Collider collider in array2)
			{
				DS2ActiveObject @object = DS2ObjectStub.GetObject<DS2ActiveObject>(collider.gameObject);
				if (@object == null)
				{
				}
				base.hitInfo.repelDirection = @object.GetTransform().position - GetTransform().position;
				if (@object.OnHit(base.hitInfo).isHit)
				{
				}
			}
		}

		public override void Destroy(bool destroy = false)
		{
			base.Destroy(destroy);
			GameBattle.m_instance.AddToInteractObjectNeedDeleteList(this);
		}

		public override IPathFinding GetPathFinding()
		{
			return null;
		}
	}
}

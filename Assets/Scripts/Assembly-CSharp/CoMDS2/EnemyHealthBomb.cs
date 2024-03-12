using UnityEngine;

namespace CoMDS2
{
	public class EnemyHealthBomb : DS2ActiveObject
	{
		private AIStateChase stateChase;

		private float m_explodeWaitTime = 3f;

		private float m_timer;

		public EnemyHealthBomb()
		{
			base.objectType = Defined.OBJECT_TYPE.OBJECT_TYPE_OTHERS;
		}

		public override void Initialize(GameObject prefab, string name, Vector3 position, Quaternion rotation, int layer)
		{
			base.Initialize(prefab, name, position, rotation, layer);
			GetTransform().parent = BattleBufferManager.s_activeObjectRoot.transform;
			base.hitInfo = new HitInfo();
			base.hitInfo.repelDistance = new NumberSection<float>(2f, 3f);
			base.hitInfo.repelTime = 0.2f;
			base.hitInfo.source = this;
			AIState aIState = new AIState(this, "Idle");
			aIState.SetCustomFunc(OnIdle);
			AddAIState(aIState.name, aIState);
			SetDefaultAIState(aIState);
			SwitchFSM(aIState);
		}

		protected override void LoadResources()
		{
			base.LoadResources();
			BattleBufferManager.Instance.CreateEffectBufferByType(Defined.EFFECT_TYPE.EFFECT_BOMB_1, 6);
		}

		public void SetActive(bool active, Character creator)
		{
			if (active)
			{
				GetGameObject().SetActive(active);
				base.clique = creator.clique;
				SwitchFSM(GetAIState("Idle"));
				base.hitInfo.damage = new NumberSection<float>(creator.GetHitInfo().damage.left * 2f, creator.GetHitInfo().damage.right * 2f);
			}
			else
			{
				GetGameObject().SetActive(active);
			}
		}

		public override HitResultInfo OnHit(HitInfo hitInfo)
		{
			return new HitResultInfo();
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
				if (m_timer >= 3f)
				{
					OnBomb();
				}
				break;
			}
		}

		public void OnBomb()
		{
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
			DataConf.EffectData effectDataByIndex = DataCenter.Conf().GetEffectDataByIndex(5);
			BattleBufferManager.Instance.GenerateEffectFromBuffer(Defined.EFFECT_TYPE.EFFECT_BOMB_1, GetTransform().position, effectDataByIndex.playTime);
			Destroy();
			SetActive(false, null);
		}

		public override void Destroy(bool destroy = false)
		{
			base.Destroy(destroy);
			GameBattle.m_instance.AddToInteractObjectNeedDeleteList(this);
		}
	}
}

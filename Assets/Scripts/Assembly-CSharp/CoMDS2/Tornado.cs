using UnityEngine;

namespace CoMDS2
{
	public class Tornado : DS2ActiveObject
	{
		private float m_timer;

		private float m_time;

		private Character m_creator;

		private BuffTrigger buffTriggerHp;

		private DS2ActiveObject m_aroundObj;

		private int m_currSeat;

		private Vector3 m_targetPosition;

		private float m_moveSpeed = 5f;

		private Vector3 m_moveDirection;

		private CharacterController m_characterController;

		public static AllySeat s_allySeat;

		public Tornado()
		{
			base.objectType = Defined.OBJECT_TYPE.OBJECT_TYPE_OTHERS;
		}

		public override void Initialize(GameObject prefab, string name, Vector3 position, Quaternion rotation, int layer)
		{
			base.Initialize(prefab, name, position, rotation, layer);
			GetTransform().parent = BattleBufferManager.s_activeObjectRoot.transform;
			BuffTrigger[] componentsInChildren = GetGameObject().GetComponentsInChildren<BuffTrigger>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				if (componentsInChildren[i].affectType == Buff.AffectType.Hp)
				{
					buffTriggerHp = componentsInChildren[i];
					break;
				}
			}
			m_characterController = GetGameObject().GetComponent<CharacterController>();
			AIState aIState = new AIState(this, "Idle");
			aIState.SetCustomFunc(OnIdle);
			AIState aIState2 = new AIState(this, "FindSeat");
			aIState2.SetCustomFunc(OnFindSeat);
			AddAIState(aIState.name, aIState);
			AddAIState(aIState2.name, aIState2);
			SetDefaultAIState(aIState);
			SwitchFSM(aIState);
		}

		public void SetDamage(float damage)
		{
			buffTriggerHp.value = damage;
		}

		public void SetAroundObj(DS2ActiveObject obj)
		{
			m_aroundObj = obj;
		}

		protected override void LoadResources()
		{
			base.LoadResources();
		}

		public void SetActive(bool active, Character creater)
		{
			if (active)
			{
				m_creator = creater;
				GetGameObject().SetActive(active);
				base.clique = m_creator.clique;
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
				if (m_timer >= 3f)
				{
					ChangeAIState("FindSeat");
				}
				break;
			}
		}

		public void OnFindSeat(AIState.AIPhase phase)
		{
			switch (phase)
			{
			case AIState.AIPhase.Enter:
			{
				m_timer = 0f;
				Vector3 seatAndPosition = s_allySeat.GetSeatAndPosition(ref m_currSeat);
				m_targetPosition = m_aroundObj.GetTransform().position + seatAndPosition * Random.Range(5f, 15f);
				m_moveDirection = m_targetPosition - GetTransform().position;
				float magnitude = m_moveDirection.magnitude;
				m_time = magnitude / m_moveSpeed;
				break;
			}
			case AIState.AIPhase.Update:
				m_timer += Time.deltaTime;
				if (m_timer >= m_time)
				{
					ChangeAIState("Idle");
				}
				m_characterController.Move(m_moveDirection.normalized * m_moveSpeed * Time.deltaTime);
				break;
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

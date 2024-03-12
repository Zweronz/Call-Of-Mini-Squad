using UnityEngine;

namespace CoMDS2
{
	public class AIStateAllyFindSeat : AIState
	{
		private enum Phase
		{
			Move = 0,
			Stay = 1
		}

		private Phase m_phase;

		private Player m_character;

		private DS2ActiveObject m_aroundTarget;

		private Vector3 m_targetPosition;

		private float m_time;

		private float m_findTime;

		private NumberSection<float> m_range;

		private bool m_attack;

		public AIStateAllyFindSeat(Player character, string name, Controller controller = Controller.System)
			: base(character, name, controller)
		{
			m_character = character;
			m_range = new NumberSection<float>(2f, Player.ALLY_TOFOLLOW_DIS);
		}

		protected override void OnEnter()
		{
			m_aroundTarget = GameBattle.m_instance.GetPlayer();
			Vector3 seatAndPosition = Player.s_allySeat.GetSeatAndPosition(ref m_character.seatId);
			Vector3 targetPosition = m_aroundTarget.GetTransform().position + seatAndPosition * Random.Range(m_range.left, m_range.right);
			m_targetPosition = targetPosition;
			m_time = 0f;
			m_findTime = Random.Range(3f, 5f);
			m_attack = false;
			stateToMove();
		}

		protected override void OnExit()
		{
		}

		protected override void OnUpdate(float deltaTime)
		{
			if (m_phase == Phase.Move)
			{
				updatePaseMove();
			}
			else if (m_phase == Phase.Stay)
			{
				updatePhaseStay();
			}
			float num = Player.ALLY_TOFOLLOW_DIS;
			if (DataCenter.State().isPVPMode)
			{
				num = m_character.shootRange;
			}
			else if (Util.s_allyMoveAttack && GameBattle.m_instance.IsInBattle)
			{
				num = 10f;
			}
			float sqrMagnitude = (m_aroundTarget.GetTransform().position - m_character.GetTransform().position).sqrMagnitude;
			if (sqrMagnitude > num * num)
			{
				m_character.ChangeAIState("AllyFollow");
			}
		}

		private void stateToMove()
		{
			m_phase = Phase.Move;
			m_character.SetNavSpeed(m_character.MoveSpeed);
			m_character.SetNavDesination(m_targetPosition);
		}

		private void updatePaseMove()
		{
			base.animName = m_character.animLowerBody;
			IPathFinding pathFinding = m_character.GetPathFinding();
			if (pathFinding != null && pathFinding.HasNavigation())
			{
				pathFinding.SetNavDesination(m_targetPosition);
				m_time += Time.deltaTime;
				UnityEngine.AI.NavMeshHit hit;
				pathFinding.GetNavMeshAgent().SamplePathPosition(-1, 1f, out hit);
				if (hit.distance == 0f)
				{
					m_time = 0f;
					stateToStay();
				}
				else if ((m_character.GetTransform().position.x == m_targetPosition.x && m_character.GetTransform().position.z == m_targetPosition.z) || m_time >= m_findTime)
				{
					m_time = 0f;
					stateToStay();
				}
			}
			if (!DataCenter.Save().m_bTeamMemberAutoAttack)
			{
				return;
			}
			if (!m_attack)
			{
				if (m_character.m_weapon.NeedReload())
				{
					return;
				}
				DS2ActiveObject dS2ActiveObject;
				if (DataCenter.State().isPVPMode)
				{
					dS2ActiveObject = m_character.lockedTarget;
					if (dS2ActiveObject == null)
					{
						dS2ActiveObject = GameBattle.m_instance.GetNearestObjFromTargetList(m_activeObject.GetTransform().position, m_character.clique);
					}
				}
				else
				{
					dS2ActiveObject = GameBattle.m_instance.GetNearestObjFromTargetList(m_activeObject.GetTransform().position, m_character.clique);
				}
				if (dS2ActiveObject != null && dS2ActiveObject.Alive())
				{
					float num = m_character.shootRange * m_character.shootRange;
					float num2 = m_character.meleeRange * m_character.meleeRange;
					float sqrMagnitude = (dS2ActiveObject.GetTransform().position - m_activeObject.GetTransform().position).sqrMagnitude;
					if (sqrMagnitude <= num - 10f)
					{
						m_attack = true;
						Push(m_character.GetAIState("UpperFire"));
					}
				}
				return;
			}
			DS2ActiveObject dS2ActiveObject2;
			if (DataCenter.State().isPVPMode)
			{
				dS2ActiveObject2 = m_character.lockedTarget;
				if (dS2ActiveObject2 == null)
				{
					dS2ActiveObject2 = GameBattle.m_instance.GetNearestObjFromTargetList(m_activeObject.GetTransform().position, m_character.clique);
				}
			}
			else
			{
				dS2ActiveObject2 = GameBattle.m_instance.GetNearestObjFromTargetList(m_activeObject.GetTransform().position, m_character.clique);
			}
			if (dS2ActiveObject2 != null && dS2ActiveObject2.Alive())
			{
				float num3 = m_character.shootRange * m_character.shootRange;
				float num4 = m_character.meleeRange * m_character.meleeRange;
				float sqrMagnitude2 = (dS2ActiveObject2.GetTransform().position - m_activeObject.GetTransform().position).sqrMagnitude;
				if (sqrMagnitude2 >= num3)
				{
					m_attack = false;
					Pop();
					if (m_character.objectType == Defined.OBJECT_TYPE.OBJECT_TYPE_PLAYER)
					{
						m_activeObject.AnimationStop(m_activeObject.animUpperBody);
					}
				}
			}
			else if (dS2ActiveObject2 == null || !dS2ActiveObject2.Alive())
			{
				m_attack = false;
				Pop();
				if (m_character.objectType == Defined.OBJECT_TYPE.OBJECT_TYPE_PLAYER)
				{
					m_activeObject.AnimationStop(m_activeObject.animUpperBody);
				}
			}
		}

		private void stateToStay()
		{
			Switch(m_activeObject.GetAIState("Guard"));
		}

		private void updatePhaseStay()
		{
		}
	}
}

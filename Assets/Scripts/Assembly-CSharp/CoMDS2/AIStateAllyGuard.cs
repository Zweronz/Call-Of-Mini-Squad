using UnityEngine;

namespace CoMDS2
{
	public class AIStateAllyGuard : AIState
	{
		private enum Phase
		{
			Idle = 0,
			Attack = 1,
			Move = 2,
			Stay = 3
		}

		private Phase m_phase = Phase.Stay;

		private Player m_character;

		private DS2ActiveObject m_aroundTarget;

		private bool bAttackControlPlaysTarget;

		private NumberSection<float> m_range;

		private NumberSection<float> m_stayTime;

		private float m_fCurrentStayTime;

		private float m_time;

		private Vector3 m_targetPosition;

		private float m_guardDistance = 10f;

		private bool m_bMoveAttack;

		public bool NeedFindSeat { get; set; }

		public AIStateAllyGuard(Player character, string name, Controller controller = Controller.System)
			: base(character, name, controller)
		{
			m_character = character;
		}

		public void SetGuard(DS2ActiveObject aroundTarget, float minRange = 2f, float maxRange = 4f)
		{
			m_aroundTarget = aroundTarget;
			m_range = new NumberSection<float>(2.5f, Player.ALLY_TOFOLLOW_DIS - 0.5f);
			m_stayTime = new NumberSection<float>(3f, 4.5f);
		}

		protected override void OnEnter()
		{
			m_aroundTarget = GameBattle.m_instance.GetPlayer();
			stateToMove();
			m_bMoveAttack = false;
		}

		protected override void OnExit()
		{
		}

		protected override void OnUpdate(float deltaTime)
		{
			m_aroundTarget = GameBattle.m_instance.GetPlayer();
			if (m_phase == Phase.Stay)
			{
				updatePaseStay();
			}
			else if (m_phase == Phase.Move)
			{
				updatePaseMove();
			}
			else if (m_phase == Phase.Attack)
			{
				updatePaseAttack();
			}
			if (DataCenter.State().isPVPMode)
			{
				m_guardDistance = m_character.shootRange;
			}
			else if (Util.s_allyMoveAttack && GameBattle.m_instance.IsInBattle)
			{
				m_guardDistance = 10f;
			}
			else
			{
				m_guardDistance = Player.ALLY_TOFOLLOW_DIS;
			}
			float sqrMagnitude = (m_aroundTarget.GetTransform().position - m_character.GetTransform().position).sqrMagnitude;
			if (sqrMagnitude > m_guardDistance * m_guardDistance)
			{
				m_character.ChangeAIState("AllyFollow");
			}
		}

		private void stateToStay()
		{
			m_phase = Phase.Stay;
			m_time = 0f;
			m_fCurrentStayTime = Random.Range(m_stayTime.left, m_stayTime.right);
			if (m_character.m_weapon != null)
			{
				m_character.animLowerBody = m_character.GetAnimationNameByWeapon("Idle");
				base.animName = m_character.animLowerBody;
			}
			base.animName2 = m_character.animUpperBody;
			m_activeObject.AnimationStop(base.animName2);
			m_activeObject.AnimationCrossFade(base.animName, true);
			IPathFinding pathFinding = m_character.GetPathFinding();
			if (pathFinding != null && pathFinding.HasNavigation())
			{
				pathFinding.StopNav();
			}
			m_character.SetFire(false, m_character.m_fireDirection);
		}

		private void updatePaseStay()
		{
			m_time += Time.deltaTime;
			if (m_time >= m_fCurrentStayTime)
			{
				m_time = 0f;
				stateToMove();
			}
			else
			{
				if (!DataCenter.Save().m_bTeamMemberAutoAttack)
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
					if (!(sqrMagnitude <= m_character.shootRange * m_character.shootRange - 10f))
					{
						return;
					}
					IPathFinding pathFinding = m_character.GetPathFinding();
					if (pathFinding != null)
					{
						if (pathFinding.HasNavigation())
						{
							pathFinding.StopNav();
						}
						stateToAttack();
						bAttackControlPlaysTarget = false;
					}
				}
				else
				{
					if (GameBattle.m_instance.GetPlayer().m_objAutoShootAreaTarget == null || !GameBattle.m_instance.GetPlayer().m_objAutoShootAreaTarget.Alive())
					{
						return;
					}
					float num3 = m_character.shootRange * m_character.shootRange;
					float num4 = m_character.meleeRange * m_character.meleeRange;
					float sqrMagnitude2 = (GameBattle.m_instance.GetPlayer().m_objAutoShootAreaTarget.GetTransform().position - m_activeObject.GetTransform().position).sqrMagnitude;
					if (!(sqrMagnitude2 <= m_character.shootRange * m_character.shootRange - 10f))
					{
						return;
					}
					IPathFinding pathFinding2 = m_character.GetPathFinding();
					if (pathFinding2 != null)
					{
						if (pathFinding2.HasNavigation())
						{
							pathFinding2.StopNav();
						}
						stateToAttack();
						bAttackControlPlaysTarget = true;
					}
				}
			}
		}

		private void stateToMove(bool searchPosInCurrSeat = false)
		{
			m_bMoveAttack = false;
			m_time = 0f;
			Vector3 seatAndPosition = Player.s_allySeat.GetSeatAndPosition(ref m_character.seatId, searchPosInCurrSeat);
			Vector3 targetPosition = m_aroundTarget.GetTransform().position + seatAndPosition * Random.Range(m_range.left, m_range.right);
			m_targetPosition = targetPosition;
			m_phase = Phase.Move;
			m_character.SetNavSpeed(m_character.MoveSpeed);
			m_character.SetNavDesination(m_targetPosition);
			m_activeObject.AnimationStop(m_activeObject.animUpperBody);
		}

		private void updatePaseMove()
		{
			if (!DataCenter.Save().m_bTeamMemberAutoAttack)
			{
				m_time = 0f;
				stateToStay();
				return;
			}
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
				else if ((m_character.GetTransform().position.x == m_targetPosition.x && m_character.GetTransform().position.z == m_targetPosition.z) || m_time >= 3f)
				{
					m_time = 0f;
					stateToStay();
				}
			}
			if (m_bMoveAttack)
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
				if (sqrMagnitude <= m_character.shootRange * m_character.shootRange - 10f && pathFinding != null)
				{
					Push(m_character.GetAIState("UpperFire"));
					m_bMoveAttack = true;
				}
			}
			else
			{
				Pop();
			}
		}

		private void stateToAttack()
		{
			if (!Util.s_allyMoveAttack)
			{
				m_character.SetMove(false, m_character.MoveDirection);
				m_character.animLowerBody = m_character.GetAnimationNameByWeapon("Idle");
				base.animName = m_character.animLowerBody;
				base.animName2 = m_character.animUpperBody;
				m_activeObject.AnimationStop(base.animName2);
				m_activeObject.AnimationCrossFade(base.animName, true);
			}
			Push(m_character.GetAIState("Shoot"));
			m_phase = Phase.Attack;
		}

		private void updatePaseAttack()
		{
			if (Util.s_allyMoveAttack)
			{
				m_time += Time.deltaTime;
			}
			if (m_time >= m_fCurrentStayTime)
			{
				m_time = 0f;
				Pop();
				stateToMove(true);
				return;
			}
			DS2ActiveObject dS2ActiveObject;
			if (!DataCenter.State().isPVPMode)
			{
				dS2ActiveObject = ((!bAttackControlPlaysTarget) ? GameBattle.m_instance.GetNearestObjFromTargetList(m_activeObject.GetTransform().position, m_character.clique) : GameBattle.m_instance.GetPlayer().m_objAutoShootAreaTarget);
			}
			else
			{
				dS2ActiveObject = m_character.lockedTarget;
				if (dS2ActiveObject == null)
				{
					dS2ActiveObject = GameBattle.m_instance.GetNearestObjFromTargetList(m_activeObject.GetTransform().position, m_character.clique);
				}
			}
			if (dS2ActiveObject == null || !dS2ActiveObject.Alive())
			{
				Pop();
				stateToStay();
			}
		}
	}
}

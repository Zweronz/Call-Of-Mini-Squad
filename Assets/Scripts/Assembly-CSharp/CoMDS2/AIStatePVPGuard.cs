using System;
using UnityEngine;

namespace CoMDS2
{
	public class AIStatePVPGuard : AIState
	{
		public enum Phase
		{
			Idle = 0,
			Attack = 1,
			Move = 2,
			Stay = 3
		}

		private Phase m_phase = Phase.Stay;

		private Player m_character;

		private DS2ActiveObject m_aroundTarget;

		private NumberSection<float> m_range;

		private NumberSection<float> m_stayTime;

		private float m_fCurrentStayTime;

		private float m_time;

		private Vector3 m_targetPosition;

		private bool m_bMoveAttack;

		private int siteNum;

		public Phase GetPhase
		{
			get
			{
				return m_phase;
			}
		}

		public bool NeedFindSeat { get; set; }

		public AIStatePVPGuard(Player character, string name, Controller controller = Controller.System)
			: base(character, name, controller)
		{
			m_character = character;
		}

		public void SetGuard(int site, float minRange = 2f, float maxRange = 4f)
		{
			siteNum = site;
			m_range = new NumberSection<float>(minRange, maxRange);
			m_stayTime = new NumberSection<float>(2f, 6f);
		}

		public void SetAroundTarget(DS2ActiveObject target)
		{
			m_aroundTarget = target;
		}

		protected override void OnEnter()
		{
			if (m_character.m_move)
			{
				stateToMove();
			}
			else
			{
				stateToStay();
			}
			m_bMoveAttack = false;
		}

		protected override void OnExit()
		{
		}

		protected override void OnUpdate(float deltaTime)
		{
			if (m_aroundTarget == null || !m_aroundTarget.Alive())
			{
				searchTarget();
				if (m_aroundTarget == null)
				{
					return;
				}
			}
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
			if (m_aroundTarget != null)
			{
				float sqrMagnitude = (m_aroundTarget.GetTransform().position - m_character.GetTransform().position).sqrMagnitude;
				if (sqrMagnitude > m_character.shootRange * m_character.shootRange && m_phase != Phase.Move)
				{
					AIStateFollow aIStateFollow = m_character.GetAIState("AllyFollow") as AIStateFollow;
					aIStateFollow.SetFollow(m_aroundTarget, m_character.shootRange - 2f);
					m_character.ChangeAIState(aIStateFollow);
				}
			}
		}

		private void stateToStay()
		{
			m_character.SetFire(false, m_character.FaceDirection);
			m_phase = Phase.Stay;
			m_fCurrentStayTime = UnityEngine.Random.Range(m_stayTime.left, m_stayTime.right);
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
		}

		private void updatePaseStay()
		{
			if (m_time >= m_fCurrentStayTime)
			{
				m_time = 0f;
				stateToMove();
				return;
			}
			DS2ActiveObject aroundTarget = m_aroundTarget;
			if (aroundTarget == null || !aroundTarget.Alive())
			{
				return;
			}
			float num = m_character.shootRange * m_character.shootRange;
			float num2 = m_character.meleeRange * m_character.meleeRange;
			float sqrMagnitude = (aroundTarget.GetTransform().position - m_activeObject.GetTransform().position).sqrMagnitude;
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
			}
		}

		private void stateToMove(bool searchPosInCurrSeat = false)
		{
			m_bMoveAttack = false;
			m_character.SetFire(false, m_character.FaceDirection);
			Transform transform = m_character.GetTransform();
			float num = UnityEngine.Random.Range(transform.eulerAngles.y - 90f, transform.eulerAngles.y + 90f) + 180f;
			float num2 = UnityEngine.Random.Range(5f, 12f);
			Vector3 vector = new Vector3(num2 * Mathf.Sin((float)Math.PI / 180f * num), 0f, num2 * Mathf.Cos((float)Math.PI / 180f * num));
			Vector3 targetPosition = transform.position + vector;
			m_targetPosition = targetPosition;
			m_phase = Phase.Move;
			m_character.SetNavSpeed(m_character.MoveSpeed);
			m_character.SetNavDesination(m_targetPosition);
			m_activeObject.AnimationStop(m_activeObject.animUpperBody);
		}

		private void updatePaseMove()
		{
			base.animName = m_character.animLowerBody;
			IPathFinding pathFinding = m_character.GetPathFinding();
			if (pathFinding != null && pathFinding.HasNavigation())
			{
				pathFinding.SetNavDesination(m_targetPosition);
				UnityEngine.AI.NavMeshHit hit;
				pathFinding.GetNavMeshAgent().SamplePathPosition(-1, 1f, out hit);
				if (hit.distance == 0f)
				{
					m_time = 0f;
					stateToStay();
				}
				else if (m_character.GetTransform().position.x == m_targetPosition.x && m_character.GetTransform().position.z == m_targetPosition.z)
				{
					m_time = 0f;
					stateToStay();
				}
			}
			if (m_bMoveAttack)
			{
				return;
			}
			DS2ActiveObject aroundTarget = m_aroundTarget;
			if (aroundTarget != null && aroundTarget.Alive())
			{
				float num = m_character.shootRange * m_character.shootRange;
				float num2 = m_character.meleeRange * m_character.meleeRange;
				float sqrMagnitude = (aroundTarget.GetTransform().position - m_activeObject.GetTransform().position).sqrMagnitude;
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
			Pop();
			m_character.SetFire(true, m_character.FaceDirection);
			Push(m_character.GetAIState("Shoot"));
			m_phase = Phase.Attack;
		}

		private void updatePaseAttack()
		{
			m_time += Time.deltaTime;
			if (m_time >= m_fCurrentStayTime)
			{
				m_time = 0f;
				Pop();
				stateToMove(true);
				return;
			}
			DS2ActiveObject aroundTarget = m_aroundTarget;
			if (aroundTarget == null || !aroundTarget.Alive())
			{
				Pop();
				stateToStay();
			}
		}

		private void searchTarget()
		{
			if (siteNum == -1)
			{
				m_aroundTarget = GameBattle.m_instance.GetRandomObjFromTargetList(m_character);
				if (m_aroundTarget == null)
				{
					stateToStay();
				}
				return;
			}
			DS2ActiveObject[] targetList = GameBattle.m_instance.GetTargetList(m_character.clique);
			if (targetList == null || targetList.Length == 0)
			{
				return;
			}
			bool flag = false;
			if (siteNum >= targetList.Length)
			{
				for (int i = 0; i < targetList.Length; i++)
				{
					if (targetList[i].Alive())
					{
						m_aroundTarget = targetList[i];
						flag = true;
					}
				}
			}
			else if (targetList[siteNum].Alive())
			{
				m_aroundTarget = targetList[siteNum];
				flag = true;
			}
			else
			{
				m_aroundTarget = GameBattle.m_instance.GetNearestObjFromTargetList(m_character.GetTransform().position, m_character.clique);
			}
			m_character.lockedTarget = m_aroundTarget;
			if (!flag)
			{
				stateToStay();
			}
		}
	}
}

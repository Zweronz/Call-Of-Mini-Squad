using System;
using UnityEngine;

namespace CoMDS2
{
	public class AIStateAvoid : AIState
	{
		private Character m_character;

		private Vector3 m_targetPosition;

		public AIStateAvoid(Character npc, string name, Controller controller = Controller.System)
			: base(npc, name, controller)
		{
			m_character = npc;
		}

		protected override void OnEnter()
		{
			Transform transform = m_character.GetTransform();
			float num = UnityEngine.Random.Range(transform.eulerAngles.y - 90f, transform.eulerAngles.y + 90f) + 180f;
			float num2 = UnityEngine.Random.Range(5f, 12f);
			Vector3 vector = new Vector3(num2 * Mathf.Sin((float)Math.PI / 180f * num), 0f, num2 * Mathf.Cos((float)Math.PI / 180f * num));
			Vector3 targetPosition = transform.position + vector;
			m_targetPosition = targetPosition;
			m_character.SetNavSpeed(m_character.MoveSpeed);
			m_character.SetNavDesination(m_targetPosition);
		}

		protected override void OnUpdate(float deltaTime)
		{
			IPathFinding pathFinding = m_character.GetPathFinding();
			if (pathFinding != null && pathFinding.HasNavigation())
			{
				pathFinding.SetNavDesination(m_targetPosition);
				UnityEngine.AI.NavMeshHit hit;
				pathFinding.GetNavMeshAgent().SamplePathPosition(-1, 1f, out hit);
				if (hit.distance == 0f)
				{
					StopMove();
				}
				else if (m_character.GetTransform().position.x == m_targetPosition.x && m_character.GetTransform().position.z == m_targetPosition.z)
				{
					StopMove();
				}
			}
		}

		private void StopMove()
		{
			if (m_character.m_weapon != null)
			{
				m_character.animLowerBody = m_character.GetAnimationNameByWeapon("Idle");
				base.animName = m_character.animLowerBody;
			}
			m_activeObject.AnimationCrossFade(base.animName, true);
			IPathFinding pathFinding = m_character.GetPathFinding();
			if (pathFinding != null && pathFinding.HasNavigation())
			{
				pathFinding.StopNav();
			}
		}
	}
}

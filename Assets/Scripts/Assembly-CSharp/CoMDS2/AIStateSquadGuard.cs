using UnityEngine;

namespace CoMDS2
{
	public class AIStateSquadGuard : AIState
	{
		private enum Phase
		{
			Idle = 0,
			Attack = 1,
			Move = 2,
			MoveAttack = 3,
			Reload = 4,
			FireReady = 5
		}

		private enum SubState
		{
			None = 0,
			Attack = 1,
			Reload = 2,
			FireReady = 3
		}

		private Phase m_phase;

		private SubState m_subState;

		private Player m_character;

		private DS2ActiveObject m_aroundTarget;

		private CharacterController m_characterController;

		private SquadController m_squadController;

		private NumberSection<float> m_range;

		private NumberSection<float> m_stayTime;

		private float m_fCurrentStayTime;

		private float m_time;

		private Vector3 m_targetPosition;

		private float m_guardDistance = 3f;

		private bool m_bMoveAttack;

		private int m_squadSite;

		private float m_emitTime;

		private float m_emitTimer;

		private AIStateSquadAttack m_subStateAttack;

		private AIStateSquadFireReady m_subStateFireReady;

		private AIStateSquadReload m_subStateReload;

		private static float followDistance = 0.8f;

		public bool NeedFindSeat { get; set; }

		public float ReloadAnimSpeed
		{
			set
			{
				m_subStateReload.animSpeed = value;
			}
		}

		public AIStateSquadGuard(Player character, string name, Controller controller = Controller.System)
			: base(character, name, controller)
		{
			m_character = character;
			m_characterController = m_character.GetGameObject().GetComponent<CharacterController>();
			m_subStateAttack = new AIStateSquadAttack(character, name, controller);
			m_subStateFireReady = new AIStateSquadFireReady(character, name, controller);
			m_subStateReload = new AIStateSquadReload(character, name, controller);
		}

		public void SetGuard(int squadSite)
		{
			m_squadSite = squadSite;
			m_subStateAttack.SetSite(squadSite);
			m_subStateFireReady.SetSite(squadSite);
			m_subStateReload.SetSite(squadSite);
		}

		protected override void OnEnter()
		{
			if (m_character.m_weapon != null)
			{
				m_emitTime = m_character.m_weapon.emitTimeInAnimation;
				m_character.animUpperBody = m_character.GetAnimationNameByWeapon("Shoting");
				base.animName2 = m_character.animUpperBody;
			}
			ChangeState(Phase.Idle);
			ChangeSubState(SubState.None);
			m_bMoveAttack = false;
			if (GameBattle.m_instance != null)
			{
				m_squadController = GameBattle.m_instance.GetSquadController();
			}
		}

		protected override void OnExit()
		{
		}

		protected override void OnUpdate(float deltaTime)
		{
			switch (m_phase)
			{
			case Phase.Idle:
				updatePhaseIdle(deltaTime);
				break;
			case Phase.Move:
				updatePaseMove(deltaTime);
				break;
			}
			if (m_character.m_weapon.NeedReload())
			{
				if (m_subState != SubState.Reload)
				{
					ChangeSubState(SubState.Reload);
				}
				else if (m_subStateReload.Finish)
				{
					m_character.m_weapon.DoReload();
					ChangeSubState(SubState.None);
				}
				return;
			}
			if (m_squadController.m_fire)
			{
				if (m_subState != SubState.Attack && !m_character.m_weapon.NeedReload())
				{
					ChangeSubState(SubState.Attack);
				}
				return;
			}
			float distance = 0f;
			if (DataCenter.State().isPVPMode)
			{
				DS2ActiveObject lockedTarget = m_character.lockedTarget;
				if (lockedTarget == null)
				{
					lockedTarget = GameBattle.m_instance.GetNearestObjFromTargetListWithDistance(m_character, ref distance);
				}
			}
			else
			{
				m_character.lockedTarget = null;
				DS2ActiveObject lockedTarget = GameBattle.m_instance.GetNearestObjFromTargetListWithDistance(m_character, ref distance);
			}
			if (distance != 0f && distance < m_character.shootRange * m_character.shootRange)
			{
				if (m_subState != SubState.Attack && !m_character.m_weapon.NeedReload())
				{
					ChangeSubState(SubState.Attack);
				}
			}
			else if (m_subState == SubState.Attack)
			{
				if (m_character.characterType == Player.CharacterType.FireDragon || m_character.characterType == Player.CharacterType.Zero)
				{
					if (!m_character.AnimationPlaying(m_character.animUpperBody))
					{
						ChangeSubState(SubState.FireReady);
					}
				}
				else
				{
					ChangeSubState(SubState.FireReady);
				}
			}
			else if (m_subState == SubState.FireReady && m_subStateFireReady.Finish)
			{
				ChangeSubState(SubState.None);
			}
		}

		private void stateToIdle()
		{
			m_phase = Phase.Idle;
			m_character.m_move = false;
			if (m_character.m_weapon != null)
			{
				m_character.animLowerBody = m_character.GetAnimationNameByWeapon("Idle");
				base.animName = m_character.animLowerBody;
			}
			m_activeObject.AnimationCrossFade(base.animName, true);
		}

		private void updatePhaseIdle(float deltaTime)
		{
			if (m_subState != SubState.Attack && m_subState != SubState.FireReady)
			{
				Player character = m_character;
				Vector3 faceDirection = m_squadController.FaceDirection;
				m_character.GetTransform().forward = faceDirection;
				character.FaceDirection = faceDirection;
			}
			Vector3 vector = new Vector3(m_squadController.SquadSite[m_character.SquadSite].x, m_character.GetTransform().position.y, m_squadController.SquadSite[m_character.SquadSite].z);
			Vector3 position = m_character.GetTransform().position;
			float magnitude = (vector - position).magnitude;
			m_squadController.neareastDistanceFromTeammember = magnitude;
			if (magnitude >= followDistance)
			{
				ChangeState(Phase.Move);
			}
		}

		private void stateToMove()
		{
			m_bMoveAttack = false;
			Vector3 targetPosition = m_squadController.SquadSite[m_character.SquadSite];
			m_targetPosition = targetPosition;
			m_character.SetMove(true, m_character.MoveDirection);
			m_phase = Phase.Move;
		}

		private void updatePaseMove(float deltaTime)
		{
			base.animName = m_character.animLowerBody;
			Vector3 targetPosition = m_squadController.SquadSite[m_character.SquadSite];
			m_targetPosition = targetPosition;
			Vector3 vector = new Vector3(m_targetPosition.x, m_character.GetTransform().position.y, m_targetPosition.z);
			Vector3 position = m_character.GetTransform().position;
			Vector3 moveDirection = vector - position;
			float magnitude = moveDirection.magnitude;
			m_character.MoveDirection = moveDirection;
			if (m_subState != SubState.Attack && m_subState != SubState.FireReady)
			{
				m_character.FaceDirection = m_squadController.FaceDirection;
				m_character.GetTransform().forward = m_character.MoveDirection;
			}
			if (m_squadController.neareastDistanceFromTeammember > magnitude)
			{
				m_squadController.neareastDistanceFromTeammember = magnitude;
			}
			IPathFinding pathFinding = m_character.GetPathFinding();
			if (pathFinding == null || !pathFinding.HasNavigation())
			{
				return;
			}
			m_time += Time.deltaTime;
			if (magnitude > 15f)
			{
				pathFinding.GetNavMeshAgent().Resume();
				pathFinding.SetNavSpeed(m_character.MoveSpeed);
				pathFinding.SetNavDesination(m_targetPosition);
				return;
			}
			if (magnitude <= followDistance / 2f)
			{
				ChangeState(Phase.Idle);
				return;
			}
			pathFinding.StopNav();
			m_character.m_move = true;
			Vector3 motion = moveDirection.normalized * m_character.MoveSpeed * Time.deltaTime;
			m_characterController.Move(motion);
			if (magnitude <= followDistance / 2f)
			{
				ChangeState(Phase.Idle);
			}
		}

		private void ChangeState(Phase phase)
		{
			switch (phase)
			{
			case Phase.Idle:
				stateToIdle();
				break;
			case Phase.Move:
				stateToMove();
				break;
			case Phase.Attack:
				break;
			}
		}

		private void ChangeSubState(SubState subState)
		{
			m_subState = subState;
			switch (subState)
			{
			case SubState.None:
				Pop();
				base.animName2 = m_character.animUpperBody;
				if (m_subState == SubState.None)
				{
					m_activeObject.AnimationStop(base.animName2);
				}
				break;
			case SubState.Attack:
				Push(m_subStateAttack);
				break;
			case SubState.FireReady:
				Push(m_subStateFireReady);
				break;
			case SubState.Reload:
				Push(m_subStateReload);
				break;
			}
		}
	}
}

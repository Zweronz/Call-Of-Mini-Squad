using UnityEngine;

namespace CoMDS2
{
	public class AIStateSquadFireReady : AIState
	{
		private Player m_character;

		private CharacterController m_characterController;

		private SquadController m_squadController;

		private int m_squadSite;

		private bool m_finish;

		public bool Finish
		{
			get
			{
				return m_finish;
			}
		}

		public AIStateSquadFireReady(Player character, string name, Controller controller = Controller.System)
			: base(character, name, controller)
		{
			m_character = character;
			m_characterController = m_character.GetGameObject().GetComponent<CharacterController>();
		}

		public void SetSite(int squadSite)
		{
			m_squadSite = squadSite;
		}

		protected override void OnEnter()
		{
			m_finish = false;
			m_squadController = GameBattle.m_instance.GetSquadController();
			stateToFireReady();
		}

		protected override void OnExit()
		{
		}

		protected override void OnUpdate(float deltaTime)
		{
			updatePhaseFireReady(deltaTime);
		}

		private void stateToFireReady()
		{
			m_character.animUpperBody = m_character.GetAnimationNameByWeapon("ReadyFire");
			base.animName2 = m_character.animUpperBody;
			m_activeObject.AnimationCrossFade(base.animName2, false);
		}

		private void updatePhaseFireReady(float deltaTime)
		{
			if (!m_finish && !m_activeObject.AnimationPlaying(base.animName2))
			{
				m_finish = true;
			}
		}
	}
}

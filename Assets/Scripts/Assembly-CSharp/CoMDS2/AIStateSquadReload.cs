using UnityEngine;

namespace CoMDS2
{
	public class AIStateSquadReload : AIState
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

		public AIStateSquadReload(Player character, string name, Controller controller = Controller.System)
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
			stateToReload();
			m_character.isInReload = true;
		}

		protected override void OnExit()
		{
			m_character.isInReload = false;
		}

		protected override void OnUpdate(float deltaTime)
		{
			updatePaseReload(deltaTime);
		}

		private void stateToReload()
		{
			m_character.animUpperBody = m_character.GetAnimationNameByWeapon("Reload");
			base.animName2 = m_character.animUpperBody;
			m_activeObject.AnimationCrossFade(base.animName2, false);
			m_activeObject.SetAnimationSpeed(base.animName2, base.animSpeed / m_character.m_weapon.attribute.reloadTime);
		}

		private void updatePaseReload(float deltaTime)
		{
			if (m_character.m_weapon.ReloadFinished())
			{
				m_finish = true;
			}
		}
	}
}

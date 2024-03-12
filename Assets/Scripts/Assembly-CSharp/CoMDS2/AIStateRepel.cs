using UnityEngine;

namespace CoMDS2
{
	public class AIStateRepel : AIState
	{
		private float m_repelDis;

		private float m_repelTime;

		private float m_repelSpeed;

		private Vector3 m_direction;

		private float m_timer;

		private Character m_character;

		private CharacterController m_characterController;

		private float m_hurtTime;

		public float HurtTime
		{
			get
			{
				return m_hurtTime;
			}
			set
			{
				m_hurtTime = value;
			}
		}

		public AIStateRepel(Character obj, string name, Controller controller = Controller.System)
			: base(obj, name, controller)
		{
			m_character = obj;
			m_characterController = m_character.GetGameObject().GetComponent<CharacterController>();
		}

		public void SetRepel(float dis, float time, Vector3 dir)
		{
			m_repelDis = dis;
			m_repelTime = time;
			m_repelSpeed = dis / m_repelTime;
			m_direction = dir;
			m_timer = 0f;
		}

		protected override void OnEnter()
		{
			m_character.AnimationStop(m_activeObject.animUpperBody);
			m_character.AnimationStop(m_activeObject.animLowerBody);
			if (m_character.objectType == Defined.OBJECT_TYPE.OBJECT_TYPE_ALLY || m_character.objectType == Defined.OBJECT_TYPE.OBJECT_TYPE_PLAYER)
			{
				m_activeObject.animLowerBody = m_character.GetAnimationNameByWeapon("Back");
				base.animName = m_activeObject.animLowerBody;
				Player player = (Player)m_character;
				if (player.CurrentController)
				{
					player.FaceToMoveDirection = false;
				}
			}
			else if (m_character.objectType == Defined.OBJECT_TYPE.OBJECT_TYPE_ENEMY)
			{
				base.animName = ((Enemy)m_activeObject).GetAnimationName("Hurt");
			}
			float num = Mathf.Max(m_repelTime, m_hurtTime);
			m_character.SetAnimationSpeed(base.animName, m_character.AnimationLength(base.animName) / num);
			m_character.AnimationPlay(base.animName, false);
			m_timer = 0f;
		}

		protected override void OnExit()
		{
			if (m_character.objectType == Defined.OBJECT_TYPE.OBJECT_TYPE_ALLY || m_character.objectType == Defined.OBJECT_TYPE.OBJECT_TYPE_PLAYER)
			{
				Player player = (Player)m_character;
				if (player.CurrentController)
				{
					player.FaceToMoveDirection = true;
				}
			}
		}

		protected override void OnUpdate(float deltaTime)
		{
			m_timer += deltaTime;
			if (m_timer >= m_repelTime)
			{
				if (m_timer >= m_hurtTime)
				{
					m_timer = 0f;
					m_character.ChangeToLastAIState();
				}
			}
			else if ((bool)m_characterController)
			{
				m_characterController.Move(m_direction.normalized * m_repelSpeed * deltaTime);
			}
			else
			{
				m_character.GetTransform().Translate(m_direction.normalized * m_repelSpeed * deltaTime, Space.World);
			}
		}
	}
}

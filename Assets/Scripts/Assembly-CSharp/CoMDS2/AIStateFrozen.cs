using UnityEngine;

namespace CoMDS2
{
	public class AIStateFrozen : AIState
	{
		private enum FrozenState
		{
			Appear = 0,
			Continue = 1
		}

		private float m_frozenTime;

		private float m_timer;

		private CharacterController m_characterController;

		private Character m_character;

		private FrozenState m_frozenState;

		private float m_emitTime;

		private float m_emitTimer;

		private bool m_bControlFire;

		public AIStateFrozen(DS2ActiveObject obj, string name, Controller controller = Controller.System)
			: base(obj, name, controller)
		{
			m_characterController = obj.GetGameObject().GetComponent<CharacterController>();
			m_character = (Character)obj;
		}

		public void SetFrozen(float time)
		{
			m_frozenTime = time;
		}

		protected override void OnEnter()
		{
			m_timer = 0f;
			m_emitTimer = 0f;
			m_bControlFire = false;
			m_characterController = m_activeObject.GetGameObject().GetComponent<CharacterController>();
			m_activeObject.AnimationStop(m_activeObject.animUpperBody);
			if (m_activeObject.objectType == Defined.OBJECT_TYPE.OBJECT_TYPE_PLAYER || m_activeObject.objectType == Defined.OBJECT_TYPE.OBJECT_TYPE_ALLY)
			{
				Player player = (Player)m_activeObject;
				if (player.m_weapon != null)
				{
					m_emitTime = player.m_weapon.emitTimeInAnimation;
				}
				DS2ActiveObject activeObject = m_activeObject;
				string animLowerBody = (base.animName = ((Player)m_activeObject).GetAnimationName("Idle"));
				activeObject.animLowerBody = animLowerBody;
				m_activeObject.AnimationCrossFade(base.animName, false);
			}
			else if (m_activeObject.objectType == Defined.OBJECT_TYPE.OBJECT_TYPE_ENEMY)
			{
				DS2ActiveObject activeObject2 = m_activeObject;
				string animLowerBody = (base.animName = ((Enemy)m_activeObject).GetAnimationName("Idle"));
				activeObject2.animLowerBody = animLowerBody;
				m_activeObject.AnimationCrossFade(base.animName, true);
			}
			m_character.effectPlayManager.PlayEffect("FrozenAppear");
			m_frozenState = FrozenState.Appear;
			m_character.isFrozen = true;
			m_character.SetMove(false, m_character.MoveDirection);
			if (m_character.GetPathFinding().HasNavigation())
			{
				m_character.GetPathFinding().StopNav();
			}
		}

		protected override void OnExit()
		{
			m_character.isFrozen = false;
			m_character.effectPlayManager.StopEffect("FrozenAppear");
			m_character.effectPlayManager.StopEffect("FrozenContinue");
		}

		protected override void OnUpdate(float deltaTime)
		{
			m_timer += deltaTime;
			if (m_timer >= m_frozenTime)
			{
				m_timer = 0f;
				m_character.effectPlayManager.StopEffect("FrozenContinue");
				m_activeObject.ChangeToDefaultAIState();
				return;
			}
			EffectControl effectControl = m_character.effectPlayManager.GetEffectControl("FrozenAppear");
			if (effectControl.isFinish && m_frozenState == FrozenState.Appear)
			{
				m_frozenState = FrozenState.Continue;
				m_character.effectPlayManager.PlayEffect("FrozenContinue");
			}
			if (m_activeObject.objectType != 0 && m_activeObject.objectType != Defined.OBJECT_TYPE.OBJECT_TYPE_ALLY)
			{
				return;
			}
			if (m_character.m_weapon.NeedReload())
			{
				if (base.ChildState == null || base.ChildState.name != "UpperReload")
				{
					Push(m_character.GetAIState("UpperReload"));
				}
				return;
			}
			if (base.ChildState != null && base.ChildState.name == "UpperReload" && m_character.m_weapon.ReloadFinished())
			{
				Pop();
			}
			Player player = (Player)m_character;
			if (!player.CurrentController || player.AutoControl || player.HalfAutoControl)
			{
				m_bControlFire = false;
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
					if (sqrMagnitude <= m_character.shootRange * m_character.shootRange - 10f && (base.ChildState == null || base.ChildState.name != "UpperFire"))
					{
						Push(m_character.GetAIState("UpperFire"));
					}
				}
			}
			else if (m_character.m_fire)
			{
				if (m_character.m_weapon == null || m_character.m_weapon.NeedReload())
				{
					return;
				}
				if (!m_bControlFire)
				{
					m_bControlFire = true;
					Character character = m_character;
					string animUpperBody = (base.animName2 = m_character.GetAnimationName("Shoting"));
					character.animUpperBody = animUpperBody;
					if (!m_character.m_weapon.m_bRunningFire)
					{
						m_character.AnimationPlay(base.animName2, false, true);
					}
					else
					{
						m_character.AnimationPlay(base.animName2, true);
					}
				}
				Pop();
				if (m_emitTimer != -1f && m_character.AnimationPlaying(base.animName2))
				{
					m_emitTimer += Time.deltaTime;
					if (m_emitTimer >= m_emitTime)
					{
						m_emitTimer = -1f;
						m_character.m_weapon.UpdateFire(deltaTime);
					}
				}
				if (m_character.m_weapon.firePermit() && m_emitTimer == -1f)
				{
					m_emitTimer = 0f;
					if (!m_character.m_weapon.m_bRunningFire)
					{
						m_character.AnimationPlay(base.animName2, false, true);
					}
				}
			}
			else
			{
				Pop();
			}
		}
	}
}

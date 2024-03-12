using UnityEngine;

namespace CoMDS2
{
	internal class AIStateMoveShoot : AIState
	{
		protected CharacterController m_characterController;

		private Character m_character;

		private float m_changeTime;

		private float m_emitTime;

		private float m_emitTimer;

		private float m_reduceSpeed;

		private string animHurt = string.Empty;

		private float m_originalSpeed;

		public Vector3 direction { get; set; }

		public AIStateMoveShoot(Character character, string name, Controller controller = Controller.System)
			: base(character, name, controller)
		{
			m_character = character;
			m_characterController = character.GetGameObject().GetComponent<CharacterController>();
		}

		protected override void OnEnter()
		{
			m_originalSpeed = m_character.MoveSpeed;
			m_reduceSpeed = m_character.baseAttribute.moveSpeed * 0f;
			m_character.MoveSpeed -= m_reduceSpeed;
			if (m_activeObject.objectType == Defined.OBJECT_TYPE.OBJECT_TYPE_PLAYER || m_activeObject.objectType == Defined.OBJECT_TYPE.OBJECT_TYPE_ALLY)
			{
				Player player = (Player)m_activeObject;
				base.animName = player.animLowerBody;
				player.animUpperBody = player.GetAnimationNameByWeapon("Shoting");
				base.animName2 = player.animUpperBody;
				if (!player.CurrentController)
				{
					m_character.SetNavSpeed(m_character.MoveSpeed);
				}
				m_emitTime = player.m_weapon.emitTimeInAnimation;
			}
			if (m_character.m_weapon.m_bRunningFire)
			{
				m_character.AnimationPlay(base.animName2, true);
			}
			else
			{
				m_emitTimer = -1f;
			}
		}

		protected override void OnExit()
		{
			m_character.MoveSpeed += m_reduceSpeed;
			if (m_character.objectType == Defined.OBJECT_TYPE.OBJECT_TYPE_PLAYER && !((Player)m_character).CurrentController)
			{
				m_character.SetNavSpeed(m_character.MoveSpeed);
			}
			if (m_character.m_weapon != null)
			{
				m_character.m_weapon.StopFire();
			}
		}

		protected override void OnUpdate(float deltaTime)
		{
			if (m_character.objectType == Defined.OBJECT_TYPE.OBJECT_TYPE_PLAYER)
			{
				Player player = (Player)m_character;
				if (player.CurrentController)
				{
					base.animName = m_character.animLowerBody;
					if (player.m_bPlayerLockAttackDirection)
					{
						if (player.m_objAutoShootAreaTarget != null)
						{
							if (Vector3.Distance(player.m_objAutoShootAreaTarget.GetTransform().position, player.GetTransform().position) >= DataCenter.Save().m_fPlayerCautionAreaRadius)
							{
								player.m_objAutoShootAreaTarget = null;
							}
							else if (Vector3.Distance(player.m_objAutoShootAreaTarget.GetTransform().position, player.GetTransform().position) >= DataCenter.Save().m_fPlayerCautionSectorRadius || Vector3.Angle(player.GetTransform().forward, (player.m_objAutoShootAreaTarget.GetTransform().position - player.GetTransform().position).normalized) >= DataCenter.Save().m_fPlayerCautionSectorAngle / 2f)
							{
								player.m_objAutoShootAreaTarget = null;
							}
							else
							{
								player.GetTransform().LookAt(player.m_objAutoShootAreaTarget.GetTransform());
							}
						}
						if (player.m_objAutoShootAreaTarget == null)
						{
							DS2ActiveObject nearestObjFromTargetList = GameBattle.m_instance.GetNearestObjFromTargetList(m_activeObject.GetTransform().position, m_activeObject.clique);
							if (nearestObjFromTargetList != null)
							{
								if (Vector3.Distance(nearestObjFromTargetList.GetTransform().position, player.GetTransform().position) < DataCenter.Save().m_fPlayerCautionAreaRadius)
								{
									player.m_objAutoShootAreaTarget = nearestObjFromTargetList;
									player.GetTransform().LookAt(nearestObjFromTargetList.GetTransform());
								}
								else
								{
									Collider[] array = Physics.OverlapSphere(player.GetTransform().position, DataCenter.Save().m_fPlayerCautionSectorRadius, 2048);
									for (int i = 0; i < array.Length; i++)
									{
										DS2ActiveObject @object = DS2ObjectStub.GetObject<DS2ActiveObject>(array[i].gameObject);
										if (Vector3.Angle(player.GetTransform().forward, (@object.GetTransform().position - player.GetTransform().position).normalized) < DataCenter.Save().m_fPlayerCautionSectorAngle / 2f)
										{
											player.m_objAutoShootAreaTarget = @object;
											player.GetTransform().LookAt(@object.GetTransform());
											break;
										}
									}
								}
							}
						}
					}
					else
					{
						player.m_objAutoShootAreaTarget = null;
					}
					if (!m_character.m_fire)
					{
						m_activeObject.ChangeAIState("FireReady");
					}
					if (!m_character.m_move)
					{
						m_activeObject.ChangeAIState("Shoot");
					}
					else
					{
						if (player.m_objAutoShootAreaTarget == null)
						{
							m_character.GetModelTransform().forward = m_character.FaceDirection;
						}
						m_characterController.Move(m_activeObject.GetMoveTransform().TransformDirection(m_character.MoveDirection) * deltaTime * m_character.MoveSpeed);
					}
				}
				else
				{
					DS2ActiveObject dS2ActiveObject;
					if (DataCenter.State().isPVPMode)
					{
						dS2ActiveObject = m_character.lockedTarget;
						if (dS2ActiveObject == null)
						{
							dS2ActiveObject = GameBattle.m_instance.GetNearestObjFromTargetList(m_activeObject.GetTransform().position, m_activeObject.clique);
						}
					}
					else
					{
						dS2ActiveObject = GameBattle.m_instance.GetNearestObjFromTargetList(m_activeObject.GetTransform().position, m_activeObject.clique);
					}
					if (dS2ActiveObject != null && dS2ActiveObject.Alive())
					{
						m_character.GetTransform().forward = dS2ActiveObject.GetTransform().position - m_character.GetTransform().position;
						m_character.FaceDirection = m_character.GetModelTransform().forward;
						float sqrMagnitude = (dS2ActiveObject.GetTransform().position - m_activeObject.GetTransform().position).sqrMagnitude;
						float num = m_character.shootRange * m_character.shootRange;
						if (sqrMagnitude > num)
						{
							m_activeObject.ChangeToDefaultAIState();
						}
					}
					else
					{
						m_activeObject.ChangeToDefaultAIState();
					}
				}
			}
			else if (m_character.objectType == Defined.OBJECT_TYPE.OBJECT_TYPE_ALLY)
			{
				DS2ActiveObject dS2ActiveObject2;
				if (DataCenter.State().isPVPMode)
				{
					dS2ActiveObject2 = m_character.lockedTarget;
					if (dS2ActiveObject2 == null)
					{
						dS2ActiveObject2 = GameBattle.m_instance.GetNearestObjFromTargetList(m_activeObject.GetTransform().position, m_activeObject.clique);
					}
				}
				else
				{
					dS2ActiveObject2 = GameBattle.m_instance.GetNearestObjFromTargetList(m_activeObject.GetTransform().position, m_activeObject.clique);
				}
				if (dS2ActiveObject2 != null && dS2ActiveObject2.Alive())
				{
					m_character.GetTransform().forward = dS2ActiveObject2.GetTransform().position - m_character.GetTransform().position;
					m_character.FaceDirection = m_character.GetModelTransform().forward;
				}
				else
				{
					m_activeObject.ChangeToDefaultAIState();
				}
			}
			if (!m_character.m_fire || m_character.m_weapon == null || m_character.m_weapon.NeedReload())
			{
				return;
			}
			if (m_character.m_weapon.m_bRunningFire)
			{
				if (!m_character.AnimationPlaying(base.animName2) && !m_character.AnimationPlaying(animHurt))
				{
					m_character.AnimationPlay(base.animName2, true);
				}
			}
			else if (m_character.AnimationPlaying(animHurt))
			{
				m_emitTimer = -1f;
			}
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
	}
}

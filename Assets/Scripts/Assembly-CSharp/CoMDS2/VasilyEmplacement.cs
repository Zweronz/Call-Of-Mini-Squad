using UnityEngine;

namespace CoMDS2
{
	public class VasilyEmplacement : DS2ActiveObject
	{
		private float m_timer;

		private float m_duration = 20f;

		private Player m_creator;

		private int m_iBulletCombo;

		private float m_fireFrequence = 1f;

		private float m_fireTimer;

		private Quaternion m_bulletRotation;

		protected float m_shootRange = 20f;

		private string m_fireAnimName = "Attack";

		private float m_emitTimer;

		private float m_emitTime = 0.03f;

		private Transform m_spine;

		private DS2ActiveObject m_lockTarget;

		protected EffectParticleContinuous m_effectFireLeft;

		protected EffectParticleContinuous m_effectFireRight;

		protected EffectAnimationEmitPool m_effectCartridge;

		protected EffectControl m_effectLight;

		protected Transform m_effectLightPoint;

		protected Transform m_firePointLeft;

		protected Transform m_firePointRight;

		protected Transform m_weaponBonePointLeft;

		protected Transform m_weaponBonePointRight;

		protected Bullet.BulletAttribute bulletAttribute;

		private DS2ObjectBuffer m_bulletBuffer;

		private int m_bulletEmitCount;

		protected EffectControl m_effectAppear;

		protected EffectControl m_effectDisappear;

		private GameObject m_model;

		public Defined.EFFECT_TYPE effectHit { get; set; }

		public float Duration
		{
			get
			{
				return m_duration;
			}
			set
			{
				m_duration = value;
				m_bulletEmitCount = (int)(m_duration / m_fireFrequence);
			}
		}

		private VasilyEmplacement()
		{
		}

		public VasilyEmplacement(Player creator)
		{
			m_creator = creator;
			base.clique = m_creator.clique;
			base.objectType = Defined.OBJECT_TYPE.OBJECT_TYPE_OTHERS;
		}

		public override void Initialize(GameObject prefab, string name, Vector3 position, Quaternion rotation, int layer)
		{
			base.Initialize(prefab, name, position, rotation, layer);
			GetTransform().parent = BattleBufferManager.s_activeObjectRoot.transform;
			base.hitInfo = new HitInfo(m_creator.skillHitInfo);
			base.hitInfo.repelTime = 0.2f;
			AIState aIState = new AIState(this, "Idle");
			AIState aIState2 = new AIState(this, "Guard");
			aIState2.SetCustomFunc(OnIdle);
			AIState aIState3 = new AIState(this, "Shoot");
			aIState3.SetCustomFunc(OnFire);
			AIState aIState4 = new AIState(this, "Born");
			aIState4.SetCustomFunc(OnAppear);
			AIState aIState5 = new AIState(this, "Death");
			aIState5.SetCustomFunc(OnDisappear);
			AddAIState(aIState.name, aIState);
			AddAIState(aIState2.name, aIState2);
			AddAIState(aIState3.name, aIState3);
			AddAIState(aIState4.name, aIState4);
			AddAIState(aIState5.name, aIState5);
			SetDefaultAIState(aIState2);
			SwitchFSM(aIState);
			m_spine = GetTransform().Find("Model/Spine01");
			m_effectFireLeft = null;
			m_firePointLeft = GetTransform().Find("Model/Spine01/FirePointL");
			if (m_firePointLeft.childCount > 0)
			{
				m_effectFireLeft = m_firePointLeft.GetChild(0).GetComponent<EffectParticleContinuous>();
				m_effectFireLeft.transform.parent = BattleBufferManager.s_effectObjectRoot.transform;
			}
			m_effectFireRight = null;
			m_firePointRight = GetTransform().Find("Model/Spine01/FirePointR");
			if (m_firePointRight.childCount > 0)
			{
				m_effectFireRight = m_firePointRight.GetChild(0).GetComponent<EffectParticleContinuous>();
				m_effectFireRight.transform.parent = BattleBufferManager.s_effectObjectRoot.transform;
			}
			Transform transform = GetTransform().Find("EffectLight");
			if (transform != null)
			{
				m_effectLight = transform.gameObject.GetComponentInChildren<EffectControl>();
				if (m_effectLight != null)
				{
					m_effectLight.Root.SetActive(false);
				}
			}
			m_effectLightPoint = GetTransform().Find("Model/Spine01/EffectLightPoint").transform;
			m_weaponBonePointLeft = GetTransform().Find("Model/Spine01/Spine/Mouth03");
			m_weaponBonePointRight = GetTransform().Find("Model/Spine01/Spine/Mouth01");
			DataConf.SkillVasily skillVasily = (DataConf.SkillVasily)m_creator.skillInfo;
			m_fireFrequence = 1f / (skillVasily.fireFrequence / 60f);
			m_shootRange = skillVasily.range;
			m_duration = skillVasily.lifeTime;
			Bullet.BulletAttribute bulletAttribute = new Bullet.BulletAttribute();
			bulletAttribute.speed = skillVasily.speed;
			bulletAttribute.life = skillVasily.range;
			bulletAttribute.effectHit = (Defined.EFFECT_TYPE)skillVasily.effectHitType;
			this.bulletAttribute = bulletAttribute;
			DataConf.BulletData bulletDataByIndex = DataCenter.Conf().GetBulletDataByIndex(skillVasily.bulletType);
			int b = (int)(4f / m_fireFrequence);
			b = Mathf.Max(1, b) * 2;
			m_bulletBuffer = new DS2ObjectBuffer(b);
			GameObject gameObject = new GameObject();
			gameObject.name = bulletDataByIndex.fileNmae;
			gameObject.transform.parent = BattleBufferManager.s_bulletObjectRoot.transform;
			NumberSection<float> aTK = skillVasily.GetATK();
			for (int i = 0; i < b; i++)
			{
				Bullet bullet = new Bullet(null);
				bullet.Initialize(Resources.Load<GameObject>("Models/Bullets/" + bulletDataByIndex.fileNmae), bulletDataByIndex.fileNmae, Vector3.zero, Quaternion.identity, 0);
				GameObject gameObject2 = bullet.GetGameObject();
				gameObject2.AddComponent<BulletTriggerScript>();
				gameObject2.AddComponent<LinearMoveToDestroy>();
				bullet.hitInfo.damage = aTK;
				bullet.hitInfo.repelDistance = new NumberSection<float>(0f, 0f);
				gameObject2.SetActive(false);
				bullet.GetTransform().parent = gameObject.transform;
				m_bulletBuffer.AddObj(bullet);
			}
			m_bulletEmitCount = (int)(m_duration / m_fireFrequence);
			m_effectAppear = null;
			GameObject gameObject3 = GetTransform().Find("EffectAppear").gameObject;
			m_effectAppear = gameObject3.GetComponentInChildren<EffectControl>();
			m_effectDisappear = null;
			GameObject gameObject4 = GetTransform().Find("EffectDisappear").gameObject;
			m_effectDisappear = gameObject4.GetComponentInChildren<EffectControl>();
			m_effectDisappear.gameObject.SetActive(false);
			m_model = GetTransform().Find("Model").gameObject;
			base.hitInfo.damage = aTK;
			m_fireTimer = m_fireFrequence;
		}

		protected override void LoadResources()
		{
			base.LoadResources();
			BattleBufferManager.Instance.CreateEffectBufferByType(Defined.EFFECT_TYPE.EFFECT_HIT_5);
		}

		public void SetActive(bool active)
		{
			if (active)
			{
				GetGameObject().SetActive(active);
				base.clique = m_creator.clique;
				m_effectDisappear.gameObject.SetActive(false);
				m_model.SetActive(false);
				m_timer = 0f;
				m_iBulletCombo = 0;
				SwitchFSM(GetAIState("Born"));
			}
			else
			{
				GetGameObject().SetActive(active);
			}
		}

		public override void Update(float deltaTime)
		{
			base.Update(deltaTime);
			if (m_fireTimer < m_fireFrequence)
			{
				m_fireTimer += deltaTime;
			}
		}

		public override HitResultInfo OnHit(HitInfo hitInfo)
		{
			HitResultInfo hitResultInfo = new HitResultInfo();
			hitResultInfo.isHit = true;
			return hitResultInfo;
		}

		public void OnIdle(AIState.AIPhase phase)
		{
			switch (phase)
			{
			case AIState.AIPhase.Enter:
				m_emitTimer = -1f;
				break;
			case AIState.AIPhase.Update:
			{
				m_timer += Time.deltaTime;
				if (m_timer >= m_duration)
				{
					ChangeAIState("Death");
				}
				DS2ActiveObject dS2ActiveObject = null;
				if (DataCenter.State().isPVPMode)
				{
					dS2ActiveObject = m_creator.lockedTarget;
					if (dS2ActiveObject == null)
					{
						dS2ActiveObject = GameBattle.m_instance.GetNearestObjFromTargetList(GetTransform().position, base.clique);
					}
				}
				else if (GameBattle.m_instance != null)
				{
					dS2ActiveObject = GameBattle.m_instance.GetNearestObjFromTargetList(GetTransform().position, base.clique);
				}
				if (dS2ActiveObject != null && dS2ActiveObject.Alive())
				{
					float num = m_shootRange * m_shootRange;
					float sqrMagnitude = (dS2ActiveObject.GetTransform().position - GetTransform().position).sqrMagnitude;
					if (sqrMagnitude <= num)
					{
						StateToFire();
					}
				}
				if (!(GameBattle.m_instance == null))
				{
					break;
				}
				if (m_emitTimer != -1f && AnimationPlaying(m_fireAnimName))
				{
					m_emitTimer += Time.deltaTime;
					if (m_emitTimer >= m_emitTime)
					{
						m_emitTimer = -1f;
						UpdateFire(Time.deltaTime);
					}
				}
				if (firePermit() && m_emitTimer == -1f)
				{
					m_emitTimer = 0f;
					AnimationPlay(m_fireAnimName, false);
				}
				break;
			}
			}
		}

		public void OnFire(AIState.AIPhase phase)
		{
			switch (phase)
			{
			case AIState.AIPhase.Enter:
				m_emitTimer = -1f;
				if (DataCenter.State().isPVPMode)
				{
					m_lockTarget = m_creator.lockedTarget;
					if (m_lockTarget == null)
					{
						m_lockTarget = GameBattle.m_instance.GetNearestObjFromTargetList(GetTransform().position, base.clique);
					}
				}
				else
				{
					m_lockTarget = GameBattle.m_instance.GetNearestObjFromTargetList(GetTransform().position, base.clique);
				}
				break;
			case AIState.AIPhase.Update:
				m_timer += Time.deltaTime;
				if (m_timer >= m_duration)
				{
					ChangeAIState("Death");
				}
				if (m_lockTarget == null || !m_lockTarget.Alive())
				{
					m_lockTarget = GameBattle.m_instance.GetNearestObjFromTargetList(GetTransform().position, base.clique);
				}
				if (m_lockTarget != null && m_lockTarget.Alive())
				{
					float sqrMagnitude = (m_lockTarget.GetTransform().position - GetTransform().position).sqrMagnitude;
					float num = m_shootRange * m_shootRange;
					if (sqrMagnitude > num)
					{
						ChangeToDefaultAIState();
						break;
					}
					m_spine.forward = m_lockTarget.GetTransform().position - GetTransform().position;
					if (m_emitTimer != -1f && AnimationPlaying(m_fireAnimName))
					{
						m_emitTimer += Time.deltaTime;
						if (m_emitTimer >= m_emitTime)
						{
							m_emitTimer = -1f;
							UpdateFire(Time.deltaTime);
						}
					}
					if (firePermit() && m_emitTimer == -1f)
					{
						m_emitTimer = 0f;
						AnimationPlay(m_fireAnimName, false);
					}
				}
				else
				{
					ChangeToDefaultAIState();
				}
				break;
			}
		}

		public void OnAppear(AIState.AIPhase phase)
		{
			switch (phase)
			{
			case AIState.AIPhase.Enter:
				m_timer = 0f;
				m_effectAppear.StartEmit();
				break;
			case AIState.AIPhase.Update:
				m_timer += Time.deltaTime;
				if (m_timer >= 0.3f)
				{
					m_model.SetActive(true);
					ChangeAIState("Idle");
				}
				break;
			}
		}

		public void OnDisappear(AIState.AIPhase phase)
		{
			switch (phase)
			{
			case AIState.AIPhase.Enter:
				m_timer = 0f;
				m_effectDisappear.gameObject.SetActive(true);
				m_effectDisappear.StartEmit();
				break;
			case AIState.AIPhase.Update:
				m_timer += Time.deltaTime;
				if (m_timer >= 1.78f)
				{
					Destroy();
				}
				break;
			}
		}

		public override void Destroy(bool destroy = false)
		{
			base.Destroy(destroy);
			if (GameBattle.m_instance != null)
			{
				GameBattle.m_instance.AddToInteractObjectNeedDeleteList(this);
			}
		}

		public void StateToGuard()
		{
			SwitchFSM(GetAIState("Guard"));
		}

		public void StateToFire()
		{
			ChangeAIState("Shoot");
		}

		protected void EffectFireStart()
		{
			if (m_effectFireLeft != null)
			{
				m_effectFireLeft.transform.rotation = m_firePointLeft.transform.rotation;
				m_effectFireLeft.transform.position = m_firePointLeft.transform.position;
				m_effectFireLeft.StartEmit();
			}
			if (m_effectFireRight != null)
			{
				m_effectFireRight.transform.rotation = m_firePointRight.transform.rotation;
				m_effectFireRight.transform.position = m_firePointRight.transform.position;
				m_effectFireRight.StartEmit();
			}
		}

		public void EffectCartridgeEmit()
		{
			if (m_effectCartridge != null)
			{
				m_effectCartridge.Emit();
			}
		}

		public void PlayEffectLight()
		{
			if (m_effectLight != null)
			{
				m_effectLight.Root.transform.position = new Vector3((m_firePointLeft.position.x + m_firePointRight.position.x) * 0.5f, m_effectLight.Root.transform.position.y, m_firePointLeft.position.z);
				m_effectLight.Root.SetActive(true);
				m_effectLight.StartEmit();
			}
		}

		public void StopEffectLight()
		{
			if (m_effectLight != null)
			{
				m_effectLight.Root.SetActive(false);
			}
		}

		public virtual void EmitBullet(float distanceLife = 0f)
		{
			Bullet bullet = null;
			int layer = ((base.clique != Clique.Computer) ? 21 : 23);
			bullet = GetBulletFromBuffer();
			bullet.GetGameObject().layer = layer;
			if (bullet != null)
			{
				bullet.hitInfo = GetHitInfo();
				bullet.SetBullet(this, bulletAttribute, new Vector3(m_firePointLeft.position.x, m_firePointLeft.position.y, m_firePointLeft.position.z), m_bulletRotation);
				bullet.Emit(distanceLife);
			}
			bullet = null;
			bullet = GetBulletFromBuffer();
			bullet.GetGameObject().layer = layer;
			if (bullet != null)
			{
				bullet.hitInfo = GetHitInfo();
				bullet.SetBullet(this, bulletAttribute, new Vector3(m_firePointRight.position.x, m_firePointRight.position.y, m_firePointRight.position.z), m_bulletRotation);
				bullet.Emit(distanceLife);
			}
		}

		public virtual Bullet GetBulletFromBuffer()
		{
			return m_bulletBuffer.GetObject() as Bullet;
		}

		protected new virtual void UpdateFire(float deltaTime)
		{
			m_iBulletCombo++;
			EffectFireStart();
			EffectCartridgeEmit();
			PlayEffectLight();
			float shootRange = m_shootRange;
			m_fireTimer = 0f;
			m_bulletRotation = m_spine.rotation;
			HitInfo hitInfo = GetHitInfo();
			hitInfo.repelDirection = GetModelTransform().forward;
			EmitBullet(shootRange);
		}

		public bool firePermit()
		{
			return m_fireTimer >= m_fireFrequence;
		}
	}
}

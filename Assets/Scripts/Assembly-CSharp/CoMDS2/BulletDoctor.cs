namespace CoMDS2
{
	public class BulletDoctor : Bullet
	{
		public float changeHpPercent;

		public BulletDoctor(DS2Object creator)
			: base(creator)
		{
		}

		public override void TriggerEnter(DS2Object obj)
		{
			IFighter fighter = obj.GetFighter();
			if (fighter == null)
			{
				return;
			}
			DS2ActiveObject dS2ActiveObject = (DS2ActiveObject)obj;
			if (m_creator.clique == dS2ActiveObject.clique || !dS2ActiveObject.Alive())
			{
				return;
			}
			HitInfo hitInfo = base.hitInfo;
			hitInfo.hitPoint = GetTransform().position;
			hitInfo.repelDirection = GetModelTransform().forward;
			HitResultInfo hitResultInfo = fighter.OnHit(hitInfo);
			if (!hitResultInfo.isHit)
			{
				return;
			}
			if (obj.GetGameObject().layer == 11 && hitResultInfo.damage > 0f)
			{
				DS2ActiveObject[] teammateList = GameBattle.m_instance.GetTeammateList(DS2ActiveObject.Clique.Player);
				for (int i = 0; i < teammateList.Length; i++)
				{
					Character character = (Character)teammateList[i];
					if (character.Alive())
					{
						int hp = character.hp;
						int num = character.hp + (int)(hitResultInfo.damage * changeHpPercent / (float)GameBattle.m_instance.GetPlayerAliveList().Length);
						int num2 = num - hp;
						character.hp = num;
						EffectNumManager.instance.GenerageEffectNum(EffectNumber.EffectNumType.Heal, num2, character.m_effectPoint.position);
						BattleBufferManager.Instance.GenerateEffectFromBuffer(Defined.EFFECT_TYPE.Bloodthirst, character.GetTransform().position, 0f, character.GetTransform());
					}
				}
			}
			ICollider collider = obj.GetCollider();
			if (collider != null)
			{
				collider.OnCollide(this);
				HitEffect();
			}
		}
	}
}

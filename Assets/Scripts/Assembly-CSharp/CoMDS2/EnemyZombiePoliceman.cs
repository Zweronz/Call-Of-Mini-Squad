using System.Collections.Generic;
using UnityEngine;

namespace CoMDS2
{
	public class EnemyZombiePoliceman : Enemy
	{
		public override void Initialize(GameObject prefab, string name, Vector3 position, Quaternion rotation, int layer)
		{
			base.Initialize(prefab, name, position, rotation, layer);
			AIStateShootWithWeapon aIStateShootWithWeapon = new AIStateShootWithWeapon(this, "Shoot");
			base.meleeAble = false;
			base.shootAble = true;
			base.shootRange = 15f;
			SetAttackCollider(false);
			AddAIState(aIStateShootWithWeapon.name, aIStateShootWithWeapon);
			if (m_weaponMap == null)
			{
				m_weaponMap = new Dictionary<int, Weapon>();
			}
			if (base.enemyType == EnemyType.PolicemanPistol)
			{
				WeaponPistolCollide weaponPistolCollide = new WeaponPistolCollide();
				weaponPistolCollide.attribute.damage = baseAttribute.damage;
				weaponPistolCollide.attribute.repelDis = base.hitInfo.repelDistance;
				weaponPistolCollide.attribute.fireFrequency = m_attFrequency;
				weaponPistolCollide.attribute.attackRange = 10f;
				weaponPistolCollide.attribute.bulletSpeed = 6.5f;
				weaponPistolCollide.attribute.bulletType = 27;
				weaponPistolCollide.attribute.hitType = Weapon.HIT_TYPE.COLLIDE;
				weaponPistolCollide.SetBulletExternal();
				base.hitInfo.buffs.Add(new Buff(Buff.AffectType.MoveSpeed, -0.4f, 0.2f, 0.2f, Buff.CalcType.Percentage, 0f));
				AddWeapon(0, weaponPistolCollide);
				UseWeapon(0);
			}
			else if (base.enemyType == EnemyType.PolicemanShotgun)
			{
				WeaponShotgunNPC weaponShotgunNPC = new WeaponShotgunNPC(Defined.RANK_TYPE.WHITE);
				weaponShotgunNPC.attribute.damage = baseAttribute.damage;
				weaponShotgunNPC.attribute.repelDis = base.hitInfo.repelDistance;
				weaponShotgunNPC.attribute.fireFrequency = m_attFrequency;
				weaponShotgunNPC.attribute.attackRange = 5f;
				weaponShotgunNPC.emitTimeInAnimation = 0.33f;
				weaponShotgunNPC.SetBulletExternal();
				AddWeapon(0, weaponShotgunNPC);
				UseWeapon(0);
			}
			isBig = true;
		}

		protected override void SetAnimationsMixing()
		{
			if (enemyAnimTag == null || enemyAnimTag == string.Empty)
			{
				return;
			}
			Transform mix = m_transform.Find("Bip01/Spine_00/Bip01 Spine");
			DataConf.AnimData enemyAnim = DataCenter.Conf().GetEnemyAnim(enemyAnimTag, "Hurt");
			if (enemyAnim.count > 1)
			{
				for (int i = 0; i < enemyAnim.count; i++)
				{
					m_gameObject.GetComponent<Animation>()[enemyAnim.name + "0" + (i + 1)].layer = 2;
					m_gameObject.GetComponent<Animation>()[enemyAnim.name + "0" + (i + 1)].AddMixingTransform(mix);
				}
			}
		}

		public new virtual void SetEnemy(Vector3 position, Quaternion rotation)
		{
			m_transform.position = position;
			m_transform.rotation = rotation;
		}

		public override void Update(float deltaTime)
		{
			base.Update(deltaTime);
		}

		private void AddAnimationEvents()
		{
		}

		public override HitResultInfo OnHit(HitInfo hitInfo)
		{
			HitResultInfo result = new HitResultInfo();
			AIState currentAIState = GetCurrentAIState();
			if (currentAIState.name == "Born")
			{
				return result;
			}
			return base.OnHit(hitInfo);
		}
	}
}

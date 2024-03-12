using UnityEngine;

namespace CoMDS2
{
	internal class BulletBuilder
	{
		public static Bullet CreateBullet(Bullet.BULLET_TYPE type, DS2Object creator, Vector3 position, Quaternion rotation)
		{
			Bullet bullet = new Bullet(creator);
			switch (type)
			{
			case Bullet.BULLET_TYPE.RIFLE_FIRELINE:
			{
				bullet.Initialize(Resources.Load<GameObject>("Models/Bullets/Rifle_fireline"), "trajectory", position, rotation, 0);
				GameObject gameObject4 = bullet.GetGameObject();
				gameObject4.AddComponent<LinearMoveToDestroy>();
				gameObject4.SetActive(false);
				break;
			}
			case Bullet.BULLET_TYPE.FAKE:
			{
				bullet.Initialize(Resources.Load<GameObject>("Models/Bullets/BulletFake"), "BulletFake", position, rotation, 0);
				GameObject gameObject3 = bullet.GetGameObject();
				gameObject3.AddComponent<LinearMoveToDestroy>();
				gameObject3.SetActive(false);
				break;
			}
			case Bullet.BULLET_TYPE.SNIPER:
			{
				bullet.Initialize(Resources.Load<GameObject>("Models/Bullets/Sniper_03_Bullet"), "Sniper_03_Bullet", position, rotation, 0);
				GameObject gameObject2 = bullet.GetGameObject();
				gameObject2.AddComponent<LinearMoveToDestroy>();
				gameObject2.SetActive(false);
				break;
			}
			case Bullet.BULLET_TYPE.GRENADE:
			{
				bullet.Initialize(Resources.Load<GameObject>("Models/Bullets/Grenade_01_bullet"), "Grenade_01_bullet", position, rotation, 0);
				GameObject gameObject = bullet.GetGameObject();
				gameObject.AddComponent<BulletTriggerScript>();
				gameObject.AddComponent<LinearMoveToDestroy>();
				gameObject.SetActive(false);
				break;
			}
			}
			return bullet;
		}
	}
}

using UnityEngine;

namespace CoMDS2
{
	public class BulletGrenade : Bullet
	{
		public override void Initialize(GameObject prefab, string name, Vector3 position, Quaternion rotation, int layer)
		{
			base.Initialize(prefab, name, position, rotation, layer);
		}

		public override void HitEffect()
		{
			DS2HalfStaticObject effectFromBuffer = BattleBufferManager.Instance.GetEffectFromBuffer(Defined.EFFECT_TYPE.EFFECT_WEAPON_GRENADE_04);
			if (effectFromBuffer != null)
			{
				BuffTrigger component = effectFromBuffer.GetGameObject().GetComponent<BuffTrigger>();
				if (GetCreator().clique == DS2ActiveObject.Clique.Computer)
				{
					effectFromBuffer.GetGameObject().layer = 15;
				}
				else if (GetCreator().clique == DS2ActiveObject.Clique.Player)
				{
					effectFromBuffer.GetGameObject().layer = 14;
				}
				else
				{
					effectFromBuffer.GetGameObject().layer = 20;
				}
				effectFromBuffer.GetTransform().parent = BattleBufferManager.s_effectObjectRoot.transform;
				effectFromBuffer.GetTransform().position = base.hitInfo.hitPoint;
				effectFromBuffer.GetGameObject().SetActive(true);
			}
			Destroy();
		}
	}
}

using UnityEngine;

namespace CoMDS2
{
	public class Item : DS2ActiveObject
	{
		public enum ItemType
		{
			Jerrican = 0,
			Box = 1
		}

		public override void Initialize(GameObject gameObject)
		{
			base.Initialize(gameObject);
			base.objectType = Defined.OBJECT_TYPE.OBJECT_TYPE_ITEM;
		}

		public override HitResultInfo OnHit(HitInfo hitInfo)
		{
			HitResultInfo hitResultInfo = new HitResultInfo();
			hitResultInfo = base.OnHit(hitInfo);
			if (hitResultInfo.isHit && base.hp > 0)
			{
				SetSplash(true);
			}
			return hitResultInfo;
		}
	}
}

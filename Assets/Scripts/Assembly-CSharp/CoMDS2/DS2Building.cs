using UnityEngine;

namespace CoMDS2
{
	public class DS2Building : DS2ActiveObject
	{
		public enum BuildingType
		{
			X1Tower = 0,
			X1Cannon = 1,
			Others = 2
		}

		public BuildingType type { get; set; }

		public override void Initialize(GameObject gameObject)
		{
			GameObject gameObject2 = null;
			Transform transform = gameObject.transform.Find("Model");
			gameObject2 = ((!(transform == null)) ? transform.gameObject : gameObject);
			base.Initialize(gameObject2);
			isBuilding = true;
		}

		public override HitResultInfo OnHit(HitInfo hitInfo)
		{
			HitResultInfo hitResultInfo = new HitResultInfo();
			if (isBuilding)
			{
				hitResultInfo.isHit = true;
				return hitResultInfo;
			}
			return base.OnHit(hitInfo);
		}
	}
}

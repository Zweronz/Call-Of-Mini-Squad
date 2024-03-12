namespace CoMDS2
{
	public class HitResultInfo
	{
		public bool isHit;

		public bool isCirt;

		public float damage;

		public DS2ActiveObject target;

		public HitResultInfo()
		{
			isHit = false;
			isCirt = false;
			damage = 0f;
			target = null;
		}
	}
}

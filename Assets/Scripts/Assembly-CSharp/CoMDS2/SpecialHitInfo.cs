namespace CoMDS2
{
	public class SpecialHitInfo
	{
		public NumberSection<float> repelDistance;

		public float time;

		public float specialHitParam;

		public int specialHitProbability = 100;

		public bool disposable;

		public SpecialHitInfo()
		{
			repelDistance = new NumberSection<float>(0f);
		}

		public SpecialHitInfo(NumberSection<float> repelDistance, float time, bool disposable)
		{
			this.repelDistance = repelDistance;
			this.time = time;
			this.disposable = disposable;
		}
	}
}

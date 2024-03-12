namespace CoMDS2
{
	internal class DS2StaticObject : DS2Object, ICollider
	{
		public virtual void OnCollide(ICollider collider)
		{
		}

		public override ICollider GetCollider()
		{
			return this;
		}

		public override IFighter GetFighter()
		{
			return null;
		}
	}
}

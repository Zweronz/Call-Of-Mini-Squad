namespace CoMDS2
{
	public interface IFighter
	{
		HitResultInfo OnHit(HitInfo hitInfo);

		void UpdateFire(float deltaTime);

		HitInfo GetHitInfo();

		void HitResult(HitResultInfo result);
	}
}

namespace CoMDS2
{
	public interface IBuffManager
	{
		void AddBuff(Buff buff, int accumulateMaxCount = -1);

		bool HasBuff(Buff.AffectType affectType);

		Buff GetBuff(Buff.AffectType affectType);

		void RemoveBuff(Buff.AffectType affectType);

		void RemoveAllBuffs();

		void UpdateBuffs();
	}
}

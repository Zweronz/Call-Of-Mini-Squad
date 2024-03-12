namespace CoMDS2
{
	public class WeaponPistolDoctor : WeaponPistol
	{
		public WeaponPistolDoctor(WeaponType weaponType, PistolType pistolType)
			: base(weaponType, pistolType)
		{
			if (m_pistolLeft != null)
			{
				m_pistolLeft.Dispose();
				m_pistolLeft = null;
			}
			if (m_pistolRight != null)
			{
				m_pistolRight.Dispose();
				m_pistolRight = null;
			}
			m_pistolLeft = new WeaponPistolDoctorSingle(weaponType, pistolType);
			m_pistolRight = new WeaponPistolDoctorSingle(weaponType, pistolType);
		}
	}
}

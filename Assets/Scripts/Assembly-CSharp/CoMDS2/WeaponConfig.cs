namespace CoMDS2
{
	public class WeaponConfig
	{
		private static WeaponConfig m_instance;

		public static WeaponConfig Instance()
		{
			if (m_instance == null)
			{
				m_instance = new WeaponConfig();
				m_instance.LoadConfig();
			}
			return m_instance;
		}

		private void LoadConfig()
		{
		}
	}
}

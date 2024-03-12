namespace CoMDS2
{
	public class DeadSpawnInfo
	{
		public enum DeadSpawnType
		{
			Ninjia_Fire = 0,
			Ninjia_Ice = 1,
			Zombie_Nurse_Venom = 2
		}

		public DeadSpawnType spawnType { get; set; }

		public int spawnCount { get; set; }

		public DeadSpawnInfo(DeadSpawnType type, int spawnCount)
		{
			spawnType = type;
			this.spawnCount = spawnCount;
		}
	}
}

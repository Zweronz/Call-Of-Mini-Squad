using System.Collections.Generic;

public class GameProgressData
{
	public ushort[] levelProgress;

	public ushort modeProgress;

	public Dictionary<Defined.LevelMode, ushort[]> levelStars;

	public GameProgressData()
	{
		levelProgress = new ushort[3];
		for (int i = 0; i < levelProgress.Length; i++)
		{
			levelProgress[i] = 0;
		}
		levelStars = new Dictionary<Defined.LevelMode, ushort[]>();
		modeProgress = 0;
	}
}

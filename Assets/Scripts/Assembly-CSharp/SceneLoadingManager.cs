using UnityEngine;

public class SceneLoadingManager
{
	public static string s_nextSceneName = string.Empty;

	public static string s_lastSceneName = string.Empty;

	public static string s_currSceneName = string.Empty;

	public static bool s_bSwitch = true;

	public static void SwitchScene(string name)
	{
		s_lastSceneName = Application.loadedLevelName;
		s_nextSceneName = name;
		Application.LoadLevel("Loading");
	}
}

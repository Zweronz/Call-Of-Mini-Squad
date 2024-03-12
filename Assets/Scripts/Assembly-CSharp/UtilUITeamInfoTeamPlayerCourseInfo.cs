using UnityEngine;

public class UtilUITeamInfoTeamPlayerCourseInfo : MonoBehaviour
{
	private void Init()
	{
	}

	private void Reset()
	{
	}

	private void OnPress(bool isPressed)
	{
		if (!isPressed)
		{
			Reset();
			Init();
		}
	}
}

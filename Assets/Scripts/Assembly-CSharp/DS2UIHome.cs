using UnityEngine;

public class DS2UIHome : MonoBehaviour
{
	public void OnGUI()
	{
		if (GUI.Button(new Rect(Screen.width - 130, 10f, 120f, 80f), "Battle"))
		{
			Application.LoadLevel("CoM_DS2.Battle");
		}
	}
}

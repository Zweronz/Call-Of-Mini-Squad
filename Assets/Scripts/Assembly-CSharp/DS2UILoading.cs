using UnityEngine;

public class DS2UILoading : MonoBehaviour
{
	public void OnGUI()
	{
		GUI.Label(new Rect(10f, 10f, 200f, 80f), "CoM_DS2");
		for (int i = 0; i < 10; i++)
		{
			int num = i / 5;
			int num2 = i % 5;
			Rect position = new Rect(10 + num2 * 130, 80 + num * 90, 120f, 80f);
			string text = string.Format("Level{0:D02}", i + 1);
			if (GUI.Button(position, text))
			{
				Application.LoadLevel("CoM_DS2.Home");
			}
		}
	}
}

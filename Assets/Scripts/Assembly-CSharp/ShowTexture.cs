using UnityEngine;

public class ShowTexture : MonoBehaviour
{
	public void OnGUI()
	{
		if (GUI.Button(new Rect(5f, 30f, 90f, 30f), "ShowTexture"))
		{
			DoShowTexture();
		}
	}

	private void DoShowTexture()
	{
		Texture[] array = Resources.FindObjectsOfTypeAll(typeof(Texture)) as Texture[];
		foreach (Texture texture in array)
		{
		}
	}
}

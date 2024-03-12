using UnityEngine;

public class UIEntry : MonoBehaviour
{
	private int m_count;

	public void Awake()
	{
		Application.targetFrameRate = 60;
		m_count = 0;
	}

	public void Update()
	{
		m_count++;
		if (m_count != 3)
		{
		}
	}

	public void OnGUI()
	{
		if (GUI.Button(new Rect(10f, 10f, 120f, 80f), "CoM_DS2"))
		{
			Application.LoadLevel("CoM_DS2.Loading");
		}
		if (GUI.Button(new Rect(140f, 10f, 120f, 80f), "CoM_MW"))
		{
			Application.LoadLevel("CoM_MW.Loading");
		}
	}
}

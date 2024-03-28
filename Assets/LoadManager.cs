using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadManager : MonoBehaviour
{
	private void Start()
	{
		DontDestroyOnLoad(gameObject);

		if (Save.TestingCreate || Save.NeedsCreate)
		{
			Save.Create();
			Save.Write();
		}
		else
		{
			Save.Load();
		}
	}

	private void OnApplicationQuit()
	{
		Save.Write();
	}
}

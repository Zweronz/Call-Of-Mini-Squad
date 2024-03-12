using UnityEngine;

public class AndroidQuit : MonoBehaviour
{
	private void Awake()
	{
		Object.DontDestroyOnLoad(base.gameObject);
	}

	private void Start()
	{
	}

	private void Update()
	{
		if (Input.GetKeyUp(KeyCode.Escape))
		{
			DevicePlugin.AndroidQuit();
		}
	}
}

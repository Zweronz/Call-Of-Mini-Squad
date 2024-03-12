using UnityEngine;

public class SceneSetting : MonoBehaviour
{
	public Color ambientLight = new Color32(200, 200, 200, byte.MaxValue);

	private void Start()
	{
		ambientLight = new Color32(200, 200, 200, byte.MaxValue);
		RenderSettings.ambientLight = ambientLight;
	}

	private void Update()
	{
	}
}

using UnityEngine;

internal class ShowFPS : MonoBehaviour
{
	private float updateInterval = 1f;

	private double lastInterval;

	private float frames;

	private string fpsText;

	private void Start()
	{
		lastInterval = Time.realtimeSinceStartup;
		frames = 0f;
		base.enabled = false;
	}

	private void Update()
	{
		frames += 1f;
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		if ((double)realtimeSinceStartup > lastInterval + (double)updateInterval)
		{
			float a = (float)((double)frames / ((double)realtimeSinceStartup - lastInterval));
			fpsText = (1000f / Mathf.Max(a, 1E-05f)).ToString("f1") + "ms " + a.ToString("f2") + "FPS";
			frames = 0f;
			lastInterval = realtimeSinceStartup;
		}
	}

	private void OnGUI()
	{
		GUI.Label(new Rect(1f, 0f, 200f, 20f), fpsText);
	}
}

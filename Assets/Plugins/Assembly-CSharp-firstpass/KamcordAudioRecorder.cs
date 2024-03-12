using UnityEngine;

public class KamcordAudioRecorder : MonoBehaviour
{
	private void OnAudioFilterRead(float[] data, int numChannels)
	{
		if (Kamcord.IsRecording())
		{
			Kamcord.WriteAudioData(data, data.Length / numChannels);
		}
	}
}

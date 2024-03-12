using UnityEngine;

public abstract class ITAudioRule : MonoBehaviour
{
	public abstract bool Try(string sample_name, GameObject go, float over_time);

	public abstract void Stop(GameObject go);
}

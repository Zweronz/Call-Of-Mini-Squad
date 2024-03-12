using UnityEngine;

public abstract class ITAudioEvent : MonoBehaviour
{
	public abstract bool isPlaying { get; }

	public abstract bool isLoop { get; }

	public abstract void Trigger();

	public abstract void Stop();
}

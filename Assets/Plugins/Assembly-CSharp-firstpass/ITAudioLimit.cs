using UnityEngine;

public abstract class ITAudioLimit : MonoBehaviour
{
	public abstract bool isCanPlay { get; }

	public abstract void OnAudioTrigger(AudioClip clip);
}

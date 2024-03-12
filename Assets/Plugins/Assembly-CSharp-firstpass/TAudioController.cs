using UnityEngine;

public class TAudioController : MonoBehaviour
{
	public delegate void OnAudioEventPlay(ref string eventName);

	public bool useAuidoEvent = true;

	private OnAudioEventPlay onAudioEventPlay;

	public void PlayAudio(string objName)
	{
		if (!useAuidoEvent || objName == "NULL")
		{
			return;
		}
		if (onAudioEventPlay != null)
		{
			onAudioEventPlay(ref objName);
		}
		int num = objName.LastIndexOf('/');
		num++;
		string text = objName.Substring(num);
		Transform transform = base.transform.Find("Audio");
		if (null == transform)
		{
			GameObject gameObject = new GameObject("Audio");
			gameObject.transform.parent = base.transform;
			gameObject.transform.localPosition = Vector3.zero;
			transform = gameObject.transform;
		}
		GameObject gameObject2 = null;
		Transform transform2 = base.transform.Find("Audio/" + text);
		if (null == transform2)
		{
			gameObject2 = Resources.Load("SoundEvent/" + objName) as GameObject;
			if (null == gameObject2)
			{
				return;
			}
			gameObject2 = Object.Instantiate(gameObject2) as GameObject;
			if (null == gameObject2)
			{
				return;
			}
			gameObject2.name = text;
			gameObject2.transform.parent = transform;
			gameObject2.transform.localPosition = Vector3.zero;
		}
		else
		{
			gameObject2 = transform2.gameObject;
		}
		ITAudioEvent component = gameObject2.GetComponent<ITAudioEvent>();
		if (component != null)
		{
			component.Trigger();
		}
	}

	public void SetAudioEventPlayDelegate(OnAudioEventPlay onAudioEventDelegate)
	{
		onAudioEventPlay = onAudioEventDelegate;
	}

	public void StopAudio(string audioName)
	{
		GameObject gameObject = null;
		Transform transform = base.transform.Find("Audio/" + audioName);
		if ((bool)transform)
		{
			gameObject = transform.gameObject;
			ITAudioEvent component = gameObject.GetComponent<ITAudioEvent>();
			if (component != null && component.isLoop)
			{
				component.Stop();
			}
		}
	}

	[ContextMenu("PreLoad")]
	private void PreLoad()
	{
	}
}

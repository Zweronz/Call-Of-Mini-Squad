using UnityEngine;

public class TestAudio : MonoBehaviour
{
	public GameObject[] testObjs;

	private GameObject[] m_testObjs;

	private void Start()
	{
		m_testObjs = new GameObject[testObjs.Length];
		for (int i = 0; i < testObjs.Length; i++)
		{
			m_testObjs[i] = Object.Instantiate(testObjs[i]) as GameObject;
			m_testObjs[i].transform.parent = base.transform;
			m_testObjs[i].transform.localPosition = Vector3.zero;
		}
	}

	private void Update()
	{
		if (!Input.GetMouseButtonUp(0))
		{
			return;
		}
		GameObject[] array = m_testObjs;
		foreach (GameObject gameObject in array)
		{
			ITAudioEvent iTAudioEvent = (ITAudioEvent)gameObject.GetComponent(typeof(ITAudioEvent));
			if (iTAudioEvent != null)
			{
				iTAudioEvent.Trigger();
			}
		}
	}
}

using UnityEngine;

public class EffectControl : MonoBehaviour
{
	public GameObject m_root;

	[HideInInspector]
	public bool isBegin;

	[HideInInspector]
	public bool isFinish;

	public GameObject Root
	{
		get
		{
			if (m_root == null)
			{
				return base.gameObject;
			}
			return m_root;
		}
	}

	private void Start()
	{
	}

	public virtual void StartEmit(bool playAudio = true)
	{
		isBegin = true;
		isFinish = false;
		Root.SetActive(true);
	}

	public virtual void StopEmit()
	{
		if (!(GetComponent<TimerDestroy>() != null))
		{
			isBegin = false;
			isFinish = true;
			Root.SetActive(false);
		}
	}

	public virtual GameObject GetGameObject()
	{
		if (m_root == null)
		{
			return base.gameObject;
		}
		return m_root;
	}

	public virtual void OnEnable()
	{
	}

	public virtual void OnDisable()
	{
		isBegin = false;
		isFinish = true;
	}

	public virtual void SetSpeed(float speed)
	{
	}
}

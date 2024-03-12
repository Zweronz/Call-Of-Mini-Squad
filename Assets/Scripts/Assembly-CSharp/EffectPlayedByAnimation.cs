using UnityEngine;

public class EffectPlayedByAnimation : EffectControl
{
	public bool loop;

	public void Awake()
	{
		if (m_root == null)
		{
			m_root = base.gameObject;
		}
	}

	public void Start()
	{
		base.GetComponent<Animation>().wrapMode = ((!loop) ? WrapMode.Once : WrapMode.Loop);
	}

	public override void StartEmit(bool playAudio = true)
	{
		base.GetComponent<Animation>().Rewind();
		base.StartEmit();
		base.GetComponent<Animation>().Play();
	}

	public override void StopEmit()
	{
		base.GetComponent<Animation>().Stop();
		base.StopEmit();
	}

	public void Update()
	{
		if (!base.GetComponent<Animation>().isPlaying && !loop)
		{
			base.Root.SetActive(false);
		}
	}

	public override void SetSpeed(float speed)
	{
		base.SetSpeed(speed);
		base.GetComponent<Animation>()[base.GetComponent<Animation>().clip.name].speed = speed;
	}
}

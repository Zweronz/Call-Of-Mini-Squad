using UnityEngine;

public class EffectPlayedByAnimator : EffectControl
{
	public bool loop;

	private Animator animator;

	private string defaultStateName;

	public void Awake()
	{
		if (m_root == null)
		{
			m_root = base.gameObject;
		}
	}

	public void Start()
	{
		animator = base.gameObject.GetComponent<Animator>();
	}

	public override void StartEmit(bool playAudio = true)
	{
		base.StartEmit();
	}

	public override void StopEmit()
	{
		base.StopEmit();
	}

	public void Update()
	{
		if (!loop && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f && !loop)
		{
			m_root.SetActive(false);
		}
	}
}

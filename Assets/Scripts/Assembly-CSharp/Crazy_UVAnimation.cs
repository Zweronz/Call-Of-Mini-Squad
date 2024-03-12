using UnityEngine;

public class Crazy_UVAnimation : MonoBehaviour
{
	public string animationname;

	private float speed = 1f;

	public int frame;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void OnUVAnimation()
	{
		base.GetComponent<Animation>().Play(animationname);
	}

	public void Pause()
	{
		speed = base.GetComponent<Animation>()[animationname].speed;
		base.GetComponent<Animation>()[animationname].speed = 0f;
	}

	public void Continue()
	{
		base.GetComponent<Animation>()[animationname].speed = speed;
	}

	public void SetTimeFactor(float factor)
	{
		speed = factor;
		base.GetComponent<Animation>()[animationname].speed = speed;
	}
}

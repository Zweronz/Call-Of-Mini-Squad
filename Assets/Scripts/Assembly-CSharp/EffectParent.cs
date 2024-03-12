using UnityEngine;

public class EffectParent : MonoBehaviour
{
	private bool result;

	public void Awake()
	{
	}

	public void OnDestroy()
	{
	}

	public void Update()
	{
		if (Input.GetKeyDown(KeyCode.Comma) && !result)
		{
			result = true;
			EffectParticleContinuous componentInChildren = base.gameObject.GetComponentInChildren<EffectParticleContinuous>();
			if (componentInChildren != null)
			{
				componentInChildren.PlayEffect();
			}
		}
		if (Input.GetKeyUp(KeyCode.Comma))
		{
			result = false;
		}
	}
}

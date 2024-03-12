using UnityEngine;

public class UIToggleMisc : MonoBehaviour
{
	private UIToggle toggle;

	private void Start()
	{
		toggle = base.gameObject.GetComponent<UIToggle>();
	}

	private void OnClick()
	{
		if (toggle.enabled)
		{
			toggle.value = !toggle.value;
		}
	}

	private void OnPress()
	{
		if (toggle.enabled)
		{
			toggle.value = !toggle.value;
		}
	}
}

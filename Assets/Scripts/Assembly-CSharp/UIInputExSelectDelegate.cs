using UnityEngine;

public class UIInputExSelectDelegate : MonoBehaviour
{
	public GameObject target;

	public UIInputExSelectDelegate_SelectedEvent selectEvent;

	private void OnSelect(bool isSelected)
	{
		if (selectEvent != null)
		{
			selectEvent(isSelected, target);
		}
		else
		{
			UIUtil.PDebug("Delegate Event Is NULL!!!", "1-4");
		}
	}
}

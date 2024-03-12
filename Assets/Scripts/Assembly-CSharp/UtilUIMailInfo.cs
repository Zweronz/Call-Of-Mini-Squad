using UnityEngine;

public class UtilUIMailInfo : MonoBehaviour
{
	[SerializeField]
	private UIToggle toggle;

	[SerializeField]
	private UISprite stateIcon;

	[SerializeField]
	private UISprite accessoryIcon;

	[SerializeField]
	private UILabel title;

	[SerializeField]
	private UILabel date;

	private UtilUIMailInfo_ToggleValueChanged_Delegate _event;

	public void SetMailBReadState(bool bRead)
	{
		if (bRead)
		{
			stateIcon.spriteName = "pic_decal082";
		}
		else
		{
			stateIcon.spriteName = "pic_decal083";
		}
	}

	public void SetAccessoryTipsVisable(bool bShow)
	{
		accessoryIcon.gameObject.SetActive(bShow);
	}

	public void UpdateTitle(string _str)
	{
		title.text = _str;
	}

	public void UpdateDate(string _str)
	{
		date.text = _str;
	}

	public void BlindFuntion(UtilUIMailInfo_ToggleValueChanged_Delegate _dele)
	{
		_event = _dele;
	}

	public void HandleMailToggleValueChanged()
	{
		if (_event != null)
		{
			_event(base.gameObject, toggle.value);
		}
		else
		{
			UIUtil.PDebug("Delegate Event Is NULL!!!", "1-4");
		}
	}
}

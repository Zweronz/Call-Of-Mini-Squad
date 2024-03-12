using UnityEngine;

public class UtilUITeamPlayersInfo : MonoBehaviour
{
	[SerializeField]
	private UIToggle[] m_teamPlayersToggle;

	[SerializeField]
	private UIPlaySound[] m_teamPlayersEquipedSounds;

	[SerializeField]
	private UILabel m_teamCombatLabel;

	[SerializeField]
	private UIImageButton m_teamBonusBtn;

	private bool bMTSelect;

	private bool bTeamBounsUnlocked;

	private int lastAccessFormationSiteIndex = -1;

	private UtilUITeamPlayersInfo_TeamBonusBtnClick_Delegate TeamBonusBtnClickEvent;

	private UtilUITeamPlayersInfo_TeamPlayersToggleStateChange_Delegate TeamPlayersToggleStateChangeEvent;

	private UtilUITeamPlayersInfo_TeamPlayerUnlockStateBtnClick_Delegate TeamPlayerUnlockStateBtnClickEvent;

	private GameObject GetTeamPlayerGO(int index)
	{
		return m_teamPlayersToggle[index].gameObject;
	}

	public int GetTeamPlayerIndex(GameObject go)
	{
		int num = -1;
		for (int i = 0; i < m_teamPlayersToggle.Length; i++)
		{
			if (GetTeamPlayerGO(i) == go)
			{
				num = i;
				break;
			}
		}
		if (num == -1)
		{
			UIUtil.PDebug("Can't find the GO!!!", "1-4");
		}
		return num;
	}

	public void PlayEquipedHeroSound(int index)
	{
		m_teamPlayersEquipedSounds[index].Play();
	}

	private void HandleEvent_TeamBonusBtnClicked()
	{
		if (TeamBonusBtnClickEvent != null)
		{
			TeamBonusBtnClickEvent(bTeamBounsUnlocked);
		}
		else
		{
			UIUtil.PDebug("Delegate Event Is NULL!!!", "1-4");
		}
	}

	public void HandleEvent_TeamPlayer0ToggleStateChange()
	{
		if (TeamPlayersToggleStateChangeEvent != null)
		{
			TeamPlayersToggleStateChangeEvent(0, m_teamPlayersToggle[0].value, bMTSelect);
		}
		else
		{
			UIUtil.PDebug("Delegate Event Is NULL!!!", "1-4");
		}
		if (m_teamPlayersToggle[0].value)
		{
			bMTSelect = false;
		}
	}

	public void HandleEvent_TeamPlayer1ToggleStateChange()
	{
		if (TeamPlayersToggleStateChangeEvent != null)
		{
			TeamPlayersToggleStateChangeEvent(1, m_teamPlayersToggle[1].value, bMTSelect);
		}
		else
		{
			UIUtil.PDebug("Delegate Event Is NULL!!!", "1-4");
		}
		if (m_teamPlayersToggle[1].value)
		{
			bMTSelect = false;
		}
	}

	public void HandleEvent_TeamPlayer2ToggleStateChange()
	{
		if (TeamPlayersToggleStateChangeEvent != null)
		{
			TeamPlayersToggleStateChangeEvent(2, m_teamPlayersToggle[2].value, bMTSelect);
		}
		else
		{
			UIUtil.PDebug("Delegate Event Is NULL!!!", "1-4");
		}
		if (m_teamPlayersToggle[2].value)
		{
			bMTSelect = false;
		}
	}

	public void HandleEvent_TeamPlayer3ToggleStateChange()
	{
		if (TeamPlayersToggleStateChangeEvent != null)
		{
			TeamPlayersToggleStateChangeEvent(3, m_teamPlayersToggle[3].value, bMTSelect);
		}
		else
		{
			UIUtil.PDebug("Delegate Event Is NULL!!!", "1-4");
		}
		if (m_teamPlayersToggle[3].value)
		{
			bMTSelect = false;
		}
	}

	public void HandleEvent_TeamPlayer4ToggleStateChange()
	{
		if (TeamPlayersToggleStateChangeEvent != null)
		{
			TeamPlayersToggleStateChangeEvent(4, m_teamPlayersToggle[4].value, bMTSelect);
		}
		else
		{
			UIUtil.PDebug("Delegate Event Is NULL!!!", "1-4");
		}
		if (m_teamPlayersToggle[4].value)
		{
			bMTSelect = false;
		}
	}

	private void HandleEvent_TeamPlayerUnlockStateBtnClicked(GameObject go)
	{
		int teamPlayerIndex = GetTeamPlayerIndex(go.transform.parent.gameObject);
		if (TeamPlayerUnlockStateBtnClickEvent != null)
		{
			TeamPlayerUnlockStateBtnClickEvent(teamPlayerIndex);
		}
		else
		{
			UIUtil.PDebug("Delegate Event Is NULL!!!", "1-4");
		}
	}

	public void Select(int index)
	{
		bMTSelect = true;
		m_teamPlayersToggle[index].value = true;
	}

	public void UpdateTeamCombat(string str)
	{
		m_teamCombatLabel.text = str;
	}

	public void UpdatePlayerIcon(int index, string str)
	{
		UISprite component = GetTeamPlayerGO(index).transform.Find("Icon").GetComponent<UISprite>();
		component.spriteName = str;
		if (str != string.Empty)
		{
			component.alpha = 1f;
		}
		else
		{
			component.alpha = 0f;
		}
	}

	public void SetPlayerArrowVisable(int index, bool bShow)
	{
		GameObject gameObject = GetTeamPlayerGO(index).transform.Find("Arrow Container").gameObject;
		TweenPosition component = gameObject.GetComponent<TweenPosition>();
		component.ResetToBeginning();
		gameObject.SetActive(bShow);
	}

	public void SetPlayerEnableState(int index, bool bDisable, string str = "")
	{
		GetTeamPlayerGO(index).transform.Find("Enable State").gameObject.SetActive(bDisable);
		GetTeamPlayerGO(index).transform.Find("Enable State").Find("Label").GetComponent<UILabel>()
			.text = str;
	}

	public void SetPlayerUnlockState(int index, bool bLock, int crystal, int money)
	{
		Transform transform = GetTeamPlayerGO(index).transform.Find("Unlock State");
		transform.gameObject.SetActive(bLock);
		if (money > 0)
		{
			transform.Find("Price").gameObject.GetComponent<UISprite>().spriteName = UIUtil.GetCurrencyNameByCurrencyType(Defined.COST_TYPE.Money);
			transform.Find("Price").Find("Label").gameObject.GetComponent<UILabel>().text = money + string.Empty;
		}
		else
		{
			transform.Find("Price").gameObject.GetComponent<UISprite>().spriteName = UIUtil.GetCurrencyNameByCurrencyType(Defined.COST_TYPE.Crystal);
			transform.Find("Price").Find("Label").gameObject.GetComponent<UILabel>().text = crystal + string.Empty;
		}
	}

	public void SetTeamBonusBUnlocked(bool bUnlocked)
	{
		bTeamBounsUnlocked = bUnlocked;
		if (bTeamBounsUnlocked)
		{
			m_teamBonusBtn.normalSprite = "btn_classic04_145x60_b";
			m_teamBonusBtn.hoverSprite = "btn_classic04_145x60_b";
			m_teamBonusBtn.pressedSprite = "btn_classic04_145x60_s";
			m_teamBonusBtn.transform.Find("Background").GetComponent<UISprite>().spriteName = "btn_classic04_145x60_b";
		}
		else
		{
			m_teamBonusBtn.normalSprite = "btn_classic03on_192x43_b__L15R15T0B0";
			m_teamBonusBtn.hoverSprite = "btn_classic03on_192x43_b__L15R15T0B0";
			m_teamBonusBtn.pressedSprite = "btn_classic03_on_192x43_s__L15R15T0B0";
			m_teamBonusBtn.transform.Find("Background").GetComponent<UISprite>().spriteName = "btn_classic03on_192x43_b__L15R15T0B0";
		}
	}

	public void SetFormationSiteAccess(int index)
	{
		if (lastAccessFormationSiteIndex != -1)
		{
			m_teamPlayersToggle[lastAccessFormationSiteIndex].transform.Find("Checkmark").GetComponent<UISprite>().spriteName = "pic_decal147";
			m_teamPlayersToggle[lastAccessFormationSiteIndex].transform.Find("Checkmark").GetComponent<UISprite>().MakePixelPerfect();
			if (m_teamPlayersToggle[lastAccessFormationSiteIndex].value)
			{
				m_teamPlayersToggle[lastAccessFormationSiteIndex].activeSprite.alpha = 1f;
			}
			else
			{
				m_teamPlayersToggle[lastAccessFormationSiteIndex].activeSprite.alpha = 0f;
			}
		}
		m_teamPlayersToggle[index].transform.Find("Checkmark").GetComponent<UISprite>().spriteName = "pic_decal148";
		m_teamPlayersToggle[index].transform.Find("Checkmark").GetComponent<UISprite>().MakePixelPerfect();
		m_teamPlayersToggle[index].activeSprite.alpha = 1f;
		lastAccessFormationSiteIndex = index;
	}

	public void SetFormationSiteAccessReset()
	{
		for (int i = 0; i < m_teamPlayersToggle.Length; i++)
		{
			m_teamPlayersToggle[i].transform.Find("Checkmark").GetComponent<UISprite>().spriteName = "pic_decal147";
			m_teamPlayersToggle[i].transform.Find("Checkmark").GetComponent<UISprite>().MakePixelPerfect();
			if (m_teamPlayersToggle[i].value)
			{
				m_teamPlayersToggle[i].activeSprite.alpha = 1f;
			}
			else
			{
				m_teamPlayersToggle[i].activeSprite.alpha = 0f;
			}
		}
		lastAccessFormationSiteIndex = -1;
	}

	public void SetTeamBonusBtnClickDelegate(UtilUITeamPlayersInfo_TeamBonusBtnClick_Delegate dele)
	{
		TeamBonusBtnClickEvent = dele;
	}

	public void SetTeamPlayersToggleStateChangeDelegate(UtilUITeamPlayersInfo_TeamPlayersToggleStateChange_Delegate dele)
	{
		TeamPlayersToggleStateChangeEvent = dele;
	}

	public void SetTeamPlayerUnlockStateBtnClickDelegate(UtilUITeamPlayersInfo_TeamPlayerUnlockStateBtnClick_Delegate dele)
	{
		TeamPlayerUnlockStateBtnClickEvent = dele;
	}
}

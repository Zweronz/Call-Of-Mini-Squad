using System;
using CoMDS2;
using UnityEngine;

public class UtilUIBaseStagePointInfoData : MonoBehaviour
{
	[Serializable]
	public class UtilUIBaseStagePointInfo
	{
		public Transform solidMapTrans;

		public GameObject go;

		public UIImageButton m_Btn;

		public UILabel m_nameLabel;

		public UISprite m_icon;

		public void SetVisable(bool bShow)
		{
			go.SetActive(bShow);
		}

		public void UpdatePosition(Vector3 pos)
		{
			go.transform.parent.localPosition = pos;
		}

		public void UpdatePositionSelf()
		{
			if (solidMapTrans != null)
			{
				Vector3 vector = SolidMapCameraControl.mInstance.WorldToScreenViewPort(solidMapTrans.position);
				Vector3 p = new Vector3((float)Screen.width * vector.x, (float)Screen.height * vector.y, 0f);
				Vector3 pos = Util.ScreenPointToNGUIForAnroid(p);
				UpdatePosition(pos);
			}
		}

		public void UpdateName(string str)
		{
			m_nameLabel.text = str;
		}

		public void UpdateIcon(string str)
		{
			m_icon.spriteName = str;
			m_icon.MakePixelPerfect();
		}

		public void UpdateState(bool bUnlock, bool bClear)
		{
			if (bUnlock)
			{
				m_nameLabel.color = Color.white;
				m_Btn.hoverSprite = m_Btn.disabledSprite;
				m_Btn.pressedSprite = m_Btn.disabledSprite;
				m_Btn.isEnabled = false;
				m_Btn.isEnabled = true;
			}
			else
			{
				m_nameLabel.color = UIUtil._UIGrayColor;
				m_Btn.hoverSprite = m_Btn.normalSprite;
				m_Btn.pressedSprite = m_Btn.normalSprite;
				m_Btn.isEnabled = false;
				m_Btn.isEnabled = true;
			}
		}

		public void SetEffectVisable(bool bEffect)
		{
			m_Btn.transform.parent.Find("Sprite").GetComponent<TweenScale>().enabled = bEffect;
			m_Btn.GetComponent<TweenPosition>().enabled = bEffect;
		}
	}

	[SerializeField]
	private UtilUIBaseStagePointInfo[] UIS = new UtilUIBaseStagePointInfo[1];

	private int id = -1;

	private bool bUnlock;

	private bool bClear;

	private int uiState;

	private UtilUIBaseStagePointInfoData_StageBtnClicked_Delegate StageBtnClickedEvent;

	public UtilUIBaseStagePointInfo UI
	{
		get
		{
			return UIS[uiState];
		}
	}

	public int ID
	{
		get
		{
			return id;
		}
	}

	public bool BUNLOCK
	{
		get
		{
			return bUnlock;
		}
	}

	public bool BCLEAR
	{
		get
		{
			return bClear;
		}
	}

	protected void HandleEvent_StageBtnClicked()
	{
		if (StageBtnClickedEvent != null)
		{
			StageBtnClickedEvent(this);
		}
		else
		{
			UIUtil.PDebug("Delegate Event Is NULL!!!", "1-4");
		}
	}

	protected void SetStageBtnClickDelegate(UtilUIBaseStagePointInfoData_StageBtnClicked_Delegate dele)
	{
		StageBtnClickedEvent = dele;
	}

	public void Init(int id, bool bUnlock, Transform trans, Vector3 position, UtilUIBaseStagePointInfoData_StageBtnClicked_Delegate dele, bool bClear, bool bCurrent, string iconName, string name = "")
	{
		this.id = id;
		this.bUnlock = bUnlock;
		this.bClear = bClear;
		if (bClear)
		{
			uiState = 1;
		}
		else if (bCurrent)
		{
			uiState = 0;
		}
		else
		{
			uiState = 2;
		}
		SetStageBtnClickDelegate(dele);
		UI.UpdateIcon(iconName);
		for (int i = 0; i < UIS.Length; i++)
		{
			UIS[i].SetVisable(false);
			UIS[i].solidMapTrans = trans;
		}
		UI.UpdatePosition(position);
		UI.UpdatePositionSelf();
		UI.SetVisable(true);
		if (name == null)
		{
			UI.UpdateName("STAGE " + ID);
		}
		else
		{
			UI.UpdateName(name);
		}
	}

	public void Init(int id, bool bUnlock, Vector3 position, UtilUIBaseStagePointInfoData_StageBtnClicked_Delegate dele, bool bClear, bool bCurrent, string iconName, string name = "")
	{
		Init(id, bUnlock, null, position, dele, bClear, bCurrent, iconName, name);
	}

	private void Update()
	{
		UI.UpdatePositionSelf();
	}
}

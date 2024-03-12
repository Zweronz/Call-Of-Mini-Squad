using System;
using CoMDS2;
using UnityEngine;

public class UtilUIBaseWavePointInfoData : MonoBehaviour
{
	[Serializable]
	public class UtilUIBaseWavePointInfo
	{
		public Transform solidMapTrans;

		public GameObject go;

		public UIImageButton m_Btn;

		public UILabel m_nameLabel;

		public GameObject m_starsGO;

		public void SetVisable(bool bShow)
		{
			go.SetActive(bShow);
		}

		public void UpdatePosition(int id, Vector3 pos, Vector2 bgSize)
		{
			Vector3 vector = pos;
			float num = vector.x / 1136f;
			float num2 = vector.y / 640f;
			go.transform.parent.parent.localPosition = new Vector3(num * bgSize.x, num2 * bgSize.y, vector.z);
		}

		public void UpdatePosition(Vector3 pos)
		{
			go.transform.parent.parent.localPosition = pos;
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

		public void UpdateStars(int iStarsCount)
		{
			if (m_starsGO != null)
			{
				if (iStarsCount > 0)
				{
					m_starsGO.SetActive(true);
					UIUtil.ShowStars(m_starsGO.transform, iStarsCount);
				}
				else
				{
					m_starsGO.SetActive(false);
				}
			}
		}
	}

	[SerializeField]
	private UtilUIBaseWavePointInfo[] UISElite = new UtilUIBaseWavePointInfo[3];

	[SerializeField]
	private UtilUIBaseWavePointInfo[] UISNormal = new UtilUIBaseWavePointInfo[3];

	public UIPanel panel;

	private int id = -1;

	private bool bElite;

	private bool bCurrent;

	private int uiState;

	private int iStarsCount = -1;

	private UtilUIBaseWavePointInfoData_WaveBtnClicked_Delegate WaveBtnClickedEvent;

	public UtilUIBaseWavePointInfo UI
	{
		get
		{
			if (bElite)
			{
				return UISElite[uiState];
			}
			return UISNormal[uiState];
		}
	}

	public int ID
	{
		get
		{
			return id;
		}
	}

	public bool BELITE
	{
		get
		{
			return bElite;
		}
	}

	public int STARS
	{
		get
		{
			return iStarsCount;
		}
		set
		{
			iStarsCount = value;
			UI.UpdateStars(iStarsCount);
		}
	}

	protected void HandleEvent_WaveBtnClicked()
	{
		if (WaveBtnClickedEvent != null)
		{
			WaveBtnClickedEvent(this);
		}
		else
		{
			UIUtil.PDebug("Delegate Event Is NULL!!!", "1-4");
		}
	}

	protected void SetWaveBtnClickDelegate(UtilUIBaseWavePointInfoData_WaveBtnClicked_Delegate dele)
	{
		WaveBtnClickedEvent = dele;
	}

	public void AddEffectSlowyShow()
	{
		TweenAlpha component = base.gameObject.GetComponent<TweenAlpha>();
		if (component != null)
		{
			UnityEngine.Object.DestroyImmediate(component);
		}
		component = base.gameObject.AddComponent<TweenAlpha>();
		component.from = 0f;
		component.to = 1f;
		component.duration = 1f;
		component.animationCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
	}

	public void SetPanelDepth(int depth)
	{
		panel.depth = depth;
	}

	public void Init(int id, bool bElite, int starsCount, Transform trans, Vector3 position, UtilUIBaseWavePointInfoData_WaveBtnClicked_Delegate dele, bool bDrift, bool bCurrent, Vector2 bgSize, int depth, string name)
	{
		this.id = id;
		this.bElite = bElite;
		this.bCurrent = bCurrent;
		iStarsCount = starsCount;
		if (STARS > 0)
		{
			uiState = 0;
		}
		else if (bCurrent)
		{
			uiState = 1;
		}
		else
		{
			uiState = 2;
		}
		SetWaveBtnClickDelegate(dele);
		for (int i = 0; i < UISElite.Length; i++)
		{
			UISElite[i].SetVisable(false);
			UISElite[i].solidMapTrans = trans;
		}
		for (int j = 0; j < UISNormal.Length; j++)
		{
			UISNormal[j].SetVisable(false);
			UISNormal[j].solidMapTrans = trans;
		}
		UI.SetVisable(true);
		UI.UpdatePosition(id, position, bgSize);
		UI.UpdatePositionSelf();
		UI.UpdateStars(STARS);
		AddEffectSlowyShow();
		SetPanelDepth((depth != -1) ? depth : id);
	}

	public void Init(int id, bool bElite, int starsCount, Vector3 position, UtilUIBaseWavePointInfoData_WaveBtnClicked_Delegate dele, bool bDrift, bool bCurrent, Vector2 bgSize, int depth, string name)
	{
		Init(id, bElite, starsCount, null, position, dele, bDrift, bCurrent, bgSize, depth, name);
	}

	private void Update()
	{
		UI.UpdatePositionSelf();
	}
}

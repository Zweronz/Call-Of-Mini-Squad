using UnityEngine;

public class UITeamAvatarInfo : MonoBehaviour
{
	public UIMoveRotation uiMove;

	private UILabel nameLabel;

	private UITexture modelTexture;

	private UIButton leaderskillImageButton;

	private GameObject modelPartGO;

	private GameObject textPartGO;

	private UILabel introUILabel;

	private UILabel nameUILabel;

	private void Awake()
	{
		nameLabel = base.transform.Find("Panel").Find("Name").Find("Label")
			.GetComponent<UILabel>();
		modelTexture = base.transform.Find("Panel").Find("Model Texture").GetComponent<UITexture>();
		leaderskillImageButton = base.transform.Find("Panel").Find("LeaderSkill Button").GetComponent<UIButton>();
		modelPartGO = base.transform.Find("Panel").gameObject;
		textPartGO = base.transform.Find("Text Panel").gameObject;
		introUILabel = textPartGO.transform.Find("Panel").Find("Label").GetComponent<UILabel>();
		nameUILabel = textPartGO.transform.Find("Panel").Find("Name Label").GetComponent<UILabel>();
	}

	public void SetModelMode(bool bShowModel)
	{
		modelPartGO.SetActive(bShowModel);
		textPartGO.SetActive(!bShowModel);
	}

	public void UpdateInroLabel(string name)
	{
		if ((bool)introUILabel)
		{
			introUILabel.text = name;
		}
	}

	public void UpdateNameUILabel(string name)
	{
		if ((bool)nameUILabel)
		{
			nameUILabel.text = name;
		}
	}

	public void UpdateAvatarName(string name)
	{
		if ((bool)nameLabel)
		{
			nameLabel.text = name;
		}
	}

	public void UpdateAvatarModel(UITexture texture)
	{
		if ((bool)modelTexture)
		{
			modelTexture = texture;
		}
	}

	public void UpdateAvatarModelUV(Rect uv)
	{
		if ((bool)modelTexture)
		{
			modelTexture.uvRect = uv;
		}
	}
}

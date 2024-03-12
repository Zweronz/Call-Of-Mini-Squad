using UnityEngine;

[AddComponentMenu("NGUI/UI/Image Button")]
public class UIImageButton : MonoBehaviour
{
	public UISprite target;

	public string normalSprite;

	public string hoverSprite;

	public string pressedSprite;

	public string disabledSprite;

	public bool pixelSnap = true;

	public bool isEnabled
	{
		get
		{
			Collider collider = base.GetComponent<Collider>();
			return (bool)collider && collider.enabled;
		}
		set
		{
			Collider collider = base.GetComponent<Collider>();
			if ((bool)collider && collider.enabled != value)
			{
				collider.enabled = value;
				UpdateImage();
			}
		}
	}

	private void OnEnable()
	{
		if (target == null)
		{
			target = GetComponentInChildren<UISprite>();
		}
		UpdateImage();
	}

	private void OnValidate()
	{
		if (target != null)
		{
			if (string.IsNullOrEmpty(normalSprite))
			{
				normalSprite = target.spriteName;
			}
			if (string.IsNullOrEmpty(hoverSprite))
			{
				hoverSprite = target.spriteName;
			}
			if (string.IsNullOrEmpty(pressedSprite))
			{
				pressedSprite = target.spriteName;
			}
			if (string.IsNullOrEmpty(disabledSprite))
			{
				disabledSprite = target.spriteName;
			}
		}
	}

	private void UpdateImage()
	{
		if (target != null)
		{
			if (isEnabled)
			{
				SetSprite((!UICamera.IsHighlighted(base.gameObject)) ? normalSprite : hoverSprite);
			}
			else
			{
				SetSprite(disabledSprite);
			}
		}
	}

	private void OnHover(bool isOver)
	{
		if (isEnabled && target != null)
		{
			SetSprite((!isOver) ? normalSprite : hoverSprite);
		}
	}

	private void OnPress(bool pressed)
	{
		if (pressed)
		{
			SetSprite(pressedSprite);
		}
		else
		{
			UpdateImage();
		}
	}

	private void SetSprite(string sprite)
	{
		if (!(target.atlas == null) && target.atlas.GetSprite(sprite) != null)
		{
			target.spriteName = sprite;
			if (pixelSnap)
			{
				target.MakePixelPerfect();
			}
		}
	}
}

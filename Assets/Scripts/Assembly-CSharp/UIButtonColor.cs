using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("NGUI/Interaction/Button Color")]
public class UIButtonColor : UIWidgetContainer
{
	public GameObject tweenTarget;

	public Color hover = new Color(0.88235295f, 40f / 51f, 0.5882353f, 1f);

	public Color pressed = new Color(61f / 85f, 0.6392157f, 41f / 85f, 1f);

	public float duration = 0.2f;

	protected Color mColor;

	protected bool mStarted;

	protected UIWidget mWidget;

	public Color defaultColor
	{
		get
		{
			Awake();
			return mColor;
		}
		set
		{
			Awake();
			mColor = value;
		}
	}

	private void Awake()
	{
		if (!mStarted)
		{
			mStarted = true;
			Init();
		}
	}

	protected virtual void OnEnable()
	{
		if (mStarted)
		{
			OnHover(UICamera.IsHighlighted(base.gameObject));
		}
		if (UICamera.currentTouch != null)
		{
			if (UICamera.currentTouch.pressed == base.gameObject)
			{
				OnPress(true);
			}
			else if (UICamera.currentTouch.current == base.gameObject)
			{
				OnHover(true);
			}
		}
	}

	protected virtual void OnDisable()
	{
		if (mStarted && tweenTarget != null)
		{
			TweenColor component = tweenTarget.GetComponent<TweenColor>();
			if (component != null)
			{
				component.value = mColor;
				component.enabled = false;
			}
		}
	}

	protected void Init()
	{
		if (tweenTarget == null)
		{
			tweenTarget = base.gameObject;
		}
		mWidget = tweenTarget.GetComponent<UIWidget>();
		if (mWidget != null)
		{
			mColor = mWidget.color;
			return;
		}
		Renderer renderer = tweenTarget.GetComponent<Renderer>();
		if (renderer != null)
		{
			mColor = ((!Application.isPlaying) ? renderer.sharedMaterial.color : renderer.material.color);
			return;
		}
		Light light = tweenTarget.GetComponent<Light>();
		if (light != null)
		{
			mColor = light.color;
			return;
		}
		tweenTarget = null;
		mStarted = false;
	}

	protected virtual void OnPress(bool isPressed)
	{
		if (!base.enabled || UICamera.currentTouch == null)
		{
			return;
		}
		if (!mStarted)
		{
			Awake();
		}
		if (tweenTarget != null)
		{
			if (isPressed)
			{
				TweenColor.Begin(tweenTarget, duration, pressed);
			}
			else if (UICamera.currentTouch.current == base.gameObject && UICamera.currentScheme == UICamera.ControlScheme.Controller)
			{
				TweenColor.Begin(tweenTarget, duration, hover);
			}
			else
			{
				TweenColor.Begin(tweenTarget, duration, mColor);
			}
		}
	}

	protected virtual void OnHover(bool isOver)
	{
		if (base.enabled)
		{
			if (!mStarted)
			{
				Awake();
			}
			if (tweenTarget != null)
			{
				TweenColor.Begin(tweenTarget, duration, (!isOver) ? mColor : hover);
			}
		}
	}

	protected virtual void OnDragOver()
	{
		if (base.enabled)
		{
			if (!mStarted)
			{
				Awake();
			}
			if (tweenTarget != null)
			{
				TweenColor.Begin(tweenTarget, duration, pressed);
			}
		}
	}

	protected virtual void OnDragOut()
	{
		if (base.enabled)
		{
			if (!mStarted)
			{
				Awake();
			}
			if (tweenTarget != null)
			{
				TweenColor.Begin(tweenTarget, duration, mColor);
			}
		}
	}

	protected virtual void OnSelect(bool isSelected)
	{
		if (base.enabled && (!isSelected || UICamera.currentScheme == UICamera.ControlScheme.Controller) && tweenTarget != null)
		{
			OnHover(isSelected);
		}
	}
}

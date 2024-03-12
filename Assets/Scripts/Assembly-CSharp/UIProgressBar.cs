using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("NGUI/Interaction/NGUI Progress Bar")]
public class UIProgressBar : UIWidgetContainer
{
	public enum FillDirection
	{
		LeftToRight = 0,
		RightToLeft = 1,
		BottomToTop = 2,
		TopToBottom = 3
	}

	public delegate void OnDragFinished();

	public static UIProgressBar current;

	public OnDragFinished onDragFinished;

	public Transform thumb;

	[SerializeField]
	[HideInInspector]
	protected UIWidget mBG;

	[HideInInspector]
	[SerializeField]
	protected UIWidget mFG;

	[HideInInspector]
	[SerializeField]
	protected float mValue = 1f;

	[SerializeField]
	[HideInInspector]
	protected FillDirection mFill;

	protected Transform mTrans;

	protected bool mIsDirty;

	protected Camera mCam;

	protected float mOffset;

	public int numberOfSteps;

	public List<EventDelegate> onChange = new List<EventDelegate>();

	public Transform cachedTransform
	{
		get
		{
			if (mTrans == null)
			{
				mTrans = base.transform;
			}
			return mTrans;
		}
	}

	public Camera cachedCamera
	{
		get
		{
			if (mCam == null)
			{
				mCam = NGUITools.FindCameraForLayer(base.gameObject.layer);
			}
			return mCam;
		}
	}

	public UIWidget foregroundWidget
	{
		get
		{
			return mFG;
		}
		set
		{
			if (mFG != value)
			{
				mFG = value;
				mIsDirty = true;
			}
		}
	}

	public UIWidget backgroundWidget
	{
		get
		{
			return mBG;
		}
		set
		{
			if (mBG != value)
			{
				mBG = value;
				mIsDirty = true;
			}
		}
	}

	public FillDirection fillDirection
	{
		get
		{
			return mFill;
		}
		set
		{
			if (mFill != value)
			{
				mFill = value;
				ForceUpdate();
			}
		}
	}

	public float value
	{
		get
		{
			if (numberOfSteps > 1)
			{
				return Mathf.Round(mValue * (float)(numberOfSteps - 1)) / (float)(numberOfSteps - 1);
			}
			return mValue;
		}
		set
		{
			float num = Mathf.Clamp01(value);
			if (mValue == num)
			{
				return;
			}
			mValue = num;
			if (NGUITools.GetActive(this))
			{
				if (EventDelegate.IsValid(onChange))
				{
					current = this;
					EventDelegate.Execute(onChange);
					current = null;
				}
				ForceUpdate();
			}
		}
	}

	public float alpha
	{
		get
		{
			if (mFG != null)
			{
				return mFG.alpha;
			}
			if (mBG != null)
			{
				return mBG.alpha;
			}
			return 1f;
		}
		set
		{
			if (mFG != null)
			{
				mFG.alpha = value;
				if (mFG.GetComponent<Collider>() != null)
				{
					mFG.GetComponent<Collider>().enabled = mFG.alpha > 0.001f;
				}
			}
			if (mBG != null)
			{
				mBG.alpha = value;
				if (mBG.GetComponent<Collider>() != null)
				{
					mBG.GetComponent<Collider>().enabled = mBG.alpha > 0.001f;
				}
			}
			if (!(thumb != null))
			{
				return;
			}
			UIWidget component = thumb.GetComponent<UIWidget>();
			if (component != null)
			{
				component.alpha = value;
				if (component.GetComponent<Collider>() != null)
				{
					component.GetComponent<Collider>().enabled = component.alpha > 0.001f;
				}
			}
		}
	}

	protected bool isHorizontal
	{
		get
		{
			return mFill == FillDirection.LeftToRight || mFill == FillDirection.RightToLeft;
		}
	}

	protected bool isInverted
	{
		get
		{
			return mFill == FillDirection.RightToLeft || mFill == FillDirection.TopToBottom;
		}
	}

	protected void Start()
	{
		Upgrade();
		if (Application.isPlaying)
		{
			if (mFG == null)
			{
				base.enabled = false;
				return;
			}
			if (mBG != null)
			{
				mBG.autoResizeBoxCollider = true;
			}
			OnStart();
			if (onChange != null)
			{
				current = this;
				EventDelegate.Execute(onChange);
				current = null;
			}
		}
		ForceUpdate();
	}

	protected virtual void Upgrade()
	{
	}

	protected virtual void OnStart()
	{
	}

	protected void Update()
	{
		if (mIsDirty)
		{
			ForceUpdate();
		}
	}

	protected void OnValidate()
	{
		if (NGUITools.GetActive(this))
		{
			Upgrade();
			mIsDirty = true;
			float num = Mathf.Clamp01(mValue);
			if (mValue != num)
			{
				mValue = num;
			}
			if (numberOfSteps < 0)
			{
				numberOfSteps = 0;
			}
			else if (numberOfSteps > 20)
			{
				numberOfSteps = 20;
			}
			ForceUpdate();
		}
		else
		{
			float num2 = Mathf.Clamp01(mValue);
			if (mValue != num2)
			{
				mValue = num2;
			}
			if (numberOfSteps < 0)
			{
				numberOfSteps = 0;
			}
			else if (numberOfSteps > 20)
			{
				numberOfSteps = 20;
			}
		}
	}

	protected float ScreenToValue(Vector2 screenPos)
	{
		Transform transform = cachedTransform;
		Plane plane = new Plane(transform.rotation * Vector3.back, transform.position);
		Ray ray = cachedCamera.ScreenPointToRay(screenPos);
		float enter;
		if (!plane.Raycast(ray, out enter))
		{
			return value;
		}
		return LocalToValue(transform.InverseTransformPoint(ray.GetPoint(enter)));
	}

	protected virtual float LocalToValue(Vector2 localPos)
	{
		if (mFG != null)
		{
			Vector3[] localCorners = mFG.localCorners;
			Vector3 vector = localCorners[2] - localCorners[0];
			if (isHorizontal)
			{
				float num = (localPos.x - localCorners[0].x) / vector.x;
				return (!isInverted) ? num : (1f - num);
			}
			float num2 = (localPos.y - localCorners[0].y) / vector.y;
			return (!isInverted) ? num2 : (1f - num2);
		}
		return value;
	}

	public virtual void ForceUpdate()
	{
		mIsDirty = false;
		if (mFG != null)
		{
			UISprite uISprite = mFG as UISprite;
			if (isHorizontal)
			{
				if (uISprite != null && uISprite.type == UISprite.Type.Filled)
				{
					uISprite.fillDirection = UISprite.FillDirection.Horizontal;
					uISprite.invert = isInverted;
					uISprite.fillAmount = value;
				}
				else
				{
					mFG.drawRegion = ((!isInverted) ? new Vector4(0f, 0f, value, 1f) : new Vector4(1f - value, 0f, 1f, 1f));
				}
			}
			else if (uISprite != null && uISprite.type == UISprite.Type.Filled)
			{
				uISprite.fillDirection = UISprite.FillDirection.Vertical;
				uISprite.invert = isInverted;
				uISprite.fillAmount = value;
			}
			else
			{
				mFG.drawRegion = ((!isInverted) ? new Vector4(0f, 0f, 1f, value) : new Vector4(0f, 1f - value, 1f, 1f));
			}
		}
		if (thumb != null && (mFG != null || mBG != null))
		{
			Vector3[] array = ((!(mFG != null)) ? mBG.localCorners : mFG.localCorners);
			Vector4 vector = ((!(mFG != null)) ? mBG.border : mFG.border);
			array[0].x += vector.x;
			array[1].x += vector.x;
			array[2].x -= vector.z;
			array[3].x -= vector.z;
			array[0].y += vector.y;
			array[1].y -= vector.w;
			array[2].y -= vector.w;
			array[3].y += vector.y;
			Transform transform = ((!(mFG != null)) ? mBG.cachedTransform : mFG.cachedTransform);
			for (int i = 0; i < 4; i++)
			{
				array[i] = transform.TransformPoint(array[i]);
			}
			if (isHorizontal)
			{
				Vector3 from = Vector3.Lerp(array[0], array[1], 0.5f);
				Vector3 to = Vector3.Lerp(array[2], array[3], 0.5f);
				SetThumbPosition(Vector3.Lerp(from, to, (!isInverted) ? value : (1f - value)));
			}
			else
			{
				Vector3 from2 = Vector3.Lerp(array[0], array[3], 0.5f);
				Vector3 to2 = Vector3.Lerp(array[1], array[2], 0.5f);
				SetThumbPosition(Vector3.Lerp(from2, to2, (!isInverted) ? value : (1f - value)));
			}
		}
	}

	protected void SetThumbPosition(Vector3 worldPos)
	{
		Transform parent = thumb.parent;
		if (parent != null)
		{
			worldPos = parent.InverseTransformPoint(worldPos);
			worldPos.x = Mathf.Round(worldPos.x);
			worldPos.y = Mathf.Round(worldPos.y);
			worldPos.z = 0f;
			if (Vector3.Distance(thumb.localPosition, worldPos) > 0.001f)
			{
				thumb.localPosition = worldPos;
			}
		}
		else if (Vector3.Distance(thumb.position, worldPos) > 1E-05f)
		{
			thumb.position = worldPos;
		}
	}
}

using System;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("NGUI/UI/NGUI Panel")]
public class UIPanel : UIRect
{
	public enum DebugInfo
	{
		None = 0,
		Gizmos = 1,
		Geometry = 2
	}

	public enum RenderQueue
	{
		Automatic = 0,
		StartAt = 1,
		Explicit = 2
	}

	public delegate void OnChangeDelegate();

	public static BetterList<UIPanel> list = new BetterList<UIPanel>();

	public OnChangeDelegate onChange;

	public bool showInPanelTool = true;

	public bool generateNormals;

	public bool widgetsAreStatic;

	public bool cullWhileDragging;

	public bool alwaysOnScreen;

	public bool anchorOffset;

	public RenderQueue renderQueue;

	public int startingRenderQueue = 3000;

	[NonSerialized]
	public BetterList<UIWidget> widgets = new BetterList<UIWidget>();

	[NonSerialized]
	public BetterList<UIDrawCall> drawCalls = new BetterList<UIDrawCall>();

	[NonSerialized]
	public Matrix4x4 worldToLocal = Matrix4x4.identity;

	[SerializeField]
	[HideInInspector]
	private float mAlpha = 1f;

	[SerializeField]
	[HideInInspector]
	private UIDrawCall.Clipping mClipping;

	[HideInInspector]
	[SerializeField]
	private Vector4 mClipRange = new Vector4(0f, 0f, 300f, 200f);

	[HideInInspector]
	[SerializeField]
	private Vector2 mClipSoftness = new Vector2(4f, 4f);

	[SerializeField]
	[HideInInspector]
	private int mDepth;

	private bool mRebuild;

	private bool mResized;

	private static BetterList<Vector3> mVerts = new BetterList<Vector3>();

	private static BetterList<Vector3> mNorms = new BetterList<Vector3>();

	private static BetterList<Vector4> mTans = new BetterList<Vector4>();

	private static BetterList<Vector2> mUvs = new BetterList<Vector2>();

	private static BetterList<Color32> mCols = new BetterList<Color32>();

	private Camera mCam;

	[SerializeField]
	private Vector2 mClipOffset = Vector2.zero;

	private float mCullTime;

	private float mUpdateTime;

	private int mMatrixFrame = -1;

	private int mAlphaFrameID;

	private int mLayer = -1;

	private static float[] mTemp = new float[4];

	private Vector2 mMin = Vector2.zero;

	private Vector2 mMax = Vector2.zero;

	private bool mHalfPixelOffset;

	private bool mSortWidgets;

	private static Vector3[] mCorners = new Vector3[4];

	private static int mUpdateFrame = -1;

	private bool mForced;

	public static int nextUnusedDepth
	{
		get
		{
			int num = int.MinValue;
			for (int i = 0; i < list.size; i++)
			{
				num = Mathf.Max(num, list[i].depth);
			}
			return (num != int.MinValue) ? (num + 1) : 0;
		}
	}

	public override bool canBeAnchored
	{
		get
		{
			return mClipping != UIDrawCall.Clipping.None;
		}
	}

	public override float alpha
	{
		get
		{
			return mAlpha;
		}
		set
		{
			float num = Mathf.Clamp01(value);
			if (mAlpha != num)
			{
				mAlphaFrameID = -1;
				mResized = true;
				mAlpha = num;
				SetDirty();
			}
		}
	}

	public int depth
	{
		get
		{
			return mDepth;
		}
		set
		{
			if (mDepth != value)
			{
				mDepth = value;
				list.Sort(CompareFunc);
			}
		}
	}

	public float width
	{
		get
		{
			return GetViewSize().x;
		}
	}

	public float height
	{
		get
		{
			return GetViewSize().y;
		}
	}

	public bool halfPixelOffset
	{
		get
		{
			return mHalfPixelOffset;
		}
	}

	public bool usedForUI
	{
		get
		{
			return mCam != null && mCam.orthographic;
		}
	}

	public Vector3 drawCallOffset
	{
		get
		{
			if (mHalfPixelOffset && mCam != null && mCam.orthographic)
			{
				float num = 1f / GetWindowSize().y / mCam.orthographicSize;
				return new Vector3(0f - num, num);
			}
			return Vector3.zero;
		}
	}

	public UIDrawCall.Clipping clipping
	{
		get
		{
			return mClipping;
		}
		set
		{
			if (mClipping != value)
			{
				mResized = true;
				mClipping = value;
				mMatrixFrame = -1;
			}
		}
	}

	public bool clipsChildren
	{
		get
		{
			return mClipping == UIDrawCall.Clipping.AlphaClip || mClipping == UIDrawCall.Clipping.SoftClip;
		}
	}

	public Vector2 clipOffset
	{
		get
		{
			return mClipOffset;
		}
		set
		{
			if (Mathf.Abs(mClipOffset.x - value.x) > 0.001f || Mathf.Abs(mClipOffset.y - value.y) > 0.001f)
			{
				mResized = true;
				mCullTime = ((mCullTime != 0f) ? (RealTime.time + 0.15f) : 0.001f);
				mClipOffset = value;
				mMatrixFrame = -1;
			}
		}
	}

	[Obsolete("Use 'finalClipRegion' or 'baseClipRegion' instead")]
	public Vector4 clipRange
	{
		get
		{
			return baseClipRegion;
		}
		set
		{
			baseClipRegion = value;
		}
	}

	public Vector4 baseClipRegion
	{
		get
		{
			return mClipRange;
		}
		set
		{
			if (Mathf.Abs(mClipRange.x - value.x) > 0.49f || Mathf.Abs(mClipRange.y - value.y) > 0.49f || Mathf.Abs(mClipRange.z - value.z) > 0.49f || Mathf.Abs(mClipRange.w - value.w) > 0.49f)
			{
				mResized = true;
				mCullTime = ((mCullTime != 0f) ? (RealTime.time + 0.15f) : 0.001f);
				mClipRange = value;
				mMatrixFrame = -1;
				UIScrollView component = GetComponent<UIScrollView>();
				if (component != null)
				{
					component.UpdatePosition();
				}
			}
		}
	}

	public Vector4 finalClipRegion
	{
		get
		{
			Vector2 viewSize = GetViewSize();
			if (mClipping != 0)
			{
				return new Vector4(mClipRange.x + mClipOffset.x, mClipRange.y + mClipOffset.y, viewSize.x, viewSize.y);
			}
			return new Vector4(0f, 0f, viewSize.x, viewSize.y);
		}
	}

	public Vector2 clipSoftness
	{
		get
		{
			return mClipSoftness;
		}
		set
		{
			if (mClipSoftness != value)
			{
				mClipSoftness = value;
			}
		}
	}

	public override Vector3[] localCorners
	{
		get
		{
			if (mClipping == UIDrawCall.Clipping.None)
			{
				Vector2 viewSize = GetViewSize();
				float num = -0.5f * viewSize.x;
				float num2 = -0.5f * viewSize.y;
				float x = num + viewSize.x;
				float y = num2 + viewSize.y;
				Transform transform = ((!(mCam != null)) ? null : mCam.transform);
				if (transform != null)
				{
					mCorners[0] = transform.TransformPoint(num, num2, 0f);
					mCorners[1] = transform.TransformPoint(num, y, 0f);
					mCorners[2] = transform.TransformPoint(x, y, 0f);
					mCorners[3] = transform.TransformPoint(x, num2, 0f);
					transform = base.cachedTransform;
					for (int i = 0; i < 4; i++)
					{
						mCorners[i] = transform.InverseTransformPoint(mCorners[i]);
					}
				}
				else
				{
					mCorners[0] = new Vector3(num, num2);
					mCorners[1] = new Vector3(num, y);
					mCorners[2] = new Vector3(x, y);
					mCorners[3] = new Vector3(x, num2);
				}
			}
			else
			{
				float num3 = mClipOffset.x + mClipRange.x - 0.5f * mClipRange.z;
				float num4 = mClipOffset.y + mClipRange.y - 0.5f * mClipRange.w;
				float x2 = num3 + mClipRange.z;
				float y2 = num4 + mClipRange.w;
				mCorners[0] = new Vector3(num3, num4);
				mCorners[1] = new Vector3(num3, y2);
				mCorners[2] = new Vector3(x2, y2);
				mCorners[3] = new Vector3(x2, num4);
			}
			return mCorners;
		}
	}

	public override Vector3[] worldCorners
	{
		get
		{
			if (mClipping == UIDrawCall.Clipping.None)
			{
				Vector2 viewSize = GetViewSize();
				float num = -0.5f * viewSize.x;
				float num2 = -0.5f * viewSize.y;
				float x = num + viewSize.x;
				float y = num2 + viewSize.y;
				Transform transform = ((!(mCam != null)) ? null : mCam.transform);
				if (transform != null)
				{
					mCorners[0] = transform.TransformPoint(num, num2, 0f);
					mCorners[1] = transform.TransformPoint(num, y, 0f);
					mCorners[2] = transform.TransformPoint(x, y, 0f);
					mCorners[3] = transform.TransformPoint(x, num2, 0f);
				}
			}
			else
			{
				float num3 = mClipOffset.x + mClipRange.x - 0.5f * mClipRange.z;
				float num4 = mClipOffset.y + mClipRange.y - 0.5f * mClipRange.w;
				float x2 = num3 + mClipRange.z;
				float y2 = num4 + mClipRange.w;
				Transform transform2 = base.cachedTransform;
				mCorners[0] = transform2.TransformPoint(num3, num4, 0f);
				mCorners[1] = transform2.TransformPoint(num3, y2, 0f);
				mCorners[2] = transform2.TransformPoint(x2, y2, 0f);
				mCorners[3] = transform2.TransformPoint(x2, num4, 0f);
			}
			return mCorners;
		}
	}

	public static int CompareFunc(UIPanel a, UIPanel b)
	{
		if (a != b && a != null && b != null)
		{
			if (a.mDepth < b.mDepth)
			{
				return -1;
			}
			if (a.mDepth > b.mDepth)
			{
				return 1;
			}
			return (a.GetInstanceID() >= b.GetInstanceID()) ? 1 : (-1);
		}
		return 0;
	}

	public override Vector3[] GetSides(Transform relativeTo)
	{
		if (mClipping != 0 || anchorOffset)
		{
			Vector2 viewSize = GetViewSize();
			Vector2 vector = ((mClipping == UIDrawCall.Clipping.None) ? Vector2.zero : ((Vector2)mClipRange + mClipOffset));
			float num = vector.x - 0.5f * viewSize.x;
			float num2 = vector.y - 0.5f * viewSize.y;
			float num3 = num + viewSize.x;
			float num4 = num2 + viewSize.y;
			float x = (num + num3) * 0.5f;
			float y = (num2 + num4) * 0.5f;
			Matrix4x4 localToWorldMatrix = base.cachedTransform.localToWorldMatrix;
			mCorners[0] = localToWorldMatrix.MultiplyPoint3x4(new Vector3(num, y));
			mCorners[1] = localToWorldMatrix.MultiplyPoint3x4(new Vector3(x, num4));
			mCorners[2] = localToWorldMatrix.MultiplyPoint3x4(new Vector3(num3, y));
			mCorners[3] = localToWorldMatrix.MultiplyPoint3x4(new Vector3(x, num2));
			if (relativeTo != null)
			{
				for (int i = 0; i < 4; i++)
				{
					mCorners[i] = relativeTo.InverseTransformPoint(mCorners[i]);
				}
			}
			return mCorners;
		}
		return base.GetSides(relativeTo);
	}

	public override void Invalidate(bool includeChildren)
	{
		mAlphaFrameID = -1;
		base.Invalidate(includeChildren);
	}

	public override float CalculateFinalAlpha(int frameID)
	{
		if (mAlphaFrameID != frameID)
		{
			mAlphaFrameID = frameID;
			UIRect uIRect = base.parent;
			finalAlpha = ((!(base.parent != null)) ? mAlpha : (uIRect.CalculateFinalAlpha(frameID) * mAlpha));
		}
		return finalAlpha;
	}

	public bool IsVisible(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
	{
		UpdateTransformMatrix();
		a = worldToLocal.MultiplyPoint3x4(a);
		b = worldToLocal.MultiplyPoint3x4(b);
		c = worldToLocal.MultiplyPoint3x4(c);
		d = worldToLocal.MultiplyPoint3x4(d);
		mTemp[0] = a.x;
		mTemp[1] = b.x;
		mTemp[2] = c.x;
		mTemp[3] = d.x;
		float num = Mathf.Min(mTemp);
		float num2 = Mathf.Max(mTemp);
		mTemp[0] = a.y;
		mTemp[1] = b.y;
		mTemp[2] = c.y;
		mTemp[3] = d.y;
		float num3 = Mathf.Min(mTemp);
		float num4 = Mathf.Max(mTemp);
		if (num2 < mMin.x)
		{
			return false;
		}
		if (num4 < mMin.y)
		{
			return false;
		}
		if (num > mMax.x)
		{
			return false;
		}
		if (num3 > mMax.y)
		{
			return false;
		}
		return true;
	}

	public bool IsVisible(Vector3 worldPos)
	{
		if (mAlpha < 0.001f)
		{
			return false;
		}
		if (mClipping == UIDrawCall.Clipping.None || mClipping == UIDrawCall.Clipping.ConstrainButDontClip)
		{
			return true;
		}
		UpdateTransformMatrix();
		Vector3 vector = worldToLocal.MultiplyPoint3x4(worldPos);
		if (vector.x < mMin.x)
		{
			return false;
		}
		if (vector.y < mMin.y)
		{
			return false;
		}
		if (vector.x > mMax.x)
		{
			return false;
		}
		if (vector.y > mMax.y)
		{
			return false;
		}
		return true;
	}

	public bool IsVisible(UIWidget w)
	{
		if ((mClipping == UIDrawCall.Clipping.None || mClipping == UIDrawCall.Clipping.ConstrainButDontClip) && !w.hideIfOffScreen)
		{
			return true;
		}
		Vector3[] array = w.worldCorners;
		return IsVisible(array[0], array[1], array[2], array[3]);
	}

	[ContextMenu("Force Refresh")]
	public void RebuildAllDrawCalls()
	{
		mRebuild = true;
	}

	public void SetDirty()
	{
		for (int i = 0; i < drawCalls.size; i++)
		{
			drawCalls.buffer[i].isDirty = true;
		}
		Invalidate(true);
	}

	private void Awake()
	{
		mGo = base.gameObject;
		mTrans = base.transform;
		mHalfPixelOffset = Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.XBOX360 || Application.platform == RuntimePlatform.WindowsEditor;
		if (mHalfPixelOffset)
		{
			mHalfPixelOffset = SystemInfo.graphicsShaderLevel < 40;
		}
	}

	protected override void OnStart()
	{
		mLayer = mGo.layer;
		UICamera uICamera = UICamera.FindCameraForLayer(mLayer);
		mCam = ((!(uICamera != null)) ? NGUITools.FindCameraForLayer(mLayer) : uICamera.cachedCamera);
	}

	protected override void OnInit()
	{
		base.OnInit();
		if (base.GetComponent<Rigidbody>() == null)
		{
			Rigidbody rigidbody = base.gameObject.AddComponent<Rigidbody>();
			rigidbody.isKinematic = true;
			rigidbody.useGravity = false;
		}
		mRebuild = true;
		mAlphaFrameID = -1;
		mMatrixFrame = -1;
		list.Add(this);
		list.Sort(CompareFunc);
	}

	protected override void OnDisable()
	{
		for (int i = 0; i < drawCalls.size; i++)
		{
			UIDrawCall uIDrawCall = drawCalls.buffer[i];
			if (uIDrawCall != null)
			{
				UIDrawCall.Destroy(uIDrawCall);
			}
		}
		drawCalls.Clear();
		list.Remove(this);
		mAlphaFrameID = -1;
		mMatrixFrame = -1;
		if (list.size == 0)
		{
			UIDrawCall.ReleaseAll();
			mUpdateFrame = -1;
		}
		base.OnDisable();
	}

	private void UpdateTransformMatrix()
	{
		int frameCount = Time.frameCount;
		if (mMatrixFrame != frameCount)
		{
			mMatrixFrame = frameCount;
			worldToLocal = base.cachedTransform.worldToLocalMatrix;
			Vector2 vector = GetViewSize() * 0.5f;
			float num = mClipOffset.x + mClipRange.x;
			float num2 = mClipOffset.y + mClipRange.y;
			mMin.x = num - vector.x;
			mMin.y = num2 - vector.y;
			mMax.x = num + vector.x;
			mMax.y = num2 + vector.y;
		}
	}

	protected override void OnAnchor()
	{
		if (mClipping == UIDrawCall.Clipping.None)
		{
			return;
		}
		Transform transform = base.cachedTransform;
		Transform transform2 = transform.parent;
		Vector2 viewSize = GetViewSize();
		Vector2 vector = transform.localPosition;
		float num;
		float num2;
		float num3;
		float num4;
		if (leftAnchor.target == bottomAnchor.target && leftAnchor.target == rightAnchor.target && leftAnchor.target == topAnchor.target)
		{
			Vector3[] sides = leftAnchor.GetSides(transform2);
			if (sides != null)
			{
				num = NGUIMath.Lerp(sides[0].x, sides[2].x, leftAnchor.relative) + (float)leftAnchor.absolute;
				num2 = NGUIMath.Lerp(sides[0].x, sides[2].x, rightAnchor.relative) + (float)rightAnchor.absolute;
				num3 = NGUIMath.Lerp(sides[3].y, sides[1].y, bottomAnchor.relative) + (float)bottomAnchor.absolute;
				num4 = NGUIMath.Lerp(sides[3].y, sides[1].y, topAnchor.relative) + (float)topAnchor.absolute;
			}
			else
			{
				Vector2 vector2 = GetLocalPos(leftAnchor, transform2);
				num = vector2.x + (float)leftAnchor.absolute;
				num3 = vector2.y + (float)bottomAnchor.absolute;
				num2 = vector2.x + (float)rightAnchor.absolute;
				num4 = vector2.y + (float)topAnchor.absolute;
			}
		}
		else
		{
			if ((bool)leftAnchor.target)
			{
				Vector3[] sides2 = leftAnchor.GetSides(transform2);
				num = ((sides2 == null) ? (GetLocalPos(leftAnchor, transform2).x + (float)leftAnchor.absolute) : (NGUIMath.Lerp(sides2[0].x, sides2[2].x, leftAnchor.relative) + (float)leftAnchor.absolute));
			}
			else
			{
				num = mClipRange.x - 0.5f * viewSize.x;
			}
			if ((bool)rightAnchor.target)
			{
				Vector3[] sides3 = rightAnchor.GetSides(transform2);
				num2 = ((sides3 == null) ? (GetLocalPos(rightAnchor, transform2).x + (float)rightAnchor.absolute) : (NGUIMath.Lerp(sides3[0].x, sides3[2].x, rightAnchor.relative) + (float)rightAnchor.absolute));
			}
			else
			{
				num2 = mClipRange.x + 0.5f * viewSize.x;
			}
			if ((bool)bottomAnchor.target)
			{
				Vector3[] sides4 = bottomAnchor.GetSides(transform2);
				num3 = ((sides4 == null) ? (GetLocalPos(bottomAnchor, transform2).y + (float)bottomAnchor.absolute) : (NGUIMath.Lerp(sides4[3].y, sides4[1].y, bottomAnchor.relative) + (float)bottomAnchor.absolute));
			}
			else
			{
				num3 = mClipRange.y - 0.5f * viewSize.y;
			}
			if ((bool)topAnchor.target)
			{
				Vector3[] sides5 = topAnchor.GetSides(transform2);
				num4 = ((sides5 == null) ? (GetLocalPos(topAnchor, transform2).y + (float)topAnchor.absolute) : (NGUIMath.Lerp(sides5[3].y, sides5[1].y, topAnchor.relative) + (float)topAnchor.absolute));
			}
			else
			{
				num4 = mClipRange.y + 0.5f * viewSize.y;
			}
		}
		num -= vector.x + mClipOffset.x;
		num2 -= vector.x + mClipOffset.x;
		num3 -= vector.y + mClipOffset.y;
		num4 -= vector.y + mClipOffset.y;
		float x = Mathf.Lerp(num, num2, 0.5f);
		float y = Mathf.Lerp(num3, num4, 0.5f);
		float num5 = num2 - num;
		float num6 = num4 - num3;
		float num7 = Mathf.Max(20f, mClipSoftness.x);
		float num8 = Mathf.Max(20f, mClipSoftness.y);
		if (num5 < num7)
		{
			num5 = num7;
		}
		if (num6 < num8)
		{
			num6 = num8;
		}
		baseClipRegion = new Vector4(x, y, num5, num6);
	}

	private void LateUpdate()
	{
		if (mUpdateFrame == Time.frameCount)
		{
			return;
		}
		mUpdateFrame = Time.frameCount;
		for (int i = 0; i < list.size; i++)
		{
			list[i].UpdateSelf();
		}
		int num = 3000;
		for (int j = 0; j < list.size; j++)
		{
			UIPanel uIPanel = list.buffer[j];
			if (uIPanel.renderQueue == RenderQueue.Automatic)
			{
				uIPanel.startingRenderQueue = num;
				uIPanel.UpdateDrawCalls();
				num += uIPanel.drawCalls.size;
			}
			else if (uIPanel.renderQueue == RenderQueue.StartAt)
			{
				uIPanel.UpdateDrawCalls();
				if (uIPanel.drawCalls.size != 0)
				{
					num = Mathf.Max(num, uIPanel.startingRenderQueue + uIPanel.drawCalls.size);
				}
			}
			else
			{
				uIPanel.UpdateDrawCalls();
				if (uIPanel.drawCalls.size != 0)
				{
					num = Mathf.Max(num, uIPanel.startingRenderQueue + 1);
				}
			}
		}
	}

	private void UpdateSelf()
	{
		mUpdateTime = RealTime.time;
		UpdateTransformMatrix();
		UpdateLayers();
		UpdateWidgets();
		if (mRebuild)
		{
			mRebuild = false;
			FillAllDrawCalls();
			return;
		}
		int num = 0;
		while (num < drawCalls.size)
		{
			UIDrawCall uIDrawCall = drawCalls.buffer[num];
			if (uIDrawCall.isDirty && !FillDrawCall(uIDrawCall))
			{
				UIDrawCall.Destroy(uIDrawCall);
				drawCalls.RemoveAt(num);
			}
			else
			{
				num++;
			}
		}
	}

	public void SortWidgets()
	{
		mSortWidgets = false;
		widgets.Sort(UIWidget.PanelCompareFunc);
	}

	private void FillAllDrawCalls()
	{
		for (int i = 0; i < drawCalls.size; i++)
		{
			UIDrawCall.Destroy(drawCalls.buffer[i]);
		}
		drawCalls.Clear();
		Material material = null;
		Texture texture = null;
		Shader shader = null;
		UIDrawCall uIDrawCall = null;
		if (mSortWidgets)
		{
			SortWidgets();
		}
		for (int j = 0; j < widgets.size; j++)
		{
			UIWidget uIWidget = widgets.buffer[j];
			if (uIWidget.isVisible && uIWidget.hasVertices)
			{
				Material material2 = uIWidget.material;
				Texture mainTexture = uIWidget.mainTexture;
				Shader shader2 = uIWidget.shader;
				if (material != material2 || texture != mainTexture || shader != shader2)
				{
					if (mVerts.size != 0)
					{
						SubmitDrawCall(uIDrawCall);
						uIDrawCall = null;
					}
					material = material2;
					texture = mainTexture;
					shader = shader2;
				}
				if (!(material != null) && !(shader != null) && !(texture != null))
				{
					continue;
				}
				if (uIDrawCall == null)
				{
					uIDrawCall = UIDrawCall.Create(this, material, texture, shader);
					uIDrawCall.depthStart = uIWidget.depth;
					uIDrawCall.depthEnd = uIDrawCall.depthStart;
					uIDrawCall.panel = this;
				}
				else
				{
					int num = uIWidget.depth;
					if (num < uIDrawCall.depthStart)
					{
						uIDrawCall.depthStart = num;
					}
					if (num > uIDrawCall.depthEnd)
					{
						uIDrawCall.depthEnd = num;
					}
				}
				uIWidget.drawCall = uIDrawCall;
				if (generateNormals)
				{
					uIWidget.WriteToBuffers(mVerts, mUvs, mCols, mNorms, mTans);
				}
				else
				{
					uIWidget.WriteToBuffers(mVerts, mUvs, mCols, null, null);
				}
			}
			else
			{
				uIWidget.drawCall = null;
			}
		}
		if (mVerts.size != 0)
		{
			SubmitDrawCall(uIDrawCall);
		}
	}

	private void SubmitDrawCall(UIDrawCall dc)
	{
		drawCalls.Add(dc);
		dc.Set(mVerts, (!generateNormals) ? null : mNorms, (!generateNormals) ? null : mTans, mUvs, mCols);
		mVerts.Clear();
		mNorms.Clear();
		mTans.Clear();
		mUvs.Clear();
		mCols.Clear();
	}

	private bool FillDrawCall(UIDrawCall dc)
	{
		if (dc != null)
		{
			dc.isDirty = false;
			int num = 0;
			while (num < widgets.size)
			{
				UIWidget uIWidget = widgets[num];
				if (uIWidget == null)
				{
					widgets.RemoveAt(num);
					continue;
				}
				if (uIWidget.drawCall == dc)
				{
					if (uIWidget.isVisible && uIWidget.hasVertices)
					{
						if (generateNormals)
						{
							uIWidget.WriteToBuffers(mVerts, mUvs, mCols, mNorms, mTans);
						}
						else
						{
							uIWidget.WriteToBuffers(mVerts, mUvs, mCols, null, null);
						}
					}
					else
					{
						uIWidget.drawCall = null;
					}
				}
				num++;
			}
			if (mVerts.size != 0)
			{
				dc.Set(mVerts, (!generateNormals) ? null : mNorms, (!generateNormals) ? null : mTans, mUvs, mCols);
				mVerts.Clear();
				mNorms.Clear();
				mTans.Clear();
				mUvs.Clear();
				mCols.Clear();
				return true;
			}
		}
		return false;
	}

	private void UpdateDrawCalls()
	{
		Transform transform = base.cachedTransform;
		Vector4 vector = Vector4.zero;
		bool flag = usedForUI;
		if (clipping != 0)
		{
			Vector4 vector2 = finalClipRegion;
			vector = new Vector4(vector2.x, vector2.y, vector2.z * 0.5f, vector2.w * 0.5f);
		}
		if (vector.z == 0f)
		{
			vector.z = (float)Screen.width * 0.5f;
		}
		if (vector.w == 0f)
		{
			vector.w = (float)Screen.height * 0.5f;
		}
		if (halfPixelOffset)
		{
			vector.x -= 0.5f;
			vector.y += 0.5f;
		}
		Vector3 position;
		if (flag)
		{
			Transform transform2 = base.cachedTransform.parent;
			position = base.cachedTransform.localPosition;
			if (transform2 != null)
			{
				float num = Mathf.Round(position.x);
				float num2 = Mathf.Round(position.y);
				vector.x += position.x - num;
				vector.y += position.y - num2;
				position.x = num;
				position.y = num2;
				position = transform2.TransformPoint(position);
			}
			position += drawCallOffset;
		}
		else
		{
			position = transform.position;
		}
		Quaternion rotation = transform.rotation;
		Vector3 lossyScale = transform.lossyScale;
		for (int i = 0; i < drawCalls.size; i++)
		{
			UIDrawCall uIDrawCall = drawCalls.buffer[i];
			Transform transform3 = uIDrawCall.cachedTransform;
			transform3.position = position;
			transform3.rotation = rotation;
			transform3.localScale = lossyScale;
			uIDrawCall.renderQueue = ((renderQueue != RenderQueue.Explicit) ? (startingRenderQueue + i) : startingRenderQueue);
			uIDrawCall.clipping = clipping;
			uIDrawCall.clipRange = vector;
			uIDrawCall.clipSoftness = mClipSoftness;
			uIDrawCall.alwaysOnScreen = alwaysOnScreen && (mClipping == UIDrawCall.Clipping.None || mClipping == UIDrawCall.Clipping.ConstrainButDontClip);
		}
	}

	private void UpdateLayers()
	{
		if (mLayer != base.cachedGameObject.layer)
		{
			mLayer = mGo.layer;
			UICamera uICamera = UICamera.FindCameraForLayer(mLayer);
			mCam = ((!(uICamera != null)) ? NGUITools.FindCameraForLayer(mLayer) : uICamera.cachedCamera);
			NGUITools.SetChildLayer(base.cachedTransform, mLayer);
			for (int i = 0; i < drawCalls.size; i++)
			{
				drawCalls.buffer[i].gameObject.layer = mLayer;
			}
		}
	}

	private void UpdateWidgets()
	{
		bool flag = !cullWhileDragging && mCullTime > mUpdateTime;
		bool flag2 = false;
		if (mForced != flag)
		{
			mForced = flag;
			mResized = true;
		}
		bool flag3 = clipsChildren;
		int i = 0;
		for (int size = widgets.size; i < size; i++)
		{
			UIWidget uIWidget = widgets.buffer[i];
			if (!(uIWidget.panel == this) || !uIWidget.enabled)
			{
				continue;
			}
			int frameCount = Time.frameCount;
			if (uIWidget.UpdateTransform(frameCount) || mResized)
			{
				bool visibleByAlpha = flag || uIWidget.CalculateCumulativeAlpha(frameCount) > 0.001f;
				uIWidget.UpdateVisibility(visibleByAlpha, flag || (!flag3 && !uIWidget.hideIfOffScreen) || IsVisible(uIWidget));
			}
			if (!uIWidget.UpdateGeometry(frameCount))
			{
				continue;
			}
			flag2 = true;
			if (!mRebuild)
			{
				if (uIWidget.drawCall != null)
				{
					uIWidget.drawCall.isDirty = true;
				}
				else
				{
					FindDrawCall(uIWidget);
				}
			}
		}
		if (flag2 && onChange != null)
		{
			onChange();
		}
		mResized = false;
	}

	public UIDrawCall FindDrawCall(UIWidget w)
	{
		Material material = w.material;
		Texture mainTexture = w.mainTexture;
		int num = w.depth;
		for (int i = 0; i < drawCalls.size; i++)
		{
			UIDrawCall uIDrawCall = drawCalls.buffer[i];
			int num2 = ((i != 0) ? (drawCalls.buffer[i - 1].depthEnd + 1) : int.MinValue);
			int num3 = ((i + 1 != drawCalls.size) ? (drawCalls.buffer[i + 1].depthStart - 1) : int.MaxValue);
			if (num2 > num || num3 < num)
			{
				continue;
			}
			if (uIDrawCall.baseMaterial == material && uIDrawCall.mainTexture == mainTexture)
			{
				if (w.isVisible)
				{
					w.drawCall = uIDrawCall;
					if (w.hasVertices)
					{
						uIDrawCall.isDirty = true;
					}
					return uIDrawCall;
				}
			}
			else
			{
				mRebuild = true;
			}
			return null;
		}
		mRebuild = true;
		return null;
	}

	public void AddWidget(UIWidget w)
	{
		if (widgets.size == 0)
		{
			widgets.Add(w);
		}
		else if (mSortWidgets)
		{
			widgets.Add(w);
			SortWidgets();
		}
		else if (UIWidget.PanelCompareFunc(w, widgets[0]) == -1)
		{
			widgets.Insert(0, w);
		}
		else
		{
			int num = widgets.size;
			while (num > 0)
			{
				if (UIWidget.PanelCompareFunc(w, widgets[--num]) == -1)
				{
					continue;
				}
				widgets.Insert(num + 1, w);
				break;
			}
		}
		FindDrawCall(w);
	}

	public void RemoveWidget(UIWidget w)
	{
		if (widgets.Remove(w) && w.drawCall != null)
		{
			int num = w.depth;
			if (num == w.drawCall.depthStart || num == w.drawCall.depthEnd)
			{
				mRebuild = true;
			}
			w.drawCall.isDirty = true;
			w.drawCall = null;
		}
	}

	public void Refresh()
	{
		mRebuild = true;
		if (list.size > 0)
		{
			list[0].LateUpdate();
		}
	}

	public virtual Vector3 CalculateConstrainOffset(Vector2 min, Vector2 max)
	{
		Vector4 vector = finalClipRegion;
		float num = vector.z * 0.5f;
		float num2 = vector.w * 0.5f;
		Vector2 minRect = new Vector2(min.x, min.y);
		Vector2 maxRect = new Vector2(max.x, max.y);
		Vector2 minArea = new Vector2(vector.x - num, vector.y - num2);
		Vector2 maxArea = new Vector2(vector.x + num, vector.y + num2);
		if (clipping == UIDrawCall.Clipping.SoftClip)
		{
			minArea.x += clipSoftness.x;
			minArea.y += clipSoftness.y;
			maxArea.x -= clipSoftness.x;
			maxArea.y -= clipSoftness.y;
		}
		return NGUIMath.ConstrainRect(minRect, maxRect, minArea, maxArea);
	}

	public bool ConstrainTargetToBounds(Transform target, ref Bounds targetBounds, bool immediate)
	{
		Vector3 vector = CalculateConstrainOffset(targetBounds.min, targetBounds.max);
		if (vector.magnitude > 0f)
		{
			if (immediate)
			{
				target.localPosition += vector;
				targetBounds.center += vector;
				SpringPosition component = target.GetComponent<SpringPosition>();
				if (component != null)
				{
					component.enabled = false;
				}
			}
			else
			{
				SpringPosition springPosition = SpringPosition.Begin(target.gameObject, target.localPosition + vector, 13f);
				springPosition.ignoreTimeScale = true;
				springPosition.worldSpace = false;
			}
			return true;
		}
		return false;
	}

	public bool ConstrainTargetToBounds(Transform target, bool immediate)
	{
		Bounds targetBounds = NGUIMath.CalculateRelativeWidgetBounds(base.cachedTransform, target);
		return ConstrainTargetToBounds(target, ref targetBounds, immediate);
	}

	public static UIPanel Find(Transform trans)
	{
		return Find(trans, false, -1);
	}

	public static UIPanel Find(Transform trans, bool createIfMissing)
	{
		return Find(trans, createIfMissing, -1);
	}

	public static UIPanel Find(Transform trans, bool createIfMissing, int layer)
	{
		UIPanel uIPanel = null;
		while (uIPanel == null && trans != null)
		{
			uIPanel = trans.GetComponent<UIPanel>();
			if (uIPanel != null)
			{
				return uIPanel;
			}
			if (trans.parent == null)
			{
				break;
			}
			trans = trans.parent;
		}
		return (!createIfMissing) ? null : NGUITools.CreateUI(trans, false, layer);
	}

	private Vector2 GetWindowSize()
	{
		UIRoot uIRoot = base.root;
		Vector2 vector = new Vector2(Screen.width, Screen.height);
		if (uIRoot != null)
		{
			return vector * uIRoot.GetPixelSizeAdjustment(Screen.height);
		}
		return vector;
	}

	public Vector2 GetViewSize()
	{
		bool flag = mClipping != UIDrawCall.Clipping.None;
		Vector2 result = ((!flag) ? new Vector2(Screen.width, Screen.height) : new Vector2(mClipRange.z, mClipRange.w));
		if (!flag)
		{
			UIRoot uIRoot = base.root;
			if (uIRoot != null)
			{
				result *= uIRoot.GetPixelSizeAdjustment(Screen.height);
			}
		}
		return result;
	}
}

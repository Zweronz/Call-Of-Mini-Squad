using System;
using UnityEngine;

[AddComponentMenu("NGUI/UI/NGUI Sprite")]
[ExecuteInEditMode]
public class UISprite : UIWidget
{
	public enum Type
	{
		Simple = 0,
		Sliced = 1,
		Tiled = 2,
		Filled = 3,
		Advanced = 4
	}

	public enum FillDirection
	{
		Horizontal = 0,
		Vertical = 1,
		Radial90 = 2,
		Radial180 = 3,
		Radial360 = 4
	}

	public enum AdvancedType
	{
		Invisible = 0,
		Sliced = 1,
		Tiled = 2
	}

	[SerializeField]
	[HideInInspector]
	private UIAtlas mAtlas;

	[HideInInspector]
	[SerializeField]
	private string mSpriteName;

	[SerializeField]
	[HideInInspector]
	private Type mType;

	[SerializeField]
	[HideInInspector]
	private FillDirection mFillDirection = FillDirection.Radial360;

	[SerializeField]
	[HideInInspector]
	[Range(0f, 1f)]
	private float mFillAmount = 1f;

	[SerializeField]
	[HideInInspector]
	private bool mInvert;

	[HideInInspector]
	[SerializeField]
	private bool mFillCenter = true;

	protected UISpriteData mSprite;

	protected Rect mInnerUV = default(Rect);

	protected Rect mOuterUV = default(Rect);

	private bool mSpriteSet;

	public AdvancedType centerType = AdvancedType.Sliced;

	public AdvancedType leftType = AdvancedType.Sliced;

	public AdvancedType rightType = AdvancedType.Sliced;

	public AdvancedType bottomType = AdvancedType.Sliced;

	public AdvancedType topType = AdvancedType.Sliced;

	private static Vector2[] mTempPos = new Vector2[4];

	private static Vector2[] mTempUVs = new Vector2[4];

	public virtual Type type
	{
		get
		{
			return mType;
		}
		set
		{
			if (mType != value)
			{
				mType = value;
				MarkAsChanged();
			}
		}
	}

	public override Material material
	{
		get
		{
			return (!(mAtlas != null)) ? null : mAtlas.spriteMaterial;
		}
	}

	public UIAtlas atlas
	{
		get
		{
			return mAtlas;
		}
		set
		{
			if (mAtlas != value)
			{
				RemoveFromPanel();
				mAtlas = value;
				mSpriteSet = false;
				mSprite = null;
				if (string.IsNullOrEmpty(mSpriteName) && mAtlas != null && mAtlas.spriteList.Count > 0)
				{
					SetAtlasSprite(mAtlas.spriteList[0]);
					mSpriteName = mSprite.name;
				}
				if (!string.IsNullOrEmpty(mSpriteName))
				{
					string text = mSpriteName;
					mSpriteName = string.Empty;
					spriteName = text;
					MarkAsChanged();
				}
			}
		}
	}

	public string spriteName
	{
		get
		{
			return mSpriteName;
		}
		set
		{
			if (string.IsNullOrEmpty(value))
			{
				if (!string.IsNullOrEmpty(mSpriteName))
				{
					mSpriteName = string.Empty;
					mSprite = null;
					mChanged = true;
					mSpriteSet = false;
				}
			}
			else if (mSpriteName != value)
			{
				mSpriteName = value;
				mSprite = null;
				mChanged = true;
				mSpriteSet = false;
			}
		}
	}

	public bool isValid
	{
		get
		{
			return GetAtlasSprite() != null;
		}
	}

	[Obsolete("Use 'centerType' instead")]
	public bool fillCenter
	{
		get
		{
			return centerType != AdvancedType.Invisible;
		}
		set
		{
			if (value != (centerType != AdvancedType.Invisible))
			{
				centerType = (value ? AdvancedType.Sliced : AdvancedType.Invisible);
				MarkAsChanged();
			}
		}
	}

	public FillDirection fillDirection
	{
		get
		{
			return mFillDirection;
		}
		set
		{
			if (mFillDirection != value)
			{
				mFillDirection = value;
				mChanged = true;
			}
		}
	}

	public float fillAmount
	{
		get
		{
			return mFillAmount;
		}
		set
		{
			float num = Mathf.Clamp01(value);
			if (mFillAmount != num)
			{
				mFillAmount = num;
				mChanged = true;
			}
		}
	}

	public bool invert
	{
		get
		{
			return mInvert;
		}
		set
		{
			if (mInvert != value)
			{
				mInvert = value;
				mChanged = true;
			}
		}
	}

	public override Vector4 border
	{
		get
		{
			if (type == Type.Sliced || type == Type.Advanced)
			{
				UISpriteData atlasSprite = GetAtlasSprite();
				if (atlasSprite == null)
				{
					return Vector2.zero;
				}
				return new Vector4(atlasSprite.borderLeft, atlasSprite.borderBottom, atlasSprite.borderRight, atlasSprite.borderTop);
			}
			return base.border;
		}
	}

	public override int minWidth
	{
		get
		{
			if (type == Type.Sliced || type == Type.Advanced)
			{
				Vector4 vector = border;
				if (atlas != null)
				{
					vector *= atlas.pixelSize;
				}
				int num = Mathf.RoundToInt(vector.x + vector.z);
				UISpriteData atlasSprite = GetAtlasSprite();
				if (atlasSprite != null)
				{
					num += atlasSprite.paddingLeft + atlasSprite.paddingRight;
				}
				return Mathf.Max(base.minWidth, ((num & 1) != 1) ? num : (num + 1));
			}
			return base.minWidth;
		}
	}

	public override int minHeight
	{
		get
		{
			if (type == Type.Sliced || type == Type.Advanced)
			{
				Vector4 vector = border;
				if (atlas != null)
				{
					vector *= atlas.pixelSize;
				}
				int num = Mathf.RoundToInt(vector.y + vector.w);
				UISpriteData atlasSprite = GetAtlasSprite();
				if (atlasSprite != null)
				{
					num += atlasSprite.paddingTop + atlasSprite.paddingBottom;
				}
				return Mathf.Max(base.minHeight, ((num & 1) != 1) ? num : (num + 1));
			}
			return base.minHeight;
		}
	}

	public override Vector4 drawingDimensions
	{
		get
		{
			Vector2 vector = base.pivotOffset;
			float num = (0f - vector.x) * (float)mWidth;
			float num2 = (0f - vector.y) * (float)mHeight;
			float num3 = num + (float)mWidth;
			float num4 = num2 + (float)mHeight;
			if (GetAtlasSprite() != null)
			{
				int paddingLeft = mSprite.paddingLeft;
				int paddingBottom = mSprite.paddingBottom;
				int num5 = mSprite.paddingRight;
				int num6 = mSprite.paddingTop;
				int num7 = mSprite.width + paddingLeft + num5;
				int num8 = mSprite.height + paddingBottom + num6;
				if (num7 > 0 && num8 > 0 && (mType == Type.Simple || mType == Type.Filled))
				{
					if (((uint)num7 & (true ? 1u : 0u)) != 0)
					{
						num5++;
					}
					if (((uint)num8 & (true ? 1u : 0u)) != 0)
					{
						num6++;
					}
					float num9 = 1f / (float)num7 * (float)mWidth;
					float num10 = 1f / (float)num8 * (float)mHeight;
					num += (float)paddingLeft * num9;
					num3 -= (float)num5 * num9;
					num2 += (float)paddingBottom * num10;
					num4 -= (float)num6 * num10;
				}
				else
				{
					num += (float)paddingLeft;
					num3 -= (float)num5;
					num2 += (float)paddingBottom;
					num4 -= (float)num6;
				}
			}
			Vector4 vector2 = border * atlas.pixelSize;
			float num11 = vector2.x + vector2.z;
			float num12 = vector2.y + vector2.w;
			float x = Mathf.Lerp(num, num3 - num11, mDrawRegion.x);
			float y = Mathf.Lerp(num2, num4 - num12, mDrawRegion.y);
			float z = Mathf.Lerp(num + num11, num3, mDrawRegion.z);
			float w = Mathf.Lerp(num2 + num12, num4, mDrawRegion.w);
			return new Vector4(x, y, z, w);
		}
	}

	public UISpriteData GetAtlasSprite()
	{
		if (!mSpriteSet)
		{
			mSprite = null;
		}
		if (mSprite == null && mAtlas != null)
		{
			if (!string.IsNullOrEmpty(mSpriteName))
			{
				UISpriteData sprite = mAtlas.GetSprite(mSpriteName);
				if (sprite == null)
				{
					return null;
				}
				SetAtlasSprite(sprite);
			}
			if (mSprite == null && mAtlas.spriteList.Count > 0)
			{
				UISpriteData uISpriteData = mAtlas.spriteList[0];
				if (uISpriteData == null)
				{
					return null;
				}
				SetAtlasSprite(uISpriteData);
				if (mSprite == null)
				{
					return null;
				}
				mSpriteName = mSprite.name;
			}
		}
		return mSprite;
	}

	protected void SetAtlasSprite(UISpriteData sp)
	{
		mChanged = true;
		mSpriteSet = true;
		if (sp != null)
		{
			mSprite = sp;
			mSpriteName = mSprite.name;
		}
		else
		{
			mSpriteName = ((mSprite == null) ? string.Empty : mSprite.name);
			mSprite = sp;
		}
	}

	public override void MakePixelPerfect()
	{
		if (!isValid)
		{
			return;
		}
		base.MakePixelPerfect();
		Type type = this.type;
		if (type != 0 && type != Type.Filled)
		{
			return;
		}
		Texture texture = mainTexture;
		UISpriteData atlasSprite = GetAtlasSprite();
		if (texture != null && atlasSprite != null)
		{
			int num = Mathf.RoundToInt(atlas.pixelSize * (float)(atlasSprite.width + atlasSprite.paddingLeft + atlasSprite.paddingRight));
			int num2 = Mathf.RoundToInt(atlas.pixelSize * (float)(atlasSprite.height + atlasSprite.paddingTop + atlasSprite.paddingBottom));
			if ((num & 1) == 1)
			{
				num++;
			}
			if ((num2 & 1) == 1)
			{
				num2++;
			}
			base.width = num;
			base.height = num2;
		}
	}

	protected override void OnInit()
	{
		if (!mFillCenter)
		{
			mFillCenter = true;
			centerType = AdvancedType.Invisible;
		}
		base.OnInit();
	}

	protected override void OnUpdate()
	{
		base.OnUpdate();
		if (mChanged || !mSpriteSet)
		{
			mSpriteSet = true;
			mSprite = null;
			mChanged = true;
		}
	}

	public override void OnFill(BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols)
	{
		Texture texture = mainTexture;
		if (texture != null)
		{
			if (mSprite == null)
			{
				mSprite = atlas.GetSprite(spriteName);
			}
			if (mSprite == null)
			{
				return;
			}
			mOuterUV.Set(mSprite.x, mSprite.y, mSprite.width, mSprite.height);
			mInnerUV.Set(mSprite.x + mSprite.borderLeft, mSprite.y + mSprite.borderTop, mSprite.width - mSprite.borderLeft - mSprite.borderRight, mSprite.height - mSprite.borderBottom - mSprite.borderTop);
			mOuterUV = NGUIMath.ConvertToTexCoords(mOuterUV, texture.width, texture.height);
			mInnerUV = NGUIMath.ConvertToTexCoords(mInnerUV, texture.width, texture.height);
		}
		switch (type)
		{
		case Type.Simple:
			SimpleFill(verts, uvs, cols);
			break;
		case Type.Sliced:
			SlicedFill(verts, uvs, cols);
			break;
		case Type.Filled:
			FilledFill(verts, uvs, cols);
			break;
		case Type.Tiled:
			TiledFill(verts, uvs, cols);
			break;
		case Type.Advanced:
			AdvancedFill(verts, uvs, cols);
			break;
		}
	}

	protected void SimpleFill(BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols)
	{
		Vector2 item = new Vector2(mOuterUV.xMin, mOuterUV.yMin);
		Vector2 item2 = new Vector2(mOuterUV.xMax, mOuterUV.yMax);
		Vector4 vector = drawingDimensions;
		verts.Add(new Vector3(vector.x, vector.y));
		verts.Add(new Vector3(vector.x, vector.w));
		verts.Add(new Vector3(vector.z, vector.w));
		verts.Add(new Vector3(vector.z, vector.y));
		uvs.Add(item);
		uvs.Add(new Vector2(item.x, item2.y));
		uvs.Add(item2);
		uvs.Add(new Vector2(item2.x, item.y));
		Color color = base.color;
		color.a = finalAlpha;
		Color32 item3 = ((!atlas.premultipliedAlpha) ? color : NGUITools.ApplyPMA(color));
		cols.Add(item3);
		cols.Add(item3);
		cols.Add(item3);
		cols.Add(item3);
	}

	protected void SlicedFill(BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols)
	{
		if (!mSprite.hasBorder)
		{
			SimpleFill(verts, uvs, cols);
			return;
		}
		Vector4 vector = drawingDimensions;
		Vector4 vector2 = border * atlas.pixelSize;
		mTempPos[0].x = vector.x;
		mTempPos[0].y = vector.y;
		mTempPos[3].x = vector.z;
		mTempPos[3].y = vector.w;
		mTempPos[1].x = mTempPos[0].x + vector2.x;
		mTempPos[1].y = mTempPos[0].y + vector2.y;
		mTempPos[2].x = mTempPos[3].x - vector2.z;
		mTempPos[2].y = mTempPos[3].y - vector2.w;
		mTempUVs[0] = new Vector2(mOuterUV.xMin, mOuterUV.yMin);
		mTempUVs[1] = new Vector2(mInnerUV.xMin, mInnerUV.yMin);
		mTempUVs[2] = new Vector2(mInnerUV.xMax, mInnerUV.yMax);
		mTempUVs[3] = new Vector2(mOuterUV.xMax, mOuterUV.yMax);
		Color color = base.color;
		color.a = finalAlpha;
		Color32 item = ((!atlas.premultipliedAlpha) ? color : NGUITools.ApplyPMA(color));
		for (int i = 0; i < 3; i++)
		{
			int num = i + 1;
			for (int j = 0; j < 3; j++)
			{
				if (centerType != 0 || i != 1 || j != 1)
				{
					int num2 = j + 1;
					verts.Add(new Vector3(mTempPos[i].x, mTempPos[j].y));
					verts.Add(new Vector3(mTempPos[i].x, mTempPos[num2].y));
					verts.Add(new Vector3(mTempPos[num].x, mTempPos[num2].y));
					verts.Add(new Vector3(mTempPos[num].x, mTempPos[j].y));
					uvs.Add(new Vector2(mTempUVs[i].x, mTempUVs[j].y));
					uvs.Add(new Vector2(mTempUVs[i].x, mTempUVs[num2].y));
					uvs.Add(new Vector2(mTempUVs[num].x, mTempUVs[num2].y));
					uvs.Add(new Vector2(mTempUVs[num].x, mTempUVs[j].y));
					cols.Add(item);
					cols.Add(item);
					cols.Add(item);
					cols.Add(item);
				}
			}
		}
	}

	protected void TiledFill(BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols)
	{
		Texture texture = material.mainTexture;
		if (texture == null)
		{
			return;
		}
		Vector4 vector = drawingDimensions;
		Vector2 vector2 = new Vector2(mInnerUV.width * (float)texture.width, mInnerUV.height * (float)texture.height);
		vector2 *= atlas.pixelSize;
		Color color = base.color;
		color.a = finalAlpha;
		Color32 item = ((!atlas.premultipliedAlpha) ? color : NGUITools.ApplyPMA(color));
		float x = vector.x;
		float num = vector.y;
		float xMin = mInnerUV.xMin;
		float yMin = mInnerUV.yMin;
		for (; num < vector.w; num += vector2.y)
		{
			x = vector.x;
			float num2 = num + vector2.y;
			float y = mInnerUV.yMax;
			if (num2 > vector.w)
			{
				y = Mathf.Lerp(mInnerUV.yMin, mInnerUV.yMax, (vector.w - num) / vector2.y);
				num2 = vector.w;
			}
			for (; x < vector.z; x += vector2.x)
			{
				float num3 = x + vector2.x;
				float x2 = mInnerUV.xMax;
				if (num3 > vector.z)
				{
					x2 = Mathf.Lerp(mInnerUV.xMin, mInnerUV.xMax, (vector.z - x) / vector2.x);
					num3 = vector.z;
				}
				verts.Add(new Vector3(x, num));
				verts.Add(new Vector3(x, num2));
				verts.Add(new Vector3(num3, num2));
				verts.Add(new Vector3(num3, num));
				uvs.Add(new Vector2(xMin, yMin));
				uvs.Add(new Vector2(xMin, y));
				uvs.Add(new Vector2(x2, y));
				uvs.Add(new Vector2(x2, yMin));
				cols.Add(item);
				cols.Add(item);
				cols.Add(item);
				cols.Add(item);
			}
		}
	}

	protected void FilledFill(BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols)
	{
		if (mFillAmount < 0.001f)
		{
			return;
		}
		Color color = base.color;
		color.a = finalAlpha;
		Color32 item = ((!atlas.premultipliedAlpha) ? color : NGUITools.ApplyPMA(color));
		Vector4 vector = drawingDimensions;
		float num = mOuterUV.xMin;
		float num2 = mOuterUV.yMin;
		float num3 = mOuterUV.xMax;
		float num4 = mOuterUV.yMax;
		if (mFillDirection == FillDirection.Horizontal || mFillDirection == FillDirection.Vertical)
		{
			if (mFillDirection == FillDirection.Horizontal)
			{
				float num5 = (num3 - num) * mFillAmount;
				if (mInvert)
				{
					vector.x = vector.z - (vector.z - vector.x) * mFillAmount;
					num = num3 - num5;
				}
				else
				{
					vector.z = vector.x + (vector.z - vector.x) * mFillAmount;
					num3 = num + num5;
				}
			}
			else if (mFillDirection == FillDirection.Vertical)
			{
				float num6 = (num4 - num2) * mFillAmount;
				if (mInvert)
				{
					vector.y = vector.w - (vector.w - vector.y) * mFillAmount;
					num2 = num4 - num6;
				}
				else
				{
					vector.w = vector.y + (vector.w - vector.y) * mFillAmount;
					num4 = num2 + num6;
				}
			}
		}
		mTempPos[0] = new Vector2(vector.x, vector.y);
		mTempPos[1] = new Vector2(vector.x, vector.w);
		mTempPos[2] = new Vector2(vector.z, vector.w);
		mTempPos[3] = new Vector2(vector.z, vector.y);
		mTempUVs[0] = new Vector2(num, num2);
		mTempUVs[1] = new Vector2(num, num4);
		mTempUVs[2] = new Vector2(num3, num4);
		mTempUVs[3] = new Vector2(num3, num2);
		if (mFillAmount < 1f)
		{
			if (mFillDirection == FillDirection.Radial90)
			{
				if (RadialCut(mTempPos, mTempUVs, mFillAmount, mInvert, 0))
				{
					for (int i = 0; i < 4; i++)
					{
						verts.Add(mTempPos[i]);
						uvs.Add(mTempUVs[i]);
						cols.Add(item);
					}
				}
				return;
			}
			if (mFillDirection == FillDirection.Radial180)
			{
				for (int j = 0; j < 2; j++)
				{
					float t = 0f;
					float t2 = 1f;
					float t3;
					float t4;
					if (j == 0)
					{
						t3 = 0f;
						t4 = 0.5f;
					}
					else
					{
						t3 = 0.5f;
						t4 = 1f;
					}
					mTempPos[0].x = Mathf.Lerp(vector.x, vector.z, t3);
					mTempPos[1].x = mTempPos[0].x;
					mTempPos[2].x = Mathf.Lerp(vector.x, vector.z, t4);
					mTempPos[3].x = mTempPos[2].x;
					mTempPos[0].y = Mathf.Lerp(vector.y, vector.w, t);
					mTempPos[1].y = Mathf.Lerp(vector.y, vector.w, t2);
					mTempPos[2].y = mTempPos[1].y;
					mTempPos[3].y = mTempPos[0].y;
					mTempUVs[0].x = Mathf.Lerp(num, num3, t3);
					mTempUVs[1].x = mTempUVs[0].x;
					mTempUVs[2].x = Mathf.Lerp(num, num3, t4);
					mTempUVs[3].x = mTempUVs[2].x;
					mTempUVs[0].y = Mathf.Lerp(num2, num4, t);
					mTempUVs[1].y = Mathf.Lerp(num2, num4, t2);
					mTempUVs[2].y = mTempUVs[1].y;
					mTempUVs[3].y = mTempUVs[0].y;
					float value = (mInvert ? (mFillAmount * 2f - (float)(1 - j)) : (fillAmount * 2f - (float)j));
					if (RadialCut(mTempPos, mTempUVs, Mathf.Clamp01(value), !mInvert, NGUIMath.RepeatIndex(j + 3, 4)))
					{
						for (int k = 0; k < 4; k++)
						{
							verts.Add(mTempPos[k]);
							uvs.Add(mTempUVs[k]);
							cols.Add(item);
						}
					}
				}
				return;
			}
			if (mFillDirection == FillDirection.Radial360)
			{
				for (int l = 0; l < 4; l++)
				{
					float t5;
					float t6;
					if (l < 2)
					{
						t5 = 0f;
						t6 = 0.5f;
					}
					else
					{
						t5 = 0.5f;
						t6 = 1f;
					}
					float t7;
					float t8;
					if (l == 0 || l == 3)
					{
						t7 = 0f;
						t8 = 0.5f;
					}
					else
					{
						t7 = 0.5f;
						t8 = 1f;
					}
					mTempPos[0].x = Mathf.Lerp(vector.x, vector.z, t5);
					mTempPos[1].x = mTempPos[0].x;
					mTempPos[2].x = Mathf.Lerp(vector.x, vector.z, t6);
					mTempPos[3].x = mTempPos[2].x;
					mTempPos[0].y = Mathf.Lerp(vector.y, vector.w, t7);
					mTempPos[1].y = Mathf.Lerp(vector.y, vector.w, t8);
					mTempPos[2].y = mTempPos[1].y;
					mTempPos[3].y = mTempPos[0].y;
					mTempUVs[0].x = Mathf.Lerp(num, num3, t5);
					mTempUVs[1].x = mTempUVs[0].x;
					mTempUVs[2].x = Mathf.Lerp(num, num3, t6);
					mTempUVs[3].x = mTempUVs[2].x;
					mTempUVs[0].y = Mathf.Lerp(num2, num4, t7);
					mTempUVs[1].y = Mathf.Lerp(num2, num4, t8);
					mTempUVs[2].y = mTempUVs[1].y;
					mTempUVs[3].y = mTempUVs[0].y;
					float value2 = ((!mInvert) ? (mFillAmount * 4f - (float)(3 - NGUIMath.RepeatIndex(l + 2, 4))) : (mFillAmount * 4f - (float)NGUIMath.RepeatIndex(l + 2, 4)));
					if (RadialCut(mTempPos, mTempUVs, Mathf.Clamp01(value2), mInvert, NGUIMath.RepeatIndex(l + 2, 4)))
					{
						for (int m = 0; m < 4; m++)
						{
							verts.Add(mTempPos[m]);
							uvs.Add(mTempUVs[m]);
							cols.Add(item);
						}
					}
				}
				return;
			}
		}
		for (int n = 0; n < 4; n++)
		{
			verts.Add(mTempPos[n]);
			uvs.Add(mTempUVs[n]);
			cols.Add(item);
		}
	}

	private static bool RadialCut(Vector2[] xy, Vector2[] uv, float fill, bool invert, int corner)
	{
		if (fill < 0.001f)
		{
			return false;
		}
		if ((corner & 1) == 1)
		{
			invert = !invert;
		}
		if (!invert && fill > 0.999f)
		{
			return true;
		}
		float num = Mathf.Clamp01(fill);
		if (invert)
		{
			num = 1f - num;
		}
		num *= (float)Math.PI / 2f;
		float cos = Mathf.Cos(num);
		float sin = Mathf.Sin(num);
		RadialCut(xy, cos, sin, invert, corner);
		RadialCut(uv, cos, sin, invert, corner);
		return true;
	}

	private static void RadialCut(Vector2[] xy, float cos, float sin, bool invert, int corner)
	{
		int num = NGUIMath.RepeatIndex(corner + 1, 4);
		int num2 = NGUIMath.RepeatIndex(corner + 2, 4);
		int num3 = NGUIMath.RepeatIndex(corner + 3, 4);
		if ((corner & 1) == 1)
		{
			if (sin > cos)
			{
				cos /= sin;
				sin = 1f;
				if (invert)
				{
					xy[num].x = Mathf.Lerp(xy[corner].x, xy[num2].x, cos);
					xy[num2].x = xy[num].x;
				}
			}
			else if (cos > sin)
			{
				sin /= cos;
				cos = 1f;
				if (!invert)
				{
					xy[num2].y = Mathf.Lerp(xy[corner].y, xy[num2].y, sin);
					xy[num3].y = xy[num2].y;
				}
			}
			else
			{
				cos = 1f;
				sin = 1f;
			}
			if (!invert)
			{
				xy[num3].x = Mathf.Lerp(xy[corner].x, xy[num2].x, cos);
			}
			else
			{
				xy[num].y = Mathf.Lerp(xy[corner].y, xy[num2].y, sin);
			}
			return;
		}
		if (cos > sin)
		{
			sin /= cos;
			cos = 1f;
			if (!invert)
			{
				xy[num].y = Mathf.Lerp(xy[corner].y, xy[num2].y, sin);
				xy[num2].y = xy[num].y;
			}
		}
		else if (sin > cos)
		{
			cos /= sin;
			sin = 1f;
			if (invert)
			{
				xy[num2].x = Mathf.Lerp(xy[corner].x, xy[num2].x, cos);
				xy[num3].x = xy[num2].x;
			}
		}
		else
		{
			cos = 1f;
			sin = 1f;
		}
		if (invert)
		{
			xy[num3].y = Mathf.Lerp(xy[corner].y, xy[num2].y, sin);
		}
		else
		{
			xy[num].x = Mathf.Lerp(xy[corner].x, xy[num2].x, cos);
		}
	}

	protected void AdvancedFill(BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols)
	{
		if (!mSprite.hasBorder)
		{
			SimpleFill(verts, uvs, cols);
			return;
		}
		Texture texture = material.mainTexture;
		if (texture == null)
		{
			return;
		}
		Vector4 vector = drawingDimensions;
		Vector4 vector2 = border * atlas.pixelSize;
		Vector2 vector3 = new Vector2(mInnerUV.width * (float)texture.width, mInnerUV.height * (float)texture.height);
		vector3 *= atlas.pixelSize;
		if (vector3.x < 1f)
		{
			vector3.x = 1f;
		}
		if (vector3.y < 1f)
		{
			vector3.y = 1f;
		}
		mTempPos[0].x = vector.x;
		mTempPos[0].y = vector.y;
		mTempPos[3].x = vector.z;
		mTempPos[3].y = vector.w;
		mTempPos[1].x = mTempPos[0].x + vector2.x;
		mTempPos[1].y = mTempPos[0].y + vector2.y;
		mTempPos[2].x = mTempPos[3].x - vector2.z;
		mTempPos[2].y = mTempPos[3].y - vector2.w;
		mTempUVs[0] = new Vector2(mOuterUV.xMin, mOuterUV.yMin);
		mTempUVs[1] = new Vector2(mInnerUV.xMin, mInnerUV.yMin);
		mTempUVs[2] = new Vector2(mInnerUV.xMax, mInnerUV.yMax);
		mTempUVs[3] = new Vector2(mOuterUV.xMax, mOuterUV.yMax);
		Color color = base.color;
		color.a = finalAlpha;
		Color32 color2 = ((!atlas.premultipliedAlpha) ? color : NGUITools.ApplyPMA(color));
		for (int i = 0; i < 3; i++)
		{
			int num = i + 1;
			for (int j = 0; j < 3; j++)
			{
				if (centerType == AdvancedType.Invisible && i == 1 && j == 1)
				{
					continue;
				}
				int num2 = j + 1;
				if (i == 1 && j == 1)
				{
					if (centerType == AdvancedType.Tiled)
					{
						float x = mTempPos[i].x;
						float x2 = mTempPos[num].x;
						float y = mTempPos[j].y;
						float y2 = mTempPos[num2].y;
						float x3 = mTempUVs[i].x;
						float y3 = mTempUVs[j].y;
						for (float num3 = y; num3 < y2; num3 += vector3.y)
						{
							float num4 = x;
							float num5 = mTempUVs[num2].y;
							float num6 = num3 + vector3.y;
							if (num6 > y2)
							{
								num5 = Mathf.Lerp(y3, num5, (y2 - num3) / vector3.y);
								num6 = y2;
							}
							for (; num4 < x2; num4 += vector3.x)
							{
								float num7 = num4 + vector3.x;
								float num8 = mTempUVs[num].x;
								if (num7 > x2)
								{
									num8 = Mathf.Lerp(x3, num8, (x2 - num4) / vector3.x);
									num7 = x2;
								}
								FillBuffers(num4, num7, num3, num6, x3, num8, y3, num5, color2, verts, uvs, cols);
							}
						}
					}
					else if (centerType == AdvancedType.Sliced)
					{
						FillBuffers(mTempPos[i].x, mTempPos[num].x, mTempPos[j].y, mTempPos[num2].y, mTempUVs[i].x, mTempUVs[num].x, mTempUVs[j].y, mTempUVs[num2].y, color2, verts, uvs, cols);
					}
				}
				else if (i == 1)
				{
					if ((j == 0 && bottomType == AdvancedType.Tiled) || (j == 2 && topType == AdvancedType.Tiled))
					{
						float x4 = mTempPos[i].x;
						float x5 = mTempPos[num].x;
						float y4 = mTempPos[j].y;
						float y5 = mTempPos[num2].y;
						float x6 = mTempUVs[i].x;
						float y6 = mTempUVs[j].y;
						float y7 = mTempUVs[num2].y;
						for (float num9 = x4; num9 < x5; num9 += vector3.x)
						{
							float num10 = num9 + vector3.x;
							float num11 = mTempUVs[num].x;
							if (num10 > x5)
							{
								num11 = Mathf.Lerp(x6, num11, (x5 - num9) / vector3.x);
								num10 = x5;
							}
							FillBuffers(num9, num10, y4, y5, x6, num11, y6, y7, color2, verts, uvs, cols);
						}
					}
					else if ((j == 0 && bottomType == AdvancedType.Sliced) || (j == 2 && topType == AdvancedType.Sliced))
					{
						FillBuffers(mTempPos[i].x, mTempPos[num].x, mTempPos[j].y, mTempPos[num2].y, mTempUVs[i].x, mTempUVs[num].x, mTempUVs[j].y, mTempUVs[num2].y, color2, verts, uvs, cols);
					}
				}
				else if (j == 1)
				{
					if ((i == 0 && leftType == AdvancedType.Tiled) || (i == 2 && rightType == AdvancedType.Tiled))
					{
						float x7 = mTempPos[i].x;
						float x8 = mTempPos[num].x;
						float y8 = mTempPos[j].y;
						float y9 = mTempPos[num2].y;
						float x9 = mTempUVs[i].x;
						float x10 = mTempUVs[num].x;
						float y10 = mTempUVs[j].y;
						for (float num12 = y8; num12 < y9; num12 += vector3.y)
						{
							float num13 = mTempUVs[num2].y;
							float num14 = num12 + vector3.y;
							if (num14 > y9)
							{
								num13 = Mathf.Lerp(y10, num13, (y9 - num12) / vector3.y);
								num14 = y9;
							}
							FillBuffers(x7, x8, num12, num14, x9, x10, y10, num13, color2, verts, uvs, cols);
						}
					}
					else if ((i == 0 && leftType == AdvancedType.Sliced) || (i == 2 && rightType == AdvancedType.Sliced))
					{
						FillBuffers(mTempPos[i].x, mTempPos[num].x, mTempPos[j].y, mTempPos[num2].y, mTempUVs[i].x, mTempUVs[num].x, mTempUVs[j].y, mTempUVs[num2].y, color2, verts, uvs, cols);
					}
				}
				else
				{
					FillBuffers(mTempPos[i].x, mTempPos[num].x, mTempPos[j].y, mTempPos[num2].y, mTempUVs[i].x, mTempUVs[num].x, mTempUVs[j].y, mTempUVs[num2].y, color2, verts, uvs, cols);
				}
			}
		}
	}

	private void FillBuffers(float v0x, float v1x, float v0y, float v1y, float u0x, float u1x, float u0y, float u1y, Color col, BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols)
	{
		verts.Add(new Vector3(v0x, v0y));
		verts.Add(new Vector3(v0x, v1y));
		verts.Add(new Vector3(v1x, v1y));
		verts.Add(new Vector3(v1x, v0y));
		uvs.Add(new Vector2(u0x, u0y));
		uvs.Add(new Vector2(u0x, u1y));
		uvs.Add(new Vector2(u1x, u1y));
		uvs.Add(new Vector2(u1x, u0y));
		cols.Add(col);
		cols.Add(col);
		cols.Add(col);
		cols.Add(col);
	}
}

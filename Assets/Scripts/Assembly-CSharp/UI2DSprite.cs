using UnityEngine;

[AddComponentMenu("NGUI/UI/NGUI Unity2D Sprite")]
[ExecuteInEditMode]
public class UI2DSprite : UIWidget
{
	[SerializeField]
	[HideInInspector]
	private Sprite mSprite;

	[SerializeField]
	[HideInInspector]
	private Material mMat;

	[HideInInspector]
	[SerializeField]
	private Shader mShader;

	public Sprite nextSprite;

	private int mPMA = -1;

	public Sprite sprite2D
	{
		get
		{
			return mSprite;
		}
		set
		{
			if (mSprite != value)
			{
				RemoveFromPanel();
				mSprite = value;
				nextSprite = null;
				MarkAsChanged();
			}
		}
	}

	public override Material material
	{
		get
		{
			return mMat;
		}
		set
		{
			if (mMat != value)
			{
				RemoveFromPanel();
				mMat = value;
				mPMA = -1;
				MarkAsChanged();
			}
		}
	}

	public override Shader shader
	{
		get
		{
			if (mMat != null)
			{
				return mMat.shader;
			}
			if (mShader == null)
			{
				mShader = Shader.Find("Unlit/Transparent Colored");
			}
			return mShader;
		}
		set
		{
			if (mShader != value)
			{
				RemoveFromPanel();
				mShader = value;
				if (mMat == null)
				{
					mPMA = -1;
					MarkAsChanged();
				}
			}
		}
	}

	public override Texture mainTexture
	{
		get
		{
			if (mSprite != null)
			{
				return mSprite.texture;
			}
			if (mMat != null)
			{
				return mMat.mainTexture;
			}
			return null;
		}
	}

	public bool premultipliedAlpha
	{
		get
		{
			if (mPMA == -1)
			{
				Shader shader = this.shader;
				mPMA = ((shader != null && shader.name.Contains("Premultiplied")) ? 1 : 0);
			}
			return mPMA == 1;
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
			int num5 = ((!(mSprite != null)) ? mWidth : Mathf.RoundToInt(mSprite.textureRect.width));
			int num6 = ((!(mSprite != null)) ? mHeight : Mathf.RoundToInt(mSprite.textureRect.height));
			if (((uint)num5 & (true ? 1u : 0u)) != 0)
			{
				num3 -= 1f / (float)num5 * (float)mWidth;
			}
			if (((uint)num6 & (true ? 1u : 0u)) != 0)
			{
				num4 -= 1f / (float)num6 * (float)mHeight;
			}
			return new Vector4((mDrawRegion.x != 0f) ? Mathf.Lerp(num, num3, mDrawRegion.x) : num, (mDrawRegion.y != 0f) ? Mathf.Lerp(num2, num4, mDrawRegion.y) : num2, (mDrawRegion.z != 1f) ? Mathf.Lerp(num, num3, mDrawRegion.z) : num3, (mDrawRegion.w != 1f) ? Mathf.Lerp(num2, num4, mDrawRegion.w) : num4);
		}
	}

	public Rect uvRect
	{
		get
		{
			Texture texture = mainTexture;
			if (texture != null)
			{
				Rect textureRect = mSprite.textureRect;
				textureRect.xMin /= texture.width;
				textureRect.xMax /= texture.width;
				textureRect.yMin /= texture.height;
				textureRect.yMax /= texture.height;
				return textureRect;
			}
			return new Rect(0f, 0f, 1f, 1f);
		}
	}

	protected override void OnUpdate()
	{
		if (nextSprite != null)
		{
			if (nextSprite != mSprite)
			{
				sprite2D = nextSprite;
			}
			nextSprite = null;
		}
		base.OnUpdate();
	}

	public override void MakePixelPerfect()
	{
		if (mSprite != null)
		{
			Rect textureRect = mSprite.textureRect;
			int num = Mathf.RoundToInt(textureRect.width);
			int num2 = Mathf.RoundToInt(textureRect.height);
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
		base.MakePixelPerfect();
	}

	public override void OnFill(BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols)
	{
		Color color = base.color;
		color.a = finalAlpha;
		Color32 item = ((!premultipliedAlpha) ? color : NGUITools.ApplyPMA(color));
		Vector4 vector = drawingDimensions;
		Rect rect = uvRect;
		verts.Add(new Vector3(vector.x, vector.y));
		verts.Add(new Vector3(vector.x, vector.w));
		verts.Add(new Vector3(vector.z, vector.w));
		verts.Add(new Vector3(vector.z, vector.y));
		uvs.Add(new Vector2(rect.xMin, rect.yMin));
		uvs.Add(new Vector2(rect.xMin, rect.yMax));
		uvs.Add(new Vector2(rect.xMax, rect.yMax));
		uvs.Add(new Vector2(rect.xMax, rect.yMin));
		cols.Add(item);
		cols.Add(item);
		cols.Add(item);
		cols.Add(item);
	}
}

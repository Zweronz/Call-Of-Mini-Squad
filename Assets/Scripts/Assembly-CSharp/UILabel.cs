using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("NGUI/UI/NGUI Label")]
public class UILabel : UIWidget
{
	public enum Effect
	{
		None = 0,
		Shadow = 1,
		Outline = 2
	}

	public enum Overflow
	{
		ShrinkContent = 0,
		ClampContent = 1,
		ResizeFreely = 2,
		ResizeHeight = 3
	}

	public enum Crispness
	{
		Never = 0,
		OnDesktop = 1,
		Always = 2
	}

	public Crispness keepCrispWhenShrunk = Crispness.OnDesktop;

	[HideInInspector]
	[SerializeField]
	private Font mTrueTypeFont;

	[HideInInspector]
	[SerializeField]
	private UIFont mFont;

	[Multiline(6)]
	[HideInInspector]
	[SerializeField]
	private string mText = string.Empty;

	[SerializeField]
	[HideInInspector]
	private int mFontSize = 16;

	[HideInInspector]
	[SerializeField]
	private FontStyle mFontStyle;

	[HideInInspector]
	[SerializeField]
	private NGUIText.Alignment mAlignment;

	[HideInInspector]
	[SerializeField]
	private bool mEncoding = true;

	[HideInInspector]
	[SerializeField]
	private int mMaxLineCount;

	[SerializeField]
	[HideInInspector]
	private Effect mEffectStyle;

	[SerializeField]
	[HideInInspector]
	private Color mEffectColor = Color.black;

	[SerializeField]
	[HideInInspector]
	private NGUIText.SymbolStyle mSymbols = NGUIText.SymbolStyle.Uncolored;

	[SerializeField]
	[HideInInspector]
	private Vector2 mEffectDistance = Vector2.one;

	[HideInInspector]
	[SerializeField]
	private Overflow mOverflow;

	[SerializeField]
	[HideInInspector]
	private Material mMaterial;

	[HideInInspector]
	[SerializeField]
	private bool mApplyGradient;

	[SerializeField]
	[HideInInspector]
	private Color mGradientTop = Color.white;

	[SerializeField]
	[HideInInspector]
	private Color mGradientBottom = new Color(0.7f, 0.7f, 0.7f);

	[HideInInspector]
	[SerializeField]
	private int mSpacingX;

	[SerializeField]
	[HideInInspector]
	private int mSpacingY;

	[SerializeField]
	[HideInInspector]
	private bool mShrinkToFit;

	[SerializeField]
	[HideInInspector]
	private int mMaxLineWidth;

	[SerializeField]
	[HideInInspector]
	private int mMaxLineHeight;

	[SerializeField]
	[HideInInspector]
	private float mLineWidth;

	[HideInInspector]
	[SerializeField]
	private bool mMultiline = true;

	private Font mActiveTTF;

	private bool mShouldBeProcessed = true;

	private string mProcessedText;

	private bool mPremultiply;

	private Vector2 mCalculatedSize = Vector2.zero;

	private float mScale = 1f;

	private int mPrintedSize;

	private int mLastWidth;

	private int mLastHeight;

	private static BetterList<UILabel> mList = new BetterList<UILabel>();

	private static Dictionary<Font, int> mFontUsage = new Dictionary<Font, int>();

	private static BetterList<Vector3> mTempVerts = new BetterList<Vector3>();

	private static BetterList<int> mTempIndices = new BetterList<int>();

	private bool shouldBeProcessed
	{
		get
		{
			return mShouldBeProcessed;
		}
		set
		{
			if (value)
			{
				mChanged = true;
				mShouldBeProcessed = true;
			}
			else
			{
				mShouldBeProcessed = false;
			}
		}
	}

	public override bool isAnchoredHorizontally
	{
		get
		{
			return base.isAnchoredHorizontally || mOverflow == Overflow.ResizeFreely;
		}
	}

	public override bool isAnchoredVertically
	{
		get
		{
			return base.isAnchoredVertically || mOverflow == Overflow.ResizeFreely || mOverflow == Overflow.ResizeHeight;
		}
	}

	public override Material material
	{
		get
		{
			if (mMaterial != null)
			{
				return mMaterial;
			}
			if (mFont != null)
			{
				return mFont.material;
			}
			if (mTrueTypeFont != null)
			{
				return mTrueTypeFont.material;
			}
			return null;
		}
		set
		{
			if (mMaterial != value)
			{
				MarkAsChanged();
				mMaterial = value;
				MarkAsChanged();
			}
		}
	}

	[Obsolete("Use UILabel.bitmapFont instead")]
	public UIFont font
	{
		get
		{
			return bitmapFont;
		}
		set
		{
			bitmapFont = value;
		}
	}

	public UIFont bitmapFont
	{
		get
		{
			return mFont;
		}
		set
		{
			if (mFont != value)
			{
				RemoveFromPanel();
				mFont = value;
				mTrueTypeFont = null;
				MarkAsChanged();
			}
		}
	}

	public Font trueTypeFont
	{
		get
		{
			if (mTrueTypeFont != null)
			{
				return mTrueTypeFont;
			}
			return (!(mFont != null)) ? null : mFont.dynamicFont;
		}
		set
		{
			if (mTrueTypeFont != value)
			{
				SetActiveFont(null);
				RemoveFromPanel();
				mTrueTypeFont = value;
				shouldBeProcessed = true;
				mFont = null;
				SetActiveFont(value);
				ProcessAndRequest();
				if (mActiveTTF != null)
				{
					base.MarkAsChanged();
				}
			}
		}
	}

	public UnityEngine.Object ambigiousFont
	{
		get
		{
			return (!(mFont != null)) ? ((UnityEngine.Object)mTrueTypeFont) : ((UnityEngine.Object)mFont);
		}
		set
		{
			UIFont uIFont = value as UIFont;
			if (uIFont != null)
			{
				bitmapFont = uIFont;
			}
			else
			{
				trueTypeFont = value as Font;
			}
		}
	}

	public string text
	{
		get
		{
			return mText;
		}
		set
		{
			try
			{
				if (mText == value)
				{
					return;
				}
				if (string.IsNullOrEmpty(value))
				{
					if (!string.IsNullOrEmpty(mText))
					{
						mText = string.Empty;
						shouldBeProcessed = true;
						ProcessAndRequest();
					}
				}
				else if (mText != value)
				{
					mText = value;
					shouldBeProcessed = true;
					ProcessAndRequest();
				}
				if (autoResizeBoxCollider)
				{
					ResizeCollider();
				}
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
		}
	}

	public int defaultFontSize
	{
		get
		{
			return (trueTypeFont != null) ? mFontSize : ((!(mFont != null)) ? 16 : mFont.defaultSize);
		}
	}

	public int fontSize
	{
		get
		{
			return mFontSize;
		}
		set
		{
			value = Mathf.Clamp(value, 0, 256);
			if (mFontSize != value)
			{
				mFontSize = value;
				shouldBeProcessed = true;
				ProcessAndRequest();
			}
		}
	}

	public FontStyle fontStyle
	{
		get
		{
			return mFontStyle;
		}
		set
		{
			if (mFontStyle != value)
			{
				mFontStyle = value;
				shouldBeProcessed = true;
				ProcessAndRequest();
			}
		}
	}

	public NGUIText.Alignment alignment
	{
		get
		{
			return mAlignment;
		}
		set
		{
			if (mAlignment != value)
			{
				mAlignment = value;
				shouldBeProcessed = true;
				ProcessAndRequest();
			}
		}
	}

	public bool applyGradient
	{
		get
		{
			return mApplyGradient;
		}
		set
		{
			if (mApplyGradient != value)
			{
				mApplyGradient = value;
				MarkAsChanged();
			}
		}
	}

	public Color gradientTop
	{
		get
		{
			return mGradientTop;
		}
		set
		{
			if (mGradientTop != value)
			{
				mGradientTop = value;
				if (mApplyGradient)
				{
					MarkAsChanged();
				}
			}
		}
	}

	public Color gradientBottom
	{
		get
		{
			return mGradientBottom;
		}
		set
		{
			if (mGradientBottom != value)
			{
				mGradientBottom = value;
				if (mApplyGradient)
				{
					MarkAsChanged();
				}
			}
		}
	}

	public int spacingX
	{
		get
		{
			return mSpacingX;
		}
		set
		{
			if (mSpacingX != value)
			{
				mSpacingX = value;
				MarkAsChanged();
			}
		}
	}

	public int spacingY
	{
		get
		{
			return mSpacingY;
		}
		set
		{
			if (mSpacingY != value)
			{
				mSpacingY = value;
				MarkAsChanged();
			}
		}
	}

	private bool keepCrisp
	{
		get
		{
			if (trueTypeFont != null && keepCrispWhenShrunk != 0)
			{
				return keepCrispWhenShrunk == Crispness.Always;
			}
			return false;
		}
	}

	public bool supportEncoding
	{
		get
		{
			return mEncoding;
		}
		set
		{
			if (mEncoding != value)
			{
				mEncoding = value;
				shouldBeProcessed = true;
			}
		}
	}

	public NGUIText.SymbolStyle symbolStyle
	{
		get
		{
			return mSymbols;
		}
		set
		{
			if (mSymbols != value)
			{
				mSymbols = value;
				shouldBeProcessed = true;
			}
		}
	}

	public Overflow overflowMethod
	{
		get
		{
			return mOverflow;
		}
		set
		{
			if (mOverflow != value)
			{
				mOverflow = value;
				shouldBeProcessed = true;
			}
		}
	}

	[Obsolete("Use 'width' instead")]
	public int lineWidth
	{
		get
		{
			return base.width;
		}
		set
		{
			base.width = value;
		}
	}

	[Obsolete("Use 'height' instead")]
	public int lineHeight
	{
		get
		{
			return base.height;
		}
		set
		{
			base.height = value;
		}
	}

	public bool multiLine
	{
		get
		{
			return mMaxLineCount != 1;
		}
		set
		{
			if (mMaxLineCount != 1 != value)
			{
				mMaxLineCount = ((!value) ? 1 : 0);
				shouldBeProcessed = true;
			}
		}
	}

	public override Vector3[] localCorners
	{
		get
		{
			if (shouldBeProcessed)
			{
				ProcessText();
			}
			return base.localCorners;
		}
	}

	public override Vector3[] worldCorners
	{
		get
		{
			if (shouldBeProcessed)
			{
				ProcessText();
			}
			return base.worldCorners;
		}
	}

	public override Vector4 drawingDimensions
	{
		get
		{
			if (shouldBeProcessed)
			{
				ProcessText();
			}
			return base.drawingDimensions;
		}
	}

	public int maxLineCount
	{
		get
		{
			return mMaxLineCount;
		}
		set
		{
			if (mMaxLineCount != value)
			{
				mMaxLineCount = Mathf.Max(value, 0);
				shouldBeProcessed = true;
				if (overflowMethod == Overflow.ShrinkContent)
				{
					MakePixelPerfect();
				}
			}
		}
	}

	public Effect effectStyle
	{
		get
		{
			return mEffectStyle;
		}
		set
		{
			if (mEffectStyle != value)
			{
				mEffectStyle = value;
				shouldBeProcessed = true;
			}
		}
	}

	public Color effectColor
	{
		get
		{
			return mEffectColor;
		}
		set
		{
			if (mEffectColor != value)
			{
				mEffectColor = value;
				if (mEffectStyle != 0)
				{
					shouldBeProcessed = true;
				}
			}
		}
	}

	public Vector2 effectDistance
	{
		get
		{
			return mEffectDistance;
		}
		set
		{
			if (mEffectDistance != value)
			{
				mEffectDistance = value;
				shouldBeProcessed = true;
			}
		}
	}

	[Obsolete("Use 'overflowMethod == UILabel.Overflow.ShrinkContent' instead")]
	public bool shrinkToFit
	{
		get
		{
			return mOverflow == Overflow.ShrinkContent;
		}
		set
		{
			if (value)
			{
				overflowMethod = Overflow.ShrinkContent;
			}
		}
	}

	public string processedText
	{
		get
		{
			if (mLastWidth != mWidth || mLastHeight != mHeight)
			{
				mLastWidth = mWidth;
				mLastHeight = mHeight;
				mShouldBeProcessed = true;
			}
			if (shouldBeProcessed)
			{
				ProcessText();
			}
			return mProcessedText;
		}
	}

	public Vector2 printedSize
	{
		get
		{
			if (shouldBeProcessed)
			{
				ProcessText();
			}
			return mCalculatedSize;
		}
	}

	public override Vector2 localSize
	{
		get
		{
			if (shouldBeProcessed)
			{
				ProcessText();
			}
			return base.localSize;
		}
	}

	private bool isValid
	{
		get
		{
			return mFont != null || mTrueTypeFont != null;
		}
	}

	private float pixelSize
	{
		get
		{
			return (!(mFont != null)) ? 1f : mFont.pixelSize;
		}
	}

	protected override void OnInit()
	{
		base.OnInit();
		mList.Add(this);
		SetActiveFont(trueTypeFont);
	}

	protected override void OnDisable()
	{
		SetActiveFont(null);
		mList.Remove(this);
		base.OnDisable();
	}

	protected void SetActiveFont(Font fnt)
	{
		if (!(mActiveTTF != fnt))
		{
			return;
		}
		if (mActiveTTF != null)
		{
			int value;
			if (mFontUsage.TryGetValue(mActiveTTF, out value))
			{
				value = Mathf.Max(0, --value);
				if (value == 0)
				{
					mActiveTTF.textureRebuildCallback = null;
					mFontUsage.Remove(mActiveTTF);
				}
				else
				{
					mFontUsage[mActiveTTF] = value;
				}
			}
			else
			{
				mActiveTTF.textureRebuildCallback = null;
			}
		}
		mActiveTTF = fnt;
		if (mActiveTTF != null)
		{
			int value2 = 0;
			if (!mFontUsage.TryGetValue(mActiveTTF, out value2))
			{
				mActiveTTF.textureRebuildCallback = OnFontTextureChanged;
			}
			value2 = (mFontUsage[mActiveTTF] = value2 + 1);
		}
	}

	private static void OnFontTextureChanged()
	{
		for (int i = 0; i < mList.size; i++)
		{
			UILabel uILabel = mList[i];
			if (uILabel != null)
			{
				Font font = uILabel.trueTypeFont;
				if (font != null)
				{
					font.RequestCharactersInTexture(uILabel.mText, uILabel.mPrintedSize, uILabel.mFontStyle);
					uILabel.MarkAsChanged();
				}
			}
		}
	}

	public override Vector3[] GetSides(Transform relativeTo)
	{
		if (shouldBeProcessed)
		{
			ProcessText();
		}
		return base.GetSides(relativeTo);
	}

	protected override void UpgradeFrom265()
	{
		ProcessText(true);
		if (mShrinkToFit)
		{
			overflowMethod = Overflow.ShrinkContent;
			mMaxLineCount = 0;
		}
		if (mMaxLineWidth != 0)
		{
			base.width = mMaxLineWidth;
			overflowMethod = ((mMaxLineCount > 0) ? Overflow.ResizeHeight : Overflow.ShrinkContent);
		}
		else
		{
			overflowMethod = Overflow.ResizeFreely;
		}
		if (mMaxLineHeight != 0)
		{
			base.height = mMaxLineHeight;
		}
		if (mFont != null)
		{
			int num = Mathf.RoundToInt((float)mFont.defaultSize * mFont.pixelSize);
			if (base.height < num)
			{
				base.height = num;
			}
		}
		mMaxLineWidth = 0;
		mMaxLineHeight = 0;
		mShrinkToFit = false;
		if (GetComponent<BoxCollider>() != null)
		{
			NGUITools.AddWidgetCollider(base.gameObject, true);
		}
	}

	protected override void OnAnchor()
	{
		if (mOverflow == Overflow.ResizeFreely)
		{
			if (base.isFullyAnchored)
			{
				mOverflow = Overflow.ShrinkContent;
			}
		}
		else if (mOverflow == Overflow.ResizeHeight && topAnchor.target != null && bottomAnchor.target != null)
		{
			mOverflow = Overflow.ShrinkContent;
		}
		base.OnAnchor();
	}

	private void ProcessAndRequest()
	{
		if (ambigiousFont != null)
		{
			ProcessText();
		}
	}

	protected override void OnStart()
	{
		base.OnStart();
		if (mLineWidth > 0f)
		{
			mMaxLineWidth = Mathf.RoundToInt(mLineWidth);
			mLineWidth = 0f;
		}
		if (!mMultiline)
		{
			mMaxLineCount = 1;
			mMultiline = true;
		}
		mPremultiply = material != null && material.shader != null && material.shader.name.Contains("Premultiplied");
		ProcessAndRequest();
	}

	public override void MarkAsChanged()
	{
		shouldBeProcessed = true;
		base.MarkAsChanged();
	}

	private void ProcessText()
	{
		ProcessText(false);
	}

	private void ProcessText(bool legacyMode)
	{
		try
		{
			if (!isValid)
			{
				return;
			}
			mChanged = true;
			shouldBeProcessed = false;
			NGUIText.rectWidth = ((!legacyMode) ? base.width : ((mMaxLineWidth == 0) ? 1000000 : mMaxLineWidth));
			NGUIText.rectHeight = ((!legacyMode) ? base.height : ((mMaxLineHeight == 0) ? 1000000 : mMaxLineHeight));
			mPrintedSize = Mathf.Abs((!legacyMode) ? defaultFontSize : Mathf.RoundToInt(base.cachedTransform.localScale.x));
			mScale = 1f;
			if (NGUIText.rectWidth < 1 || NGUIText.rectHeight < 0)
			{
				mProcessedText = string.Empty;
				return;
			}
			UpdateNGUIText(mPrintedSize, mWidth, mHeight);
			bool flag = trueTypeFont != null;
			if (mOverflow == Overflow.ResizeFreely)
			{
				NGUIText.rectWidth = 1000000;
			}
			if (mOverflow == Overflow.ResizeFreely || mOverflow == Overflow.ResizeHeight)
			{
				NGUIText.rectHeight = 1000000;
			}
			if (mPrintedSize > 0)
			{
				bool flag2 = keepCrisp;
				int num = mPrintedSize;
				while (num > 0)
				{
					if (flag2)
					{
						mPrintedSize = num;
						NGUIText.fontSize = mPrintedSize;
					}
					else
					{
						mScale = (float)num / (float)mPrintedSize;
						NGUIText.fontScale = ((!flag) ? ((float)mFontSize / (float)mFont.defaultSize * mScale * bitmapFont.pixelSize) : mScale);
					}
					NGUIText.Update(false);
					bool flag3 = NGUIText.WrapText(mText, out mProcessedText, true);
					if (mOverflow == Overflow.ShrinkContent && !flag3)
					{
						if (--num <= 1)
						{
							break;
						}
						num--;
						continue;
					}
					if (mOverflow == Overflow.ResizeFreely)
					{
						mCalculatedSize = NGUIText.CalculatePrintedSize(mProcessedText);
						mWidth = Mathf.Max(minWidth, Mathf.RoundToInt(mCalculatedSize.x));
						mHeight = Mathf.Max(minHeight, Mathf.RoundToInt(mCalculatedSize.y));
						if ((mWidth & 1) == 1)
						{
							mWidth++;
						}
						if ((mHeight & 1) == 1)
						{
							mHeight++;
						}
					}
					else if (mOverflow == Overflow.ResizeHeight)
					{
						mCalculatedSize = NGUIText.CalculatePrintedSize(mProcessedText);
						mHeight = Mathf.Max(minHeight, Mathf.RoundToInt(mCalculatedSize.y));
						if ((mHeight & 1) == 1)
						{
							mHeight++;
						}
					}
					else
					{
						mCalculatedSize = NGUIText.CalculatePrintedSize(mProcessedText);
					}
					if (legacyMode)
					{
						base.width = Mathf.RoundToInt(mCalculatedSize.x);
						base.height = Mathf.RoundToInt(mCalculatedSize.y);
						base.cachedTransform.localScale = Vector3.one;
					}
					break;
				}
			}
			else
			{
				base.cachedTransform.localScale = Vector3.one;
				mProcessedText = string.Empty;
				mScale = 1f;
			}
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
	}

	public override void MakePixelPerfect()
	{
		if (ambigiousFont != null)
		{
			float num = ((!(trueTypeFont != null)) ? bitmapFont.pixelSize : 1f);
			Vector3 localPosition = base.cachedTransform.localPosition;
			localPosition.x = Mathf.RoundToInt(localPosition.x);
			localPosition.y = Mathf.RoundToInt(localPosition.y);
			localPosition.z = Mathf.RoundToInt(localPosition.z);
			base.cachedTransform.localPosition = localPosition;
			base.cachedTransform.localScale = Vector3.one;
			if (mOverflow == Overflow.ResizeFreely)
			{
				AssumeNaturalSize();
				return;
			}
			int a = base.width;
			int a2 = base.height;
			Overflow overflow = mOverflow;
			mOverflow = Overflow.ShrinkContent;
			mWidth = 100000;
			mHeight = 100000;
			ProcessText(false);
			mOverflow = overflow;
			int a3 = Mathf.RoundToInt(mCalculatedSize.x * num);
			int a4 = Mathf.RoundToInt(mCalculatedSize.y * num);
			a3 = Mathf.Max(a3, base.minWidth);
			a4 = Mathf.Max(a4, base.minHeight);
			mWidth = Mathf.Max(a, a3);
			mHeight = Mathf.Max(a2, a4);
			MarkAsChanged();
		}
		else
		{
			base.MakePixelPerfect();
		}
	}

	public void AssumeNaturalSize()
	{
		if (ambigiousFont != null)
		{
			mWidth = 100000;
			mHeight = 100000;
			ProcessText(false);
			mWidth = Mathf.RoundToInt(mCalculatedSize.x * pixelSize);
			mHeight = Mathf.RoundToInt(mCalculatedSize.y * pixelSize);
			if ((mWidth & 1) == 1)
			{
				mWidth++;
			}
			if ((mHeight & 1) == 1)
			{
				mHeight++;
			}
			MarkAsChanged();
		}
	}

	[Obsolete("Use UILabel.GetCharacterAtPosition instead")]
	public int GetCharacterIndex(Vector3 worldPos)
	{
		return GetCharacterIndexAtPosition(worldPos);
	}

	[Obsolete("Use UILabel.GetCharacterAtPosition instead")]
	public int GetCharacterIndex(Vector2 localPos)
	{
		return GetCharacterIndexAtPosition(localPos);
	}

	public int GetCharacterIndexAtPosition(Vector3 worldPos)
	{
		Vector2 localPos = base.cachedTransform.InverseTransformPoint(worldPos);
		return GetCharacterIndexAtPosition(localPos);
	}

	public int GetCharacterIndexAtPosition(Vector2 localPos)
	{
		if (isValid)
		{
			string value = processedText;
			if (string.IsNullOrEmpty(value))
			{
				return 0;
			}
			UpdateNGUIText(defaultFontSize, mWidth, mHeight);
			NGUIText.PrintCharacterPositions(value, mTempVerts, mTempIndices);
			if (mTempVerts.size > 0)
			{
				ApplyOffset(mTempVerts, 0);
				int closestCharacter = NGUIText.GetClosestCharacter(mTempVerts, localPos);
				closestCharacter = mTempIndices[closestCharacter];
				mTempVerts.Clear();
				mTempIndices.Clear();
				return closestCharacter;
			}
		}
		return 0;
	}

	public string GetWordAtPosition(Vector3 worldPos)
	{
		return GetWordAtCharacterIndex(GetCharacterIndexAtPosition(worldPos));
	}

	public string GetWordAtPosition(Vector2 localPos)
	{
		return GetWordAtCharacterIndex(GetCharacterIndexAtPosition(localPos));
	}

	public string GetWordAtCharacterIndex(int characterIndex)
	{
		if (characterIndex != -1 && characterIndex < mText.Length)
		{
			int num = mText.LastIndexOf(' ', characterIndex, characterIndex) + 1;
			int num2 = mText.IndexOf(' ', characterIndex);
			if (num2 == -1)
			{
				num2 = mText.Length;
			}
			if (num != num2)
			{
				string text = mText.Substring(num, num2 - num);
				return NGUIText.StripSymbols(text);
			}
		}
		return null;
	}

	public string GetUrlAtPosition(Vector3 worldPos)
	{
		return GetUrlAtCharacterIndex(GetCharacterIndexAtPosition(worldPos));
	}

	public string GetUrlAtPosition(Vector2 localPos)
	{
		return GetUrlAtCharacterIndex(GetCharacterIndexAtPosition(localPos));
	}

	public string GetUrlAtCharacterIndex(int characterIndex)
	{
		if (characterIndex != -1 && characterIndex < mText.Length)
		{
			int num = mText.LastIndexOf("[url=", characterIndex);
			if (num != -1)
			{
				num += 5;
				int num2 = mText.IndexOf("]", num);
				if (num2 != -1)
				{
					int num3 = mText.IndexOf("[/url]", num2);
					if (num3 == -1 || num3 >= characterIndex)
					{
						return mText.Substring(num, num2 - num);
					}
				}
			}
		}
		return null;
	}

	public int GetCharacterIndex(int currentIndex, KeyCode key)
	{
		if (isValid)
		{
			string text = processedText;
			if (string.IsNullOrEmpty(text))
			{
				return 0;
			}
			int num = defaultFontSize;
			UpdateNGUIText(num, mWidth, mHeight);
			NGUIText.PrintCharacterPositions(text, mTempVerts, mTempIndices);
			if (mTempVerts.size > 0)
			{
				ApplyOffset(mTempVerts, 0);
				for (int i = 0; i < mTempIndices.size; i++)
				{
					if (mTempIndices[i] == currentIndex)
					{
						Vector2 pos = mTempVerts[i];
						switch (key)
						{
						case KeyCode.UpArrow:
							pos.y += num + spacingY;
							break;
						case KeyCode.DownArrow:
							pos.y -= num + spacingY;
							break;
						case KeyCode.Home:
							pos.x -= 1000f;
							break;
						case KeyCode.End:
							pos.x += 1000f;
							break;
						}
						int closestCharacter = NGUIText.GetClosestCharacter(mTempVerts, pos);
						closestCharacter = mTempIndices[closestCharacter];
						if (closestCharacter == currentIndex)
						{
							break;
						}
						mTempVerts.Clear();
						mTempIndices.Clear();
						return closestCharacter;
					}
				}
				mTempVerts.Clear();
				mTempIndices.Clear();
			}
			switch (key)
			{
			case KeyCode.UpArrow:
			case KeyCode.Home:
				return 0;
			case KeyCode.DownArrow:
			case KeyCode.End:
				return text.Length;
			}
		}
		return currentIndex;
	}

	public void PrintOverlay(int start, int end, UIGeometry caret, UIGeometry highlight, Color caretColor, Color highlightColor)
	{
		if (caret != null)
		{
			caret.Clear();
		}
		if (highlight != null)
		{
			highlight.Clear();
		}
		if (!isValid)
		{
			return;
		}
		string text = processedText;
		UpdateNGUIText(defaultFontSize, mWidth, mHeight);
		int size = caret.verts.size;
		Vector2 item = new Vector2(0.5f, 0.5f);
		float num = finalAlpha;
		if (highlight != null && start != end)
		{
			int size2 = highlight.verts.size;
			NGUIText.PrintCaretAndSelection(text, start, end, caret.verts, highlight.verts);
			if (highlight.verts.size > size2)
			{
				ApplyOffset(highlight.verts, size2);
				Color32 item2 = new Color(highlightColor.r, highlightColor.g, highlightColor.b, highlightColor.a * num);
				for (int i = size2; i < highlight.verts.size; i++)
				{
					highlight.uvs.Add(item);
					highlight.cols.Add(item2);
				}
			}
		}
		else
		{
			NGUIText.PrintCaretAndSelection(text, start, end, caret.verts, null);
		}
		ApplyOffset(caret.verts, size);
		Color32 item3 = new Color(caretColor.r, caretColor.g, caretColor.b, caretColor.a * num);
		for (int j = size; j < caret.verts.size; j++)
		{
			caret.uvs.Add(item);
			caret.cols.Add(item3);
		}
	}

	public override void OnFill(BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols)
	{
		if (!isValid)
		{
			return;
		}
		int size = verts.size;
		Color color = base.color;
		color.a = finalAlpha;
		if (mFont != null && mFont.premultipliedAlpha)
		{
			color = NGUITools.ApplyPMA(color);
		}
		string text = processedText;
		float num = ((!(mFont != null)) ? 1f : mFont.pixelSize);
		int size2 = verts.size;
		UpdateNGUIText(defaultFontSize, mWidth, mHeight);
		NGUIText.tint = color;
		NGUIText.Print(text, verts, uvs, cols);
		Vector2 vector = ApplyOffset(verts, size2);
		if (effectStyle != 0)
		{
			int size3 = verts.size;
			float num2 = num;
			vector.x = num2 * mEffectDistance.x;
			vector.y = num2 * mEffectDistance.y;
			ApplyShadow(verts, uvs, cols, size, size3, vector.x, 0f - vector.y);
			if (effectStyle == Effect.Outline)
			{
				size = size3;
				size3 = verts.size;
				ApplyShadow(verts, uvs, cols, size, size3, 0f - vector.x, vector.y);
				size = size3;
				size3 = verts.size;
				ApplyShadow(verts, uvs, cols, size, size3, vector.x, vector.y);
				size = size3;
				size3 = verts.size;
				ApplyShadow(verts, uvs, cols, size, size3, 0f - vector.x, 0f - vector.y);
			}
		}
	}

	protected Vector2 ApplyOffset(BetterList<Vector3> verts, int start)
	{
		Vector2 vector = base.pivotOffset;
		float f = Mathf.Lerp(0f, -mWidth, vector.x);
		float f2 = Mathf.Lerp(mHeight, 0f, vector.y) + Mathf.Lerp(mCalculatedSize.y - (float)mHeight, 0f, vector.y);
		f = Mathf.Round(f);
		f2 = Mathf.Round(f2);
		for (int i = start; i < verts.size; i++)
		{
			verts.buffer[i].x += f;
			verts.buffer[i].y += f2;
		}
		return new Vector2(f, f2);
	}

	private void ApplyShadow(BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols, int start, int end, float x, float y)
	{
		Color color = mEffectColor;
		color.a *= finalAlpha;
		Color32 color2 = ((!(bitmapFont != null) || !bitmapFont.premultipliedAlpha) ? color : NGUITools.ApplyPMA(color));
		for (int i = start; i < end; i++)
		{
			verts.Add(verts.buffer[i]);
			uvs.Add(uvs.buffer[i]);
			cols.Add(cols.buffer[i]);
			Vector3 vector = verts.buffer[i];
			vector.x += x;
			vector.y += y;
			verts.buffer[i] = vector;
			cols.buffer[i] = color2;
		}
	}

	public int CalculateOffsetToFit(string text)
	{
		UpdateNGUIText();
		NGUIText.encoding = false;
		NGUIText.symbolStyle = NGUIText.SymbolStyle.None;
		return NGUIText.CalculateOffsetToFit(text);
	}

	public void SetCurrentProgress()
	{
		if (UIProgressBar.current != null)
		{
			text = UIProgressBar.current.value.ToString("F");
		}
	}

	public void SetCurrentPercent()
	{
		if (UIProgressBar.current != null)
		{
			text = Mathf.RoundToInt(UIProgressBar.current.value * 100f) + "%";
		}
	}

	public void SetCurrentSelection()
	{
		if (UIPopupList.current != null)
		{
			text = ((!UIPopupList.current.isLocalized) ? UIPopupList.current.value : Localization.Get(UIPopupList.current.value));
		}
	}

	public bool Wrap(string text, out string final)
	{
		return Wrap(text, out final, 1000000);
	}

	public bool Wrap(string text, out string final, int height)
	{
		UpdateNGUIText(defaultFontSize, mWidth, height);
		return NGUIText.WrapText(text, out final);
	}

	public void UpdateNGUIText()
	{
		UpdateNGUIText(defaultFontSize, mWidth, mHeight);
	}

	public void UpdateNGUIText(int size, int lineWidth, int lineHeight)
	{
		Font font = trueTypeFont;
		bool flag = font != null;
		NGUIText.fontSize = mPrintedSize;
		NGUIText.fontStyle = mFontStyle;
		NGUIText.rectWidth = lineWidth;
		NGUIText.rectHeight = lineHeight;
		NGUIText.gradient = mApplyGradient;
		NGUIText.gradientTop = mGradientTop;
		NGUIText.gradientBottom = mGradientBottom;
		NGUIText.encoding = mEncoding;
		NGUIText.premultiply = mPremultiply;
		NGUIText.symbolStyle = mSymbols;
		NGUIText.maxLines = mMaxLineCount;
		NGUIText.spacingX = mSpacingX;
		NGUIText.spacingY = mSpacingY;
		NGUIText.fontScale = ((!flag) ? ((float)mFontSize / (float)mFont.defaultSize * mScale * bitmapFont.pixelSize) : mScale);
		if (mFont != null)
		{
			NGUIText.bitmapFont = mFont;
			while (true)
			{
				UIFont replacement = NGUIText.bitmapFont.replacement;
				if (replacement == null)
				{
					break;
				}
				NGUIText.bitmapFont = replacement;
			}
			if (NGUIText.bitmapFont.isDynamic)
			{
				NGUIText.dynamicFont = NGUIText.bitmapFont.dynamicFont;
				NGUIText.bitmapFont = null;
			}
			else
			{
				NGUIText.dynamicFont = null;
			}
		}
		else
		{
			NGUIText.dynamicFont = font;
			NGUIText.bitmapFont = null;
		}
		if (flag && keepCrisp)
		{
			UIRoot uIRoot = base.root;
			if (uIRoot != null)
			{
				NGUIText.pixelDensity = ((!(uIRoot != null)) ? 1f : uIRoot.pixelSizeAdjustment);
			}
		}
		if (alignment == NGUIText.Alignment.Automatic)
		{
			switch (base.pivot)
			{
			case Pivot.TopLeft:
			case Pivot.Left:
			case Pivot.BottomLeft:
				NGUIText.alignment = NGUIText.Alignment.Left;
				break;
			case Pivot.TopRight:
			case Pivot.Right:
			case Pivot.BottomRight:
				NGUIText.alignment = NGUIText.Alignment.Right;
				break;
			default:
				NGUIText.alignment = NGUIText.Alignment.Center;
				break;
			}
		}
		else
		{
			NGUIText.alignment = alignment;
		}
		NGUIText.Update();
	}
}

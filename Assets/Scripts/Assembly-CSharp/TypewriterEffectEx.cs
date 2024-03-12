using UnityEngine;

public class TypewriterEffectEx : MonoBehaviour
{
	public int charsPerSecond = 60;

	private UILabel mLabel;

	private string mText;

	private int mOffset;

	private float mNextChar;

	private void Start()
	{
	}

	private void Update()
	{
		if (mLabel == null)
		{
			mLabel = GetComponent<UILabel>();
			mLabel.supportEncoding = false;
			mLabel.symbolStyle = NGUIText.SymbolStyle.None;
			mText = mLabel.processedText;
		}
		if (mOffset < mText.Length && mNextChar <= RealTime.time)
		{
			charsPerSecond = Mathf.Max(1, charsPerSecond);
			float num = 1f / (float)charsPerSecond;
			char c = mText[mOffset];
			if (c == '.' || c == '\n' || c == '!' || c == '?')
			{
				num *= 4f;
			}
			mNextChar = RealTime.time + num;
			mLabel.text = mText.Substring(0, ++mOffset);
		}
	}

	public void Reset()
	{
		if (mLabel == null)
		{
			mLabel = GetComponent<UILabel>();
			mLabel.supportEncoding = false;
			mLabel.symbolStyle = NGUIText.SymbolStyle.None;
		}
		mText = mLabel.text;
		mLabel.text = string.Empty;
		mOffset = 0;
		mNextChar = 0f;
	}

	public bool ShowFinish()
	{
		return mOffset >= mText.Length;
	}
}

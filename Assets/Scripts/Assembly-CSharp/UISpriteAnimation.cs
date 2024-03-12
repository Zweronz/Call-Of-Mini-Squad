using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(UISprite))]
[AddComponentMenu("NGUI/UI/Sprite Animation")]
public class UISpriteAnimation : MonoBehaviour
{
	[HideInInspector]
	[SerializeField]
	private int mFPS = 30;

	[HideInInspector]
	[SerializeField]
	private string mPrefix = string.Empty;

	[SerializeField]
	[HideInInspector]
	private bool mLoop = true;

	private UISprite mSprite;

	private float mDelta;

	private int mIndex;

	private bool mActive = true;

	private List<string> mSpriteNames = new List<string>();

	public int frames
	{
		get
		{
			return mSpriteNames.Count;
		}
	}

	public int framesPerSecond
	{
		get
		{
			return mFPS;
		}
		set
		{
			mFPS = value;
		}
	}

	public string namePrefix
	{
		get
		{
			return mPrefix;
		}
		set
		{
			if (mPrefix != value)
			{
				mPrefix = value;
				RebuildSpriteList();
			}
		}
	}

	public bool loop
	{
		get
		{
			return mLoop;
		}
		set
		{
			mLoop = value;
		}
	}

	public bool isPlaying
	{
		get
		{
			return mActive;
		}
	}

	private void Start()
	{
		RebuildSpriteList();
	}

	private void Update()
	{
		if (!mActive || mSpriteNames.Count <= 1 || !Application.isPlaying || !((float)mFPS > 0f))
		{
			return;
		}
		mDelta += RealTime.deltaTime;
		float num = 1f / (float)mFPS;
		if (num < mDelta)
		{
			mDelta = ((!(num > 0f)) ? 0f : (mDelta - num));
			if (++mIndex >= mSpriteNames.Count)
			{
				mIndex = 0;
				mActive = loop;
			}
			if (mActive)
			{
				mSprite.spriteName = mSpriteNames[mIndex];
				mSprite.MakePixelPerfect();
			}
		}
	}

	private void RebuildSpriteList()
	{
		if (mSprite == null)
		{
			mSprite = GetComponent<UISprite>();
		}
		mSpriteNames.Clear();
		if (!(mSprite != null) || !(mSprite.atlas != null))
		{
			return;
		}
		List<UISpriteData> spriteList = mSprite.atlas.spriteList;
		int i = 0;
		for (int count = spriteList.Count; i < count; i++)
		{
			UISpriteData uISpriteData = spriteList[i];
			if (string.IsNullOrEmpty(mPrefix) || uISpriteData.name.StartsWith(mPrefix))
			{
				mSpriteNames.Add(uISpriteData.name);
			}
		}
		mSpriteNames.Sort();
	}

	public void Reset()
	{
		mActive = true;
		mIndex = 0;
		if (mSprite != null && mSpriteNames.Count > 0)
		{
			mSprite.spriteName = mSpriteNames[mIndex];
			mSprite.MakePixelPerfect();
		}
	}
}

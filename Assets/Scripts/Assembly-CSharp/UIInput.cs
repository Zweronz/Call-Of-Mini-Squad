using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[AddComponentMenu("NGUI/UI/Input Field")]
public class UIInput : MonoBehaviour
{
	public enum InputType
	{
		Standard = 0,
		AutoCorrect = 1,
		Password = 2
	}

	public enum Validation
	{
		None = 0,
		Integer = 1,
		Float = 2,
		Alphanumeric = 3,
		Username = 4,
		Name = 5
	}

	public enum KeyboardType
	{
		Default = 0,
		ASCIICapable = 1,
		NumbersAndPunctuation = 2,
		URL = 3,
		NumberPad = 4,
		PhonePad = 5,
		NamePhonePad = 6,
		EmailAddress = 7
	}

	public delegate char OnValidate(string text, int charIndex, char addedChar);

	public static UIInput current;

	public static UIInput selection;

	public UILabel label;

	public InputType inputType;

	public KeyboardType keyboardType;

	public Validation validation;

	public int characterLimit;

	public string savedAs;

	public GameObject selectOnTab;

	public Color activeTextColor = Color.white;

	public Color caretColor = new Color(1f, 1f, 1f, 0.8f);

	public Color selectionColor = new Color(1f, 0.8745098f, 47f / 85f, 0.5f);

	public List<EventDelegate> onSubmit = new List<EventDelegate>();

	public List<EventDelegate> onChange = new List<EventDelegate>();

	public OnValidate onValidate;

	[SerializeField]
	[HideInInspector]
	protected string mValue;

	protected string mDefaultText = string.Empty;

	protected Color mDefaultColor = Color.white;

	protected float mPosition;

	protected bool mDoInit = true;

	protected UIWidget.Pivot mPivot;

	protected static int mDrawStart;

	protected static TouchScreenKeyboard mKeyboard;

	private string mCached = string.Empty;

	public string defaultText
	{
		get
		{
			return mDefaultText;
		}
		set
		{
			if (mDoInit)
			{
				Init();
			}
			mDefaultText = value;
			UpdateLabel();
		}
	}

	[Obsolete("Use UIInput.value instead")]
	public string text
	{
		get
		{
			return value;
		}
		set
		{
			this.value = value;
		}
	}

	public string value
	{
		get
		{
			if (mDoInit)
			{
				Init();
			}
			return mValue;
		}
		set
		{
			try
			{
				if (mDoInit)
				{
					Init();
				}
				mDrawStart = 0;
				if (Application.platform == RuntimePlatform.BB10Player)
				{
					value = value.Replace("\\b", "\b");
				}
				value = Validate(value);
				if (isSelected && mKeyboard != null && mCached != value)
				{
					mKeyboard.text = value;
					mCached = value;
				}
				if (mValue != value)
				{
					mValue = value;
					if (!isSelected)
					{
						SaveToPlayerPrefs(value);
					}
					UpdateLabel();
					ExecuteOnChange();
				}
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
		}
	}

	[Obsolete("Use UIInput.isSelected instead")]
	public bool selected
	{
		get
		{
			return isSelected;
		}
		set
		{
			isSelected = value;
		}
	}

	public bool isSelected
	{
		get
		{
			return selection == this;
		}
		set
		{
			if (!value)
			{
				if (isSelected)
				{
					UICamera.selectedObject = null;
				}
			}
			else
			{
				UICamera.selectedObject = base.gameObject;
			}
		}
	}

	protected int cursorPosition
	{
		get
		{
			return value.Length;
		}
	}

	public string Validate(string val)
	{
		try
		{
			if (string.IsNullOrEmpty(val))
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder(val.Length);
			for (int i = 0; i < val.Length; i++)
			{
				char c = val[i];
				if (onValidate != null)
				{
					c = onValidate(stringBuilder.ToString(), stringBuilder.Length, c);
				}
				else if (validation != 0)
				{
					c = Validate(stringBuilder.ToString(), stringBuilder.Length, c);
				}
				if (c != 0)
				{
					stringBuilder.Append(c);
				}
			}
			if (characterLimit > 0 && stringBuilder.Length > characterLimit)
			{
				return stringBuilder.ToString(0, characterLimit);
			}
			return stringBuilder.ToString();
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
			return string.Empty;
		}
	}

	private void Start()
	{
		if (string.IsNullOrEmpty(mValue))
		{
			if (!string.IsNullOrEmpty(savedAs) && PlayerPrefs.HasKey(savedAs))
			{
				value = PlayerPrefs.GetString(savedAs);
			}
		}
		else
		{
			value = mValue.Replace("\\n", "\n");
		}
	}

	protected void Init()
	{
		if (mDoInit && label != null)
		{
			mDoInit = false;
			mDefaultText = label.text;
			mDefaultColor = label.color;
			label.supportEncoding = false;
			if (label.alignment == NGUIText.Alignment.Justified)
			{
				label.alignment = NGUIText.Alignment.Left;
				Debug.LogWarning("Input fields using labels with justified alignment are not supported at this time", this);
			}
			mPivot = label.pivot;
			mPosition = label.cachedTransform.localPosition.x;
			UpdateLabel();
		}
	}

	protected void SaveToPlayerPrefs(string val)
	{
		if (!string.IsNullOrEmpty(savedAs))
		{
			if (string.IsNullOrEmpty(val))
			{
				PlayerPrefs.DeleteKey(savedAs);
			}
			else
			{
				PlayerPrefs.SetString(savedAs, val);
			}
		}
	}

	protected virtual void OnSelect(bool isSelected)
	{
		if (isSelected)
		{
			OnSelectEvent();
		}
		else
		{
			OnDeselectEvent();
		}
	}

	protected void OnSelectEvent()
	{
		selection = this;
		if (mDoInit)
		{
			Init();
		}
		if (label != null && NGUITools.GetActive(this))
		{
			label.color = activeTextColor;
			if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android)
			{
				mKeyboard = ((inputType != InputType.Password) ? TouchScreenKeyboard.Open(mValue, (TouchScreenKeyboardType)keyboardType, inputType == InputType.AutoCorrect, label.multiLine, false, false, defaultText) : TouchScreenKeyboard.Open(mValue, TouchScreenKeyboardType.Default, false, false, true));
			}
			else
			{
				Vector2 compositionCursorPos = ((!(UICamera.current != null) || !(UICamera.current.cachedCamera != null)) ? label.worldCorners[0] : UICamera.current.cachedCamera.WorldToScreenPoint(label.worldCorners[0]));
				compositionCursorPos.y = (float)Screen.height - compositionCursorPos.y;
				Input.imeCompositionMode = IMECompositionMode.On;
				Input.compositionCursorPos = compositionCursorPos;
				mDrawStart = 0;
			}
			UpdateLabel();
		}
	}

	protected void OnDeselectEvent()
	{
		if (mDoInit)
		{
			Init();
		}
		if (label != null && NGUITools.GetActive(this))
		{
			mValue = value;
			if (mKeyboard != null)
			{
				mKeyboard.active = false;
				mKeyboard = null;
			}
			if (string.IsNullOrEmpty(mValue))
			{
				label.text = mDefaultText;
				label.color = mDefaultColor;
			}
			else
			{
				label.text = mValue;
			}
			Input.imeCompositionMode = IMECompositionMode.Auto;
			RestoreLabelPivot();
		}
		selection = null;
		UpdateLabel();
	}

	private void Update()
	{
		if (mKeyboard == null || !isSelected)
		{
			return;
		}
		string text = mKeyboard.text;
		if (mCached != text)
		{
			mCached = text;
			value = text;
		}
		if (mKeyboard.done)
		{
			if (!mKeyboard.wasCanceled)
			{
				Submit();
			}
			mKeyboard = null;
			isSelected = false;
			mCached = string.Empty;
		}
	}

	public void Submit()
	{
		if (NGUITools.GetActive(this))
		{
			current = this;
			mValue = value;
			EventDelegate.Execute(onSubmit);
			SaveToPlayerPrefs(mValue);
			current = null;
		}
	}

	public void UpdateLabel()
	{
		try
		{
			if (!(label != null))
			{
				return;
			}
			if (mDoInit)
			{
				Init();
			}
			bool flag = isSelected;
			string text = value;
			bool flag2 = string.IsNullOrEmpty(text) && string.IsNullOrEmpty(Input.compositionString);
			label.color = ((!flag2 || flag) ? activeTextColor : mDefaultColor);
			string text2;
			if (flag2)
			{
				text2 = ((!flag) ? mDefaultText : string.Empty);
				RestoreLabelPivot();
			}
			else
			{
				if (inputType == InputType.Password)
				{
					text2 = string.Empty;
					int i = 0;
					for (int length = text.Length; i < length; i++)
					{
						text2 += "*";
					}
				}
				else
				{
					text2 = text;
				}
				int num = (flag ? Mathf.Min(text2.Length, cursorPosition) : 0);
				string text3 = text2.Substring(0, num);
				if (flag)
				{
					text3 += Input.compositionString;
				}
				text2 = text3 + text2.Substring(num, text2.Length - num);
				if (flag && label.overflowMethod == UILabel.Overflow.ClampContent)
				{
					int num2 = label.CalculateOffsetToFit(text2);
					if (num2 == 0)
					{
						mDrawStart = 0;
						RestoreLabelPivot();
					}
					else if (num < mDrawStart)
					{
						mDrawStart = num;
						SetPivotToLeft();
					}
					else if (num2 < mDrawStart)
					{
						mDrawStart = num2;
						SetPivotToLeft();
					}
					else
					{
						num2 = label.CalculateOffsetToFit(text2.Substring(0, num));
						if (num2 > mDrawStart)
						{
							mDrawStart = num2;
							SetPivotToRight();
						}
					}
					if (mDrawStart != 0)
					{
						text2 = text2.Substring(mDrawStart, text2.Length - mDrawStart);
					}
				}
				else
				{
					mDrawStart = 0;
					RestoreLabelPivot();
				}
			}
			label.text = text2;
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
	}

	protected void SetPivotToLeft()
	{
		Vector2 pivotOffset = NGUIMath.GetPivotOffset(mPivot);
		pivotOffset.x = 0f;
		label.pivot = NGUIMath.GetPivot(pivotOffset);
	}

	protected void SetPivotToRight()
	{
		Vector2 pivotOffset = NGUIMath.GetPivotOffset(mPivot);
		pivotOffset.x = 1f;
		label.pivot = NGUIMath.GetPivot(pivotOffset);
	}

	protected void RestoreLabelPivot()
	{
		if (label != null && label.pivot != mPivot)
		{
			label.pivot = mPivot;
		}
	}

	protected char Validate(string text, int pos, char ch)
	{
		try
		{
			if (validation == Validation.None || !base.enabled)
			{
				return ch;
			}
			if (validation == Validation.Integer)
			{
				if (ch >= '0' && ch <= '9')
				{
					return ch;
				}
				if (ch == '-' && pos == 0 && !text.Contains("-"))
				{
					return ch;
				}
			}
			else if (validation == Validation.Float)
			{
				if (ch >= '0' && ch <= '9')
				{
					return ch;
				}
				if (ch == '-' && pos == 0 && !text.Contains("-"))
				{
					return ch;
				}
				if (ch == '.' && !text.Contains("."))
				{
					return ch;
				}
			}
			else if (validation == Validation.Alphanumeric)
			{
				if (ch >= 'A' && ch <= 'Z')
				{
					return ch;
				}
				if (ch >= 'a' && ch <= 'z')
				{
					return ch;
				}
				if (ch >= '0' && ch <= '9')
				{
					return ch;
				}
			}
			else if (validation == Validation.Username)
			{
				if (ch >= 'A' && ch <= 'Z')
				{
					return (char)(ch - 65 + 97);
				}
				if (ch >= 'a' && ch <= 'z')
				{
					return ch;
				}
				if (ch >= '0' && ch <= '9')
				{
					return ch;
				}
			}
			else if (validation == Validation.Name)
			{
				char c = ((text.Length <= 0) ? ' ' : text[Mathf.Clamp(pos, 0, text.Length - 1)]);
				char c2 = ((text.Length <= 0) ? '\n' : text[Mathf.Clamp(pos + 1, 0, text.Length - 1)]);
				if (ch >= 'a' && ch <= 'z')
				{
					if (c == ' ')
					{
						return (char)(ch - 97 + 65);
					}
					return ch;
				}
				if (ch >= 'A' && ch <= 'Z')
				{
					if (c != ' ' && c != '\'')
					{
						return (char)(ch - 65 + 97);
					}
					return ch;
				}
				switch (ch)
				{
				case '\'':
					if (c != ' ' && c != '\'' && c2 != '\'' && !text.Contains("'"))
					{
						return ch;
					}
					break;
				case ' ':
					if (c != ' ' && c != '\'' && c2 != ' ' && c2 != '\'')
					{
						return ch;
					}
					break;
				}
			}
			return '\0';
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
			return '\0';
		}
	}

	protected void ExecuteOnChange()
	{
		if (EventDelegate.IsValid(onChange))
		{
			current = this;
			EventDelegate.Execute(onChange);
			current = null;
		}
	}
}

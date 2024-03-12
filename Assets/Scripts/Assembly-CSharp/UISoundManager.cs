using UnityEngine;

public class UISoundManager : MonoBehaviour
{
	private static UISoundManager mInstance;

	public UIPlaySound levelUpSound;

	public UIPlaySound breakSound;

	public static UISoundManager Instance
	{
		get
		{
			if (mInstance == null)
			{
				mInstance = GameObject.Find("UISound").GetComponent<UISoundManager>();
			}
			return mInstance;
		}
	}

	public void PlayLevelUpSound()
	{
		levelUpSound.Play();
	}

	public void PlayBreakSound()
	{
		breakSound.Play();
	}
}

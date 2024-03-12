using System.Collections;
using UnityEngine;

public class KamcordAndroidPostRender : MonoBehaviour
{
	private IEnumerator OnPostRender()
	{
		yield return new WaitForEndOfFrame();
		Kamcord.EndDraw();
	}
}

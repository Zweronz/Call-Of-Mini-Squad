using UnityEngine;

public class UtilUIBase3DMapStateControlInfo : MonoBehaviour
{
	public GameObject bUnlockEffctGO;

	public GameObject[] bCurrentEffectGO;

	public void SetState(bool bCurrent, bool bUnlock, bool bClear)
	{
		bUnlockEffctGO.SetActive(false);
		for (int i = 0; i < bCurrentEffectGO.Length; i++)
		{
			bCurrentEffectGO[i].SetActive(false);
		}
		if (bClear)
		{
			bUnlockEffctGO.SetActive(true);
			bUnlockEffctGO.GetComponent<Renderer>().material.SetColor("_Color", UIUtil._UIBASEMAPYELLOWColor);
		}
		else if (bCurrent)
		{
			for (int j = 0; j < bCurrentEffectGO.Length; j++)
			{
				bCurrentEffectGO[j].SetActive(true);
			}
		}
		else
		{
			bUnlockEffctGO.SetActive(true);
			bUnlockEffctGO.GetComponent<Renderer>().material.SetColor("_Color", UIUtil._UIBASEMAPGrayColor);
		}
	}
}

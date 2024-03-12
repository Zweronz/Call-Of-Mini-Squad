using UnityEngine;

public class SceneLoading : MonoBehaviour
{
	private bool m_bLoaded;

	private float m_timer;

	private AsyncOperation m_unloadProcess;

	private AsyncOperation m_loadProcess;

	public UITexture characterIcon;

	public UILabel tips;

	public void Start()
	{
		m_bLoaded = false;
		m_timer = 0f;
		m_unloadProcess = Resources.UnloadUnusedAssets();
		FadeInfoScript.Instance.transform.position = Vector3.zero;
		int num = Random.Range(0, Defined.LoadingIcons.Length);
		characterIcon.mainTexture = Resources.Load("UI/Loading/tex/" + Defined.LoadingIcons[num]) as Texture;
		characterIcon.MakePixelPerfect();
		characterIcon.transform.localScale = new Vector3(0.7f, 0.7f, 1f);
		string[] loadingTips = DataCenter.Conf().GetLoadingTips();
		num = Random.Range(0, loadingTips.Length);
		tips.text = loadingTips[num];
	}

	public void Update()
	{
		if (SceneLoadingManager.s_bSwitch)
		{
			if (!m_bLoaded)
			{
				m_bLoaded = true;
				m_loadProcess = Application.LoadLevelAsync(SceneLoadingManager.s_nextSceneName);
				SceneLoadingManager.s_currSceneName = SceneLoadingManager.s_nextSceneName;
			}
			if (m_loadProcess.isDone)
			{
				SceneLoadingManager.s_currSceneName = SceneLoadingManager.s_nextSceneName;
			}
		}
	}
}

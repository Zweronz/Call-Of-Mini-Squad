using System.Collections;
using UnityEngine;

public class LoadObb : MonoBehaviour
{
	private string expPath;

	private string logtxt;

	private bool alreadyLogged;

	private string nextScene = "PreStartUI";

	private bool downloadStarted;

	private bool loadingkey;

	public GUISkin mySkin;

	private void log(string t)
	{
		logtxt = logtxt + t + "\n";
		MonoBehaviour.print("MYLOG " + t);
	}

	private void Start()
	{
	}

	private void OnGUI()
	{
		GUI.skin = mySkin;
		if (!GooglePlayDownloader.RunningOnAndroid())
		{
			GUI.Label(new Rect(10f, 10f, Screen.width - 10, 20f), "Use GooglePlayDownloader only on Android device!");
			return;
		}
		expPath = GooglePlayDownloader.GetExpansionFilePath();
		if (expPath == null)
		{
			GUI.Label(new Rect(10f, 10f, Screen.width - 10, 20f), "External storage is not available!");
			return;
		}
		string mainOBBPath = GooglePlayDownloader.GetMainOBBPath(expPath);
		string patchOBBPath = GooglePlayDownloader.GetPatchOBBPath(expPath);
		if (!alreadyLogged)
		{
			alreadyLogged = true;
			log("expPath = " + expPath);
			log("Main = " + mainOBBPath);
			log("Main = " + mainOBBPath.Substring(expPath.Length));
			if (mainOBBPath != null)
			{
				StartCoroutine(loadLevel());
			}
		}
		if (mainOBBPath == null)
		{
			if (!loadingkey)
			{
				loadingkey = true;
				GooglePlayDownloader.FetchOBB();
				StartCoroutine(loadLevel());
			}
		}
		else
		{
			StartCoroutine(loadLevel());
		}
	}

	protected IEnumerator loadLevel()
	{
		string mainPath;
		do
		{
			yield return new WaitForSeconds(0.5f);
			mainPath = GooglePlayDownloader.GetMainOBBPath(expPath);
			log("waiting mainPath " + mainPath);
		}
		while (mainPath == null);
		if (!downloadStarted)
		{
			downloadStarted = true;
			string uri = "file://" + mainPath;
			log("downloading " + uri);
			WWW www = WWW.LoadFromCacheOrDownload(uri, 0);
			yield return www;
			if (www.error != null)
			{
				log("wwww error " + www.error);
			}
			else
			{
				Application.LoadLevel(nextScene);
			}
		}
	}
}

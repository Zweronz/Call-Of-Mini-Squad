using System;
using System.Collections.Generic;
using UnityEngine;

public class COMI_VideoController : MonoBehaviour
{
	private readonly string appName = "CoM Infinity";

	private readonly string devKey = "upZsIdbOoCrxeDQAiZ7UvZHirLwnRt3nstEQYnTegQO";

	private readonly string devSecret = "34Y4iD1FmmxhyanHRwFKrwOElWzBeVumf2zebbFNyba";

	private static COMI_VideoController sInstance;

	private Dictionary<string, object> mVideoMetaData;

	[NonSerialized]
	private bool mbRecorded;

	private bool mbRecording;

	public static COMI_VideoController Instance
	{
		get
		{
			return sInstance;
		}
	}

	private void Awake()
	{
		sInstance = this;
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		Kamcord.Init(devKey, devSecret, appName, Kamcord.VideoQuality.Standard);
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void OnDisable()
	{
	}

	public void Init(string deviceId, string nickName, int level)
	{
		mbRecorded = false;
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("ID", deviceId);
		dictionary.Add("Name", nickName);
		dictionary.Add("Level", level);
		mVideoMetaData = dictionary;
	}

	public void SwitchState()
	{
		if (mbRecording)
		{
			StopRecording();
		}
		else
		{
			StartRecording();
		}
		mbRecording = !mbRecording;
	}

	public void EndRecord()
	{
		if (mbRecording)
		{
			mbRecording = false;
			mbRecorded = true;
			StopRecording();
			SetMetadata(mVideoMetaData);
		}
	}

	public bool IsRecording()
	{
		return mbRecording;
	}

	public bool HaveLastRecord()
	{
		return mbRecorded;
	}

	public void PlayLastRecord()
	{
		Kamcord.ShowView();
	}

	public void ShowMainView()
	{
		Kamcord.ShowWatchView();
	}

	private void StartRecording()
	{
		Kamcord.StartRecording();
	}

	private void StopRecording()
	{
		Kamcord.StopRecording();
	}

	private void SetMetadata(string key, object val)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add(key, val);
		SetMetadata(dictionary);
	}

	private void SetMetadata(Dictionary<string, object> metadata)
	{
		Kamcord.SetVideoTitle("CoM Infinity Gameplay");
		Kamcord.SetVideoMetadata(metadata);
	}
}

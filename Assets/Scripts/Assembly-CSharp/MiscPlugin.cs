using System;
using System.IO;
using UnityEngine;

public class MiscPlugin
{
	public static void TakePhoto(string save_path, string photo_key)
	{
		try
		{
			Texture2D texture2D = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
			texture2D.ReadPixels(new Rect(0f, 0f, Screen.width, Screen.height), 0, 0);
			texture2D.Apply();
			byte[] buffer = texture2D.EncodeToPNG();
			string path = save_path + "/" + photo_key + "_photo.png";
			FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.Write);
			BinaryWriter binaryWriter = new BinaryWriter(fileStream);
			binaryWriter.Write(buffer);
			binaryWriter.Close();
			fileStream.Close();
			UnityEngine.Object.Destroy(texture2D);
		}
		catch
		{
		}
	}

	public static void ToSendMail(string address, string subject, string content)
	{
	}

	public static int ShowMessageBox1(string title, string message, string button)
	{
		return 0;
	}

	public static int ShowMessageBox2(string title, string message, string button1, string button2)
	{
		return 0;
	}

	public static string GetMacAddr()
	{
		return "000000000000";
	}

	public static void SavePhoto(int photo_index, int width, int height)
	{
	}

	public static long GetSystemSecond()
	{
		TimeSpan timeSpan = new TimeSpan(DateTime.UtcNow.Ticks - new DateTime(1970, 1, 1, 0, 0, 0).Ticks);
		return (long)timeSpan.TotalSeconds;
	}

	public static int OnCheckPhotoSaveStatus()
	{
		return 1;
	}

	public static void OnResetPhotoSaveStatus()
	{
	}

	public static void OpenLocalCameraRoll()
	{
	}

	public static void ShowIndicatorSystem(int style, bool iPad, float r, float g, float b, float a)
	{
	}

	public static void ShowIndicatorSystem_int(int style, int iPad, float r, float g, float b, float a)
	{
	}

	public static void HideIndicatorSystem()
	{
	}

	public static int GetIOSYear()
	{
		return DateTime.Now.Year;
	}

	public static int GetIOSMonth()
	{
		return DateTime.Now.Month;
	}

	public static int GetIOSDay()
	{
		return DateTime.Now.Day;
	}

	public static int GetIOSHour()
	{
		return DateTime.Now.Hour;
	}

	public static int GetIOSMin()
	{
		return DateTime.Now.Minute;
	}

	public static int GetIOSSec()
	{
		return DateTime.Now.Second;
	}

	public static bool IsJailbreak()
	{
		return true;
	}

	public static bool IsIAPCrack()
	{
		return false;
	}
}

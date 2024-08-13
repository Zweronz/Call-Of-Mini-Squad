using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using UnityEngine;

namespace CoMDS2
{
	public class Util
	{
		public const string ENCRYPT_KEY = "A;g^L%S&K*7620";

		public const string ENCRYPT_KEY1 = "B;g^L%S&K*7630";

		public const string ENCRYPT_KEY2 = "C;g^L%S&K*7640";

		public const string ENCRYPT_KEY3 = "D;g^L%S&K*7650";

		public static bool s_debug = false;

		public static bool s_autoControl = false;

		public static bool s_debugBuild = true;

		public static bool s_pvp_atutoPlay = false;

		public static bool s_allyMoveAttack = true;

		public static bool s_squadMode = true;

		public static bool s_cheatControl = false;

		public static bool s_cheatGodMode = false;

		public static float s_cheatAddDamage = 1f;

		private static long m_serverTime = -1L;

		public static Color s_color_posion = new Color(0f, 1f, 0f);

		public static Color s_color_curse = new Color(0.3f, 0.3f, 0.3f);

		public static Color s_color_bleeding = new Color(1f, 0f, 0f);

		public static Color s_color_elite_yellow = new Color(14f / 15f, 47f / 51f, 0.4745098f, 1f);

		public static Color s_color_elite_green = new Color(0f, 47f / 51f, 0f, 1f);

		public static Vector3 s_compareRaycastHitPosition = Vector3.zero;

		public static float[,] s_intersectPoints = new float[2, 2];

		public static long ServerTime
		{
			set
			{
				m_serverTime = value;
			}
		}

		public static bool RaycastHitFirstTarget(RaycastHit[] hits, Vector3 position, out RaycastHit firstHit)
		{
			bool result = false;
			firstHit = default(RaycastHit);
			if (hits == null || hits.Length == 0)
			{
				return result;
			}
			float num = float.PositiveInfinity;
			for (int i = 0; i < hits.Length; i++)
			{
				RaycastHit raycastHit = hits[i];
				float sqrMagnitude = (raycastHit.collider.gameObject.transform.position - position).sqrMagnitude;
				if (sqrMagnitude < num)
				{
					firstHit = raycastHit;
					num = sqrMagnitude;
					result = true;
				}
			}
			return result;
		}

		public static void SortHitListFromNearToFar(RaycastHit[] hits)
		{
			if (hits != null && hits.Length != 0)
			{
				Array.Sort(hits, CompareRaycastHit);
			}
		}

		public static int CompareRaycastHit(RaycastHit lhs, RaycastHit rhs)
		{
			float sqrMagnitude = (lhs.collider.transform.position - s_compareRaycastHitPosition).sqrMagnitude;
			float sqrMagnitude2 = (rhs.collider.transform.position - s_compareRaycastHitPosition).sqrMagnitude;
			if (sqrMagnitude < sqrMagnitude2)
			{
				return -1;
			}
			if (sqrMagnitude > sqrMagnitude2)
			{
				return 1;
			}
			return 0;
		}

		public static int GetIntersectPoint(float i, float j, float k, float l, Rect rect)
		{
			float x = rect.x;
			float num = rect.x + rect.width;
			float y = rect.y;
			float num2 = rect.y + rect.height;
			float num3;
			float num4;
			float num5;
			float num6;
			if (i - k == 0f)
			{
				if (k < x || k > num)
				{
					return 0;
				}
				num3 = k;
				num4 = y;
				num5 = k;
				num6 = num2;
			}
			else
			{
				float num7 = (j - l) / (i - k);
				if (num7 == 0f)
				{
					if (l < y || l > num2)
					{
						return 0;
					}
					num3 = x;
					num4 = l;
					num5 = num;
					num6 = l;
				}
				else
				{
					float num8 = 0f - num7 * k + l;
					num3 = x;
					num4 = num7 * num3 + num8;
					if (num4 < y || num4 > num2)
					{
						num4 = ((!(num4 >= y)) ? y : num2);
						num3 = (num8 - num4) / (0f - num7);
						if (num3 < x || num3 > num)
						{
							return 0;
						}
					}
					num5 = num;
					num6 = num7 * num5 + num8;
					if (num6 < y || num6 > num2)
					{
						num6 = ((!(num6 >= y)) ? y : num2);
						num5 = (num8 - num6) / (0f - num7);
						if (num5 < x || num5 > num)
						{
							return 0;
						}
					}
				}
			}
			s_intersectPoints[0, 0] = num3;
			s_intersectPoints[0, 1] = num4;
			s_intersectPoints[1, 0] = num5;
			s_intersectPoints[1, 1] = num6;
			return (num3 == num5 && num4 == num6) ? 2 : 4;
		}

		public static bool IsPointOnLine(Vector2 point, Line line, bool segment = false)
		{
			if (segment)
			{
				float num;
				float num2;
				if (line.x1 <= line.x2)
				{
					num = line.x1;
					num2 = line.x2;
				}
				else
				{
					num = line.x2;
					num2 = line.x1;
				}
				float num3;
				float num4;
				if (line.y1 <= line.y2)
				{
					num3 = line.y1;
					num4 = line.y2;
				}
				else
				{
					num3 = line.y2;
					num4 = line.y1;
				}
				if (point.x < num || point.x > num2 || point.y < num3 || point.y > num4)
				{
					return false;
				}
			}
			if (line.x2 - line.x1 == 0f || line.y2 - line.y1 == 0f)
			{
				return true;
			}
			float num5 = (line.y2 - line.y1) / (line.x2 - line.x1);
			float num6 = line.y1 - num5 * line.x1;
			float num7 = Mathf.Abs(point.y - (num5 * point.x + num6));
			return num7 < 0.1f;
		}

		public static void ZipString(string content, ref string zipedcontent)
		{
			if (content.Length >= 1)
			{
				byte[] bytes = Encoding.UTF8.GetBytes(content);
				MemoryStream memoryStream = new MemoryStream();
				DeflaterOutputStream deflaterOutputStream = new DeflaterOutputStream(memoryStream);
				deflaterOutputStream.Write(bytes, 0, bytes.Length);
				deflaterOutputStream.Close();
				bytes = memoryStream.ToArray();
				zipedcontent = Convert.ToBase64String(bytes);
			}
		}

		public static void UnZipString(string content, ref string unzipedcontent)
		{
			if (content.Length >= 1)
			{
				byte[] array = Convert.FromBase64String(content);
				InflaterInputStream inflaterInputStream = new InflaterInputStream(new MemoryStream(array, 0, array.Length));
				MemoryStream memoryStream = new MemoryStream();
				int num = 0;
				byte[] array2 = new byte[4096];
				while ((num = inflaterInputStream.Read(array2, 0, array2.Length)) != 0)
				{
					memoryStream.Write(array2, 0, num);
				}
				unzipedcontent = Encoding.UTF8.GetString(memoryStream.ToArray());
			}
		}

		public static string GetMD5(string strContent)
		{
			MD5 mD = new MD5CryptoServiceProvider();
			byte[] bytes = Encoding.Default.GetBytes(strContent);
			byte[] array = mD.ComputeHash(bytes);
			return BitConverter.ToString(array).Replace("-", string.Empty);
		}

		public static string EncryptData(string input_data, string encrypt_key = "A;g^L%S&K*7620")
		{
			string empty = string.Empty;
			try
			{
				byte[] inArray = XXTEAUtils.Encrypt(Encoding.UTF8.GetBytes(input_data), Encoding.UTF8.GetBytes(encrypt_key));
				return Convert.ToBase64String(inArray);
			}
			catch (Exception)
			{
				return string.Empty;
			}
		}

		public static string DecryptData(string input_data, string encrypt_key = "A;g^L%S&K*7620")
		{
			string empty = string.Empty;
			try
			{
				byte[] data = Convert.FromBase64String(input_data);
				byte[] bytes = XXTEAUtils.Decrypt(data, Encoding.UTF8.GetBytes(encrypt_key));
				return Encoding.UTF8.GetString(bytes);
			}
			catch (Exception)
			{
				return string.Empty;
			}
		}

		public static int EncryptInt(int value)
		{
			return ~value ^ 0xA1B2C3;
		}

		public static int DecryptInt(int value)
		{
			return ~(value ^ 0xA1B2C3);
		}

		public static bool IsNetworkConnected()
		{
			if (Application.internetReachability == NetworkReachability.NotReachable)
			{
				return false;
			}
			return true;
		}

		public static Vector3 ScreenPointToNGUI(Vector3 p)
		{
			Vector3 result = new Vector3(p.x - (float)(Screen.width / 2), p.y - (float)(Screen.height / 2), p.z);
			float num = Mathf.Max(Screen.width, Screen.height);
			float num2 = Mathf.Min(Screen.width, Screen.height);
			if (GameBattle.m_instance != null)
			{
				result.x *= 768f / (float)Screen.height;
				result.y *= 768f / (float)Screen.height;
			}
			else if ((float)Screen.height > 768f)
			{
				result.x *= 768f / (float)Screen.height;
				result.y *= 768f / (float)Screen.height;
			}
			return result;
		}

		public static Vector3 ScreenPointToNGUIForAnroid(Vector3 p)
		{
			Vector3 result = new Vector3(p.x - (float)(Screen.width / 2), p.y - (float)(Screen.height / 2), p.z);
			result.x *= 768f / (float)Screen.height;
			result.y *= 768f / (float)Screen.height;
			return result;
		}

		public static DateTime GetServerTime()
		{
			DateTime dateTime = new DateTime(1970, 1, 1);
			long num = HttpRequestHandle.instance.serverTimeSeconds + Mathf.FloorToInt(Time.realtimeSinceStartup);
			TimeSpan value = new TimeSpan(num * 10000000);
			return dateTime.Add(value);
		}

		public static int GetLeftSecondsToday()
		{
			DateTime serverTime = GetServerTime();
			int num = 86400;
			int num2 = serverTime.Hour * 60 * 60 + serverTime.Minute * 60 + serverTime.Second;
			return num - num2;
		}

		public static Material LoadEquipIconMaterial(string fileName)
		{
			return Resources.Load<Material>("EquipMaterial/" + fileName);
		}

		public static void PlayCG(bool bMustWatchFinished)
		{
			UIConstant.bNeedLoseConnect = false;
			if (bMustWatchFinished)
			{
			//	Handheld.PlayFullScreenMovie("squad1.mp4", Color.black, FullScreenMovieControlMode.Hidden);
			}
			else
			{
			//	Handheld.PlayFullScreenMovie("squad1.mp4", Color.black, FullScreenMovieControlMode.CancelOnInput);
			}
		}
	}
}

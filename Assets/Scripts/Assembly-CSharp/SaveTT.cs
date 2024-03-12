using System.IO;
using UnityEngine;

public class SaveTT : MonoBehaviour
{
	public RenderTexture _rt;

	private bool _bSave;

	private int index;

	private void Update()
	{
		if (null != _rt && Input.GetKeyUp(KeyCode.F10))
		{
			_bSave = true;
		}
	}

	private void OnPostRender()
	{
		if (!_bSave)
		{
			return;
		}
		Texture2D texture2D = new Texture2D(_rt.width, _rt.height, TextureFormat.ARGB32, false);
		texture2D.ReadPixels(new Rect(0f, 0f, _rt.width, _rt.height), 0, 0);
		byte[] buffer = texture2D.EncodeToPNG();
		using (FileStream output = File.Create("d:\\xxx" + index + ".png"))
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(output))
			{
				binaryWriter.Write(buffer);
			}
		}
		index++;
		_bSave = false;
	}
}

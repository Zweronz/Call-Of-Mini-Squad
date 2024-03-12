using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[ExecuteInEditMode]
public class GameBuildingCell : MonoBehaviour
{
	public Material material;

	public Vector2 textureSize;

	public Rect textureFrame;

	public int cellCountX;

	public int cellCountY;

	public void Start()
	{
		MeshRenderer component = base.gameObject.GetComponent<MeshRenderer>();
		component.sharedMaterial = material;
		component.castShadows = false;
		component.receiveShadows = false;
		MeshFilter component2 = base.gameObject.GetComponent<MeshFilter>();
		component2.sharedMesh = new Mesh();
		Vector2[,] array = new Vector2[2, 2];
		array[0, 0] = new Vector2(textureFrame.xMin / textureSize.x, 1f - textureFrame.yMax / textureSize.y);
		array[0, 1] = new Vector2(textureFrame.xMax / textureSize.x, 1f - textureFrame.yMax / textureSize.y);
		array[1, 1] = new Vector2(textureFrame.xMax / textureSize.x, 1f - textureFrame.yMin / textureSize.y);
		array[1, 0] = new Vector2(textureFrame.xMin / textureSize.x, 1f - textureFrame.yMin / textureSize.y);
		int num = (cellCountX + 1) * (cellCountY + 1);
		Vector3[] array2 = new Vector3[num];
		Vector2[] array3 = new Vector2[num];
		Color[] array4 = new Color[num];
		int num2 = 0;
		for (int i = 0; i <= cellCountY; i++)
		{
			for (int j = 0; j <= cellCountX; j++)
			{
				array2[num2] = new Vector3((float)j * 1f, 0f, (float)i * 1f);
				array3[num2] = array[i % 2, j % 2];
				array4[num2] = Color.white;
				num2++;
			}
		}
		component2.sharedMesh.vertices = array2;
		component2.sharedMesh.uv = array3;
		component2.sharedMesh.colors = array4;
		int num3 = cellCountX * cellCountY * 6;
		int[] array5 = new int[num3];
		num2 = 0;
		for (int k = 0; k < cellCountY; k++)
		{
			for (int l = 0; l < cellCountX; l++)
			{
				int num4 = (k + 1) * (cellCountX + 1) + l;
				int num5 = (k + 1) * (cellCountX + 1) + l + 1;
				int num6 = k * (cellCountX + 1) + l + 1;
				int num7 = k * (cellCountX + 1) + l;
				array5[num2] = num4;
				array5[num2 + 1] = num5;
				array5[num2 + 2] = num6;
				array5[num2 + 3] = num4;
				array5[num2 + 4] = num6;
				array5[num2 + 5] = num7;
				num2 += 6;
			}
		}
		component2.sharedMesh.triangles = array5;
	}
}

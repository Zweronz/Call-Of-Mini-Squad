using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEditor.SceneManagement;
using UnityEditor.AI;

public class AutomaticBaker : Editor
{
	[MenuItem("Nav Baker/Bake")]
	public static void StartBaking()
	{
		SceneAsset temp = null;

		assetsInQueue = (from asset in AssetDatabase.FindAssets("MAP t:SceneAsset") where (temp = AssetDatabase.LoadAssetAtPath<SceneAsset>(AssetDatabase.GUIDToAssetPath(asset))).name != "NEW MAP" && temp.name != "PVPMAP" select AssetDatabase.GUIDToAssetPath(asset)).ToList();

		NextScene();

		EditorApplication.update += Update;
	}

	private static void NextScene()
	{
		EditorSceneManager.sceneOpened += FinishedLoading;
		EditorSceneManager.OpenScene(assetsInQueue[0]);
	}

	private static void FinishedLoading(Scene scene, OpenSceneMode mode)
	{
		EditorSceneManager.sceneOpened -= FinishedLoading;

		GameObject roomDoor = GameObject.Find("ROOM--DOOR");

		roomDoor.isStatic = true;

		foreach (Transform child in roomDoor.GetComponentsInChildren<Transform>())
		{
			child.gameObject.isStatic = true;
		}

		NavMeshBuilder.BuildNavMeshAsync();
		building = true;
	}

	private static void Update()
	{
		if (assetsInQueue.Count == 0)
		{
			EditorApplication.update -= Update;
			return;
		}

		if (building && !NavMeshBuilder.isRunning)
		{
			EditorSceneManager.SaveOpenScenes();
			assetsInQueue.RemoveAt(0);

			building = false;

			NextScene();
		}
	}

	private static List<string> assetsInQueue;

	private static bool building;
}

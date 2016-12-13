using UnityEngine;
using UnityEditor;
using System.Collections;

public class ExampleScriptable : ScriptableObject
{
	[MenuItem("Example/Create Example Asset")]

	static void CreateExampleAsset()
	{
		var exampleAsset = CreateInstance<ExampleScriptable>();

		AssetDatabase.CreateAsset(exampleAsset, "Assets/Editor/ExampleAsset.asset");
		AssetDatabase.Refresh();
	}
}

using UnityEngine;
using UnityEditor;
using System.Collections;

public class EditorWindowTest : EditorWindow {

	[MenuItem("Window/Example")]
	static void Open()
	{
		GetWindow<EditorWindowTest>();
	}

	void OnGUI()
	{
		EditorGUILayout.LabelField("Example Label");
	}
}

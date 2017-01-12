using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(ListTest))]
public class ListEditor : Editor {

	ReorderableList m_list;

	void OnEnable()
	{
		m_list = new ReorderableList(serializedObject, serializedObject.FindProperty("weak_list"));
		m_list.drawHeaderCallback = (rect) =>
		{
			EditorGUI.LabelField(rect, "weak_list");
		};

		m_list.drawElementCallback = (rect, index, isActive, isFocused) =>
		{
			var type = m_list.serializedProperty.GetArrayElementAtIndex(index).FindPropertyRelative("type");
			var multiple = m_list.serializedProperty.GetArrayElementAtIndex(index).FindPropertyRelative("multiple");
			//motion.intValue = EditorGUI.IntField(rect, motion.intValue);
			rect.width = 100;
			WeakPointType output = (WeakPointType)EditorGUI.EnumPopup(rect, (WeakPointType)type.enumValueIndex);
			type.enumValueIndex = (int)output;

			rect.x += 100;
			multiple.floatValue = EditorGUI.FloatField(rect, multiple.floatValue);
		};
	}

	public override void OnInspectorGUI()
	{
		// とりあえず元のプロパティ表示はしておく
		DrawDefaultInspector();

		serializedObject.Update();

		// リスト・配列の変更可能なリストの表示
		m_list.DoLayoutList();

		serializedObject.ApplyModifiedProperties();
	}

}

[CustomEditor(typeof(WeakPointParam))]
public class WeakPointBar : Editor
{

	ReorderableList m_list;

	void OnEnable()
	{
		m_list = new ReorderableList(serializedObject, serializedObject.FindProperty("enum_list"));
		m_list.drawHeaderCallback = (rect) =>
		{
			EditorGUI.LabelField(rect, "enum_list");
		};

		m_list.drawElementCallback = (rect, index, isActive, isFocused) =>
		{
			var motion = m_list.serializedProperty.GetArrayElementAtIndex(index);
			//motion.intValue = EditorGUI.IntField(rect, motion.intValue);
			rect.width = 100;
			WeakPointType output = (WeakPointType)EditorGUI.EnumPopup(rect, (WeakPointType)motion.enumValueIndex);
			rect.x += 100;
			EditorGUI.IntField(rect, motion.intValue);
			motion.enumValueIndex = (int)output;
		};
	}

	public override void OnInspectorGUI()
	{
		// とりあえず元のプロパティ表示はしておく
		DrawDefaultInspector();

		serializedObject.Update();

		// リスト・配列の変更可能なリストの表示
		m_list.DoLayoutList();

		serializedObject.ApplyModifiedProperties();
	}

}


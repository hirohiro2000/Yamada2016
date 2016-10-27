using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomPropertyDrawer(typeof(MultiToggleAttribute))]
public class MultiToggleDrawer : PropertyDrawer
{
	//bool init = false;
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		//MultiToggleAttribute multiToggleAttribute = (MultiToggleAttribute)attribute;

		//if(init == false)
		//{
		//	property.InsertArrayElementAtIndex(multiToggleAttribute.num);
		//	property.serializedObject.
		//	init = true;
  //      }

		if(property.propertyType == SerializedPropertyType.Boolean)
		{
			EditorGUILayout.BeginHorizontal();
			{
				EditorGUI.LabelField(position, property.ToString());
				for (int i = 0; i < property.arraySize; i++)
				{
					EditorGUI.Toggle(position, property.GetArrayElementAtIndex(i).boolValue);
				}
			}
			EditorGUILayout.EndHorizontal();

		}

	}



}

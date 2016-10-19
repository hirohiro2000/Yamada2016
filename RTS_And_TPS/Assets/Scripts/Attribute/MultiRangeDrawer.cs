using UnityEngine;
using UnityEditor;
using System.Collections;


[CustomPropertyDrawer(typeof(MultiRangeAttribute))]
public class MultiRangeDrawer : PropertyDrawer {


	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		MultiRangeAttribute multiRangeAttribute = (MultiRangeAttribute)attribute;
        if(property.propertyType ==SerializedPropertyType.Float)
		{

		}

	}
}

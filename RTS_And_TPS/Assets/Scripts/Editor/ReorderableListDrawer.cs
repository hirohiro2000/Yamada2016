using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditorInternal;
using System;
using System.Text.RegularExpressions;
using System.Reflection;

[CustomPropertyDrawer(typeof(ReorderableListAttribute))]
public class ReorderableListDrawer : PropertyDrawer
{

 ReorderableList _list;

	bool inited = false;
	int elementLineNum = 1;
	public void OnEnable()
	{
		UpdateElementLines();
	}
	public override void OnGUI(Rect position, SerializedProperty serializedProperty, GUIContent label)
	{
		SerializedProperty listProperty = serializedProperty.FindPropertyRelative("list");
		ReorderableList list = GetList(listProperty);

		UpdateElementLines();
		float height = 0f;
		for (var i = 0; i < listProperty.arraySize; i++)
		{
			height = Mathf.Max(height, EditorGUI.GetPropertyHeight(listProperty.GetArrayElementAtIndex(i)));
		}
		list.elementHeight = height * elementLineNum;

		if (inited == false)
		{
			inited = true;
			list.drawHeaderCallback = (rect) =>
			{ 
				EditorGUI.LabelField(rect, serializedProperty.displayName);
			};
			//var element = serializedProperty.GetArrayElementAtIndex(0);
			//int i = element.CountInProperty();
			//var multiple = list.serializedProperty.GetArrayElementAtIndex(index).FindPropertyRelative("multiple");
			list.drawElementCallback = (rect, index, isActive, isFocused) =>
			{
				ReorderableListAttribute listAttribute = (ReorderableListAttribute)attribute;
				
				//幅が設定されていたら代入
				int widthNum = 1;
				if (listAttribute.widths != null)
				{
					widthNum = listAttribute.widths.Length;
				}


				float beginX = rect.x;
				rect.height = list.elementHeight / elementLineNum;
				SerializedProperty value = list.serializedProperty.GetArrayElementAtIndex(index);
				int beforeDepth = value.depth;
				int cnt = -1;

				value.NextVisible(true);
				do
				{
					//子プロパティを見終わっていたら表示終了
					if (beforeDepth > value.depth)
						break;
					//幅の設定
					cnt++;
					if (listAttribute.widths != null)
					{
						while (listAttribute.widths.Length > cnt)
						{
							//-1なら改行
							if (listAttribute.widths[cnt] == -1)
							{
								rect.x = beginX;
								rect.y += rect.height;
								cnt++;
								continue;
							}
							else
							{
								rect.width = listAttribute.widths[cnt];
								break;
							}
						}

					}
					if (value.propertyType == SerializedPropertyType.Integer)
					{
						value.intValue = EditorGUI.IntField(rect, value.intValue);
					}
					else if (value.propertyType == SerializedPropertyType.Float)
					{
						value.floatValue = EditorGUI.FloatField(rect, value.floatValue);
					}
					else if (value.propertyType == SerializedPropertyType.Enum)
					{
						value.enumValueIndex = EditorGUI.Popup(rect, value.enumValueIndex, value.enumDisplayNames);
					}
					else if (value.propertyType == SerializedPropertyType.String)
					{
						value.stringValue = EditorGUI.TextField(rect, value.stringValue);
					}
					else if (value.propertyType == SerializedPropertyType.Color)
					{
						value.colorValue = EditorGUI.ColorField(rect, value.colorValue);
					}
					else if (value.propertyType == SerializedPropertyType.Boolean)
					{
						value.boolValue = EditorGUI.Toggle(rect, value.boolValue);
					}
					else if (value.propertyType == SerializedPropertyType.AnimationCurve)
					{
						value.animationCurveValue = EditorGUI.CurveField(rect, value.animationCurveValue);
					}
					else if (value.propertyType == SerializedPropertyType.Rect)
					{
						value.rectValue = EditorGUI.RectField(rect, value.rectValue);
					}
					else if (value.propertyType == SerializedPropertyType.Vector2)
					{
						value.vector2Value = EditorGUI.Vector2Field(rect, "", value.vector2Value);
					}
					else if (value.propertyType == SerializedPropertyType.Vector3)
					{
						value.vector3Value = EditorGUI.Vector3Field(rect, "", value.vector3Value);
					}
					else if (value.propertyType == SerializedPropertyType.Vector4)
					{
						value.vector4Value = EditorGUI.Vector4Field(rect, "", value.vector4Value);
					}
					else if (value.propertyType == SerializedPropertyType.ObjectReference)
					{
						string type = value.type;
						Match match = Regex.Match(type, @"PPtr<\$(.*?)>");
						if (match.Success)
						{
							type = match.Groups[1].Value;
							if (typeof(UnityEngine.Object).Assembly.GetType("UnityEngine." + type) != null)//UnityEngineのAPIならば
							{
								value.objectReferenceValue = EditorGUI.ObjectField(rect, value.objectReferenceValue, typeof(UnityEngine.Object).Assembly.GetType("UnityEngine." + type), true);
							}
							else //スクリプトならば
							{
								foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
								{
									foreach (Type tyPe in assembly.GetTypes())
									{
										if (tyPe.Name == type)
										{
											value.objectReferenceValue = EditorGUI.ObjectField(rect, value.objectReferenceValue, tyPe, true);
											break;
										}
									}
								}
								
							}
						}
						else
						{
							Debug.LogWarning("ObjectReferenceの表示に失敗しました。 by中野");
						}
					}
					else
					{
						Debug.LogWarning("表示されないプロパティーが存在します 中野に要連絡");
					}
					rect.x += rect.width + 10;
					beforeDepth = value.depth;
				}
				while (value.NextVisible(false));
				if (widthNum <= cnt)
					Debug.LogWarning("構造体の要素数が表示幅を設定した数を超えています。\n 修正例:[ReorderableList(new int[]{100,200,50,....})] by中野");
				//var type = m_list.serializedProperty.GetArrayElementAtIndex(index).FindPropertyRelative("type");
				//var multiple = m_list.serializedProperty.GetArrayElementAtIndex(index).FindPropertyRelative("multiple");
				////motion.intValue = EditorGUI.IntField(rect, motion.intValue);
				//rect.width = 100;
				//WeakPointType output = (WeakPointType)EditorGUI.EnumPopup(rect, (WeakPointType)type.enumValueIndex);
				//type.enumValueIndex = (int)output;

				//rect.x += 100;
				//multiple.floatValue = EditorGUI.FloatField(rect, multiple.floatValue);
			};
		}

		list.DoList(position);

	}

	public override float GetPropertyHeight(SerializedProperty serializedProperty, GUIContent label)
	{
		SerializedProperty listProperty = serializedProperty.FindPropertyRelative("list");
		return GetList(listProperty).GetHeight();
	}

	private ReorderableList GetList(SerializedProperty serializedProperty)
	{

		if (_list == null)
		{
			_list = new ReorderableList(serializedProperty.serializedObject, serializedProperty);
		}
		return _list;

	}

	private void UpdateElementLines()
	{
		ReorderableListAttribute listAttribute = (ReorderableListAttribute)attribute;

		if (listAttribute.widths != null)
		{
			elementLineNum = 1;
			for (int i = 0; i < listAttribute.widths.Length; i++)
			{
				if (listAttribute.widths[i] == -1)
					elementLineNum++;
			}
		}
	}

};

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// GameObjectの拡張クラス
/// </summary>
public static class GameObjectExtension
{
  /// <summary>
  /// 親や子オブジェクトも含めた範囲から指定のコンポーネントを取得する
  /// </summary>
	public static T GetComponentInParentAndChildren<T>( GameObject gameObject ) where T : UnityEngine.Component
	{
		if(gameObject.GetComponentInParent<T>() != null){
			return gameObject.GetComponentInParent<T>();
		}
		if(gameObject.GetComponentInChildren<T>() != null){
			return gameObject.GetComponentInChildren<T>();
		}

		return gameObject.GetComponent<T>();
	}
  
  /// <summary>
  /// 親や子オブジェクトも含めた範囲から指定のコンポーネントを全て取得する
  /// </summary>
	public static List<T> GetComponentsInParentAndChildren<T>( GameObject gameObject ) where T : UnityEngine.Component
	{
		List<T> list = new List<T>();
		list.AddRange (gameObject.GetComponents<T>());
		list.AddRange (gameObject.GetComponentsInChildren<T>());
		list.AddRange (gameObject.GetComponentsInParent<T>());
		return list;
	}  
}
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class ReorderableList<type>
{
	public List<type> list;
	public type this[int index]
	{
		get
		{
			return list.ToArray()[index];
		}
		set
		{
			list.Insert(index, value);
			list.RemoveAt(index +1);
		}
	}

	public int Length
	{
		get
		{
			return list.Count;
		}
	}

}


/*
 * ReorderableListAttribute
 * 
 * 引数解説
 * 
 * widths = リスト内に表示されるプロパティの表示幅の配列
 * 例えば(new int[]{100,200,50})ならば
 * 1番目のプロパティ:幅100px
 * 2番目のプロパティ:幅200px
 * 3番目のプロパティ:幅50px
 * で表示される
 * 
 * 改行の設定
 * -1と入力すると、改行できる
 * 例えば(new int[]{100,-1,50})ならば
 * 1番目のプロパティ:幅100px
 * 改行
 * 2番目のプロパティ:幅50px
 * で表示される
 *  
*/
public class ReorderableListAttribute : PropertyAttribute {

	public readonly int[] widths;
	public ReorderableListAttribute(int[] widths,bool isVisiblePropertyName = false)
	{
		this.widths = widths;
	}

	public ReorderableListAttribute(bool isVisiblePropertyName = true)
	{
		this.widths = null;
	}
}

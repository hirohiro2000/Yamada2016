using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum WeakPointType//最大値 = 
{
	ATTACK_PLAYER_TYPE = 0,
	TPSAttack,
	RTSAttack,
	EnemyAttack,
	ATTACK_PLAYER_END = 50,

	ATTACK_ATTRIBUTE_TYPE,
	Fire,
	Ice,
	Explosion,
	Arrow,
	ATTACK_ATTRIBUTE_END = ATTACK_PLAYER_END + 100,

	SPECIAL_EFFECT_TYPE,
	Recovery,
	Slow,
	Stop,
	SPECIAL_EFFECT_END = ATTACK_ATTRIBUTE_END + 100,
};

[System.Serializable]
public class WeakPointParam
{
	public WeakPointType type;
	public float multiple = 1.0f;
}

[System.Serializable]
public class WeakPointParamReorderableList : ReorderableList<WeakPointParam> { }


public class WeakPointList : MonoBehaviour
{

	[ReorderableList(new int[] { 100, 100 })]
	public WeakPointParamReorderableList weak_lists;

	public WeakPointList(WeakPointList weak)
	{
		weak_lists = new WeakPointParamReorderableList();
		weak_lists.list = new List<WeakPointParam>(weak.weak_lists.list);
	}

	public delegate void WeakPointParamChange(ref WeakPointList weak, Vector3 attackedPostion);

	//ダメージ計算・衝突判定前に呼び出します(計算後に破棄されます)
	public WeakPointParamChange BeforeCalcDamegeCallBack = null;

	//衝突判定をした後に呼び出します(計算された値は継続しています)
	public WeakPointParamChange HitedCallBack = null;


}

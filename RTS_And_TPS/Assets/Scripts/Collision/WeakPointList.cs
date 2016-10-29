using UnityEngine;
using System.Collections;



public enum WeakPointType//最大値 = 
{
	ATTACK_PLAYER_TYPE = 0,
	TPSAttack,
	RTSAttack,
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
	public float multiple;
}

[System.Serializable]
public class WeakPointParamReorderableList : ReorderableList<WeakPointParam> { }


public class WeakPointList : MonoBehaviour
{

	[ReorderableList(new int[] { 100, 100 })]
	public WeakPointParamReorderableList weak_lists;

}

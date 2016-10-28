using UnityEngine;
using System.Collections;



public enum WeakPointType
{
	TPSAttack,
	RTSAttack,
	Fire,
	Ice,
	Explosion,
	Arrow,
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

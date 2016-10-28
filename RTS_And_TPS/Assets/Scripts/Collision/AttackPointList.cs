using UnityEngine;
using System.Collections;

public class AttackPointList : MonoBehaviour {

	public float baseAttackPoint;

	[SerializeField, ReorderableList(new int[] { 100, 100 })]
	WeakPointParamReorderableList attack_list;

	[SerializeField]
	Transform autoDestroyObject;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider collider)
	{
		collider.attachedRigidbody.GetComponent<DamageBank>().SendDamege(this,collider);
    }
}

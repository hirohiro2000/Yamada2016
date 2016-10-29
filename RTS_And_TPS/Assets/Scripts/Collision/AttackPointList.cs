using UnityEngine;
using System.Collections;

public class AttackPointList : MonoBehaviour {

	public float baseAttackPoint;

	[SerializeField, ReorderableList(new int[] { 100, 100 })]
	WeakPointParamReorderableList attack_list = null;

	[SerializeField]
	Transform autoDestroyObject = null;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider collider)
	{	
		for (int i = 0; i < attack_list.Length; i++)
		{
			float damage = baseAttackPoint * attack_list[i].multiple;
			Debug.Log("Damage:" + damage + "(" + attack_list[i].type.ToString() + ")");
		}
		collider.attachedRigidbody.GetComponent<DamageBank>().SendDamege(this,collider);
    }

	public void CallDestroy()
	{
		if(autoDestroyObject != null)
		{
			Destroy(autoDestroyObject.gameObject);
		}
	}
}

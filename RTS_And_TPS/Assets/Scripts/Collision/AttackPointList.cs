using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AttackPointList : MonoBehaviour {

	public float baseAttackPoint = 1.0f;

	[SerializeField, ReorderableList(new int[] { 100, 100 })]
	public WeakPointParamReorderableList attack_list = null;

	[SerializeField]
	Transform autoDestroyObject = null;

	public AttackPointList(AttackPointList atk)
	{
		baseAttackPoint = atk.baseAttackPoint;
		attack_list = new WeakPointParamReorderableList();
        attack_list.list = new List<WeakPointParam>(atk.attack_list.list);
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Collider collider = GetComponent<Collider>();
		if(collider.enabled == true)
		{
			collider.enabled = false;
			collider.enabled = true;
		}
	}

	void OnTriggerEnter(Collider collider)
	{	
		//for (int i = 0; i < attack_list.Length; i++)
		//{
		//	float damage = baseAttackPoint * attack_list[i].multiple;
		//	Debug.Log("Damage:" + damage + "(" + attack_list[i].type.ToString() + ")");
		//}
		DamageBank damageBank = collider.GetComponentInParent<DamageBank>();
		if (damageBank != null)
		{
			damageBank.RecieveDamage(this,collider);
		}
    }

	public delegate void AttackPointParamChange(ref AttackPointList atk, Vector3 damagedPostion);

	//ダメージ計算・衝突判定前に呼び出します(計算後に破棄されます)
	public AttackPointParamChange BeforeCalcDamegeCallBack = null;

	//衝突判定をした後に呼び出します(計算された値は継続しています)
	public AttackPointParamChange HitedCallBack = null;

	public delegate void DamageBankParamChange(ref DamageBank damaged, DamageResult result);

	//ダメージを与えた後に呼び出します(ダメージ量が0でも)
	public DamageBankParamChange GiveDamagedCallBack = null;



	public void CallDestroy()
	{
		if(autoDestroyObject != null)
		{
			Destroy(autoDestroyObject.gameObject);
		}
	}
}

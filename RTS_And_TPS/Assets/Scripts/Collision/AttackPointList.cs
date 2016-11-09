using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AttackPointList : MonoBehaviour {

	public float baseAttackPoint = 1.0f;


	[SerializeField, ReorderableList(new int[] { 100, 100 })]
	public WeakPointParamReorderableList attack_list = null;

	//衝突後に自動的に消去するオブジェクト
	[SerializeField]
	Transform autoDestroyObject = null;

	//衝突後に自動的に自身の原点に出現するオブジェクト
	[SerializeField]
	Transform autoEmitObjectOnMyPosition = null;

	//衝突後に自動的に衝突点に出現するオブジェクト
	[SerializeField]
	Transform autoEmitObjectOnHitPoint = null;

	//コライダーのリセットを要求
	bool ColliderResetRequest = false;


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
		ColliderResetRequest = true;
	}

	void FixedUpdate()
	{
		//要求を受け入れる
		if (ColliderResetRequest)
		{
			ColliderResetRequest = false;
			Collider collider = GetComponent<Collider>();
			if (collider.enabled == true)
			{
				collider.enabled = false;
				collider.enabled = true;
			}
		}
	}

	void OnTriggerEnter(Collider collider)
	{	
		DamageBank damageBank = collider.GetComponentInParent<DamageBank>();
		if (damageBank != null)
		{
			damageBank.RecieveDamage(this,collider);
		}
    }

	public delegate void AttackPointParamChange(ref AttackPointList atk,CollisionInfo info);

	//ダメージ計算・衝突判定前に呼び出します(計算後に破棄されます)
	public AttackPointParamChange BeforeCalcDamegeCallBack = null;

	//衝突判定をした後に呼び出します(計算された値は継続しています)
	public AttackPointParamChange HitedCallBack = null;


	public void CallDestroy()
	{
		if(autoDestroyObject != null)
		{
			Destroy(autoDestroyObject.gameObject);
		}
	}

	public void CallEmitObjectOnMyPosition()
	{
		if (autoEmitObjectOnMyPosition != null)
		{
			Instantiate(autoEmitObjectOnMyPosition.gameObject,transform.position, transform.rotation);
		}
	}


	public void CallEmitObjectOnHitPoint(Vector3 hitPoint)
	{
		if (autoEmitObjectOnHitPoint != null)
		{
			Instantiate(autoEmitObjectOnHitPoint.gameObject, hitPoint, transform.rotation);
		}
	}

}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DamageResult
{
	//float damage;
	//AttackPointList attackPointList;
}
[RequireComponent(typeof(Rigidbody))]
public class DamageBank : MonoBehaviour {

	//public List<DamageResult> damageList;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void SendDamege(AttackPointList atk ,Collider damagedCollider)
	{
		WeakPointList[] weak;
		Transform searchTransform =  damagedCollider.transform;

		//弱点スクリプトを検索
        while(true)
		{
			weak = searchTransform.GetComponents<WeakPointList>();
			if (weak != null)
				break;
			if (searchTransform.parent == null)
				break;

			searchTransform = searchTransform.parent;
		}

		if(weak != null)
		{
			Debug.Log("damaged:" + searchTransform.gameObject.name + "   attacked:" + atk.gameObject.name);
        }

    }
}


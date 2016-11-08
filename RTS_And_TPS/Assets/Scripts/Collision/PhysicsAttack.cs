using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PhysicsAttack : MonoBehaviour {

	public AttackPointList attackPointList;

	List<Transform> attackdObject = new List<Transform>();

	
	// Use this for initialization
	void Awake () {
		attackPointList.HitedCallBack += (ref AttackPointList atk, CollisionInfo info) =>
		 {
			 //リストに追加
			 attackdObject.Add(info.damagedObject);
         };

		attackPointList.BeforeCalcDamegeCallBack += (ref AttackPointList atk, CollisionInfo info) =>
		{
			foreach(Transform obj in attackdObject)
			{
				if(obj == info.damagedObject)
				{
					atk.baseAttackPoint = .0f;
					atk.attack_list.list.Clear();
                    return;
                }
            }
		};
	}
	
	public void ListClear()
	{
		attackdObject.Clear();
    }

	// Update is called once per frame
	void Update () {
	
	}
}

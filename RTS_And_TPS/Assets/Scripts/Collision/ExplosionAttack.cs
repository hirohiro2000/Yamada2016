using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AttackPointList))]
[RequireComponent(typeof(SphereCollider))]
public class ExplosionAttack : MonoBehaviour {
	[SerializeField]
	AnimationCurve hitPowerLengthRate;

	AttackPointList _attackPointList = null;

	AttackPointList attackPointList
	{
		get
		{
			if(_attackPointList == null)
			{
				_attackPointList = GetComponent<AttackPointList>();
            }
			return _attackPointList;
		}
	}

	SphereCollider _sphereCollider = null;

	SphereCollider sphereCollider
	{
		get
		{
			if (_sphereCollider == null)
			{
				_sphereCollider = GetComponent<SphereCollider>();
			}
			return _sphereCollider;
		}
	}



	// Use this for initialization
	void Awake () {

		attackPointList.BeforeCalcDamegeCallBack += (ref AttackPointList atk, CollisionInfo info) =>
		{
			//距離により修正
			//当たり判定の半径を取得
			float colliderGlobalsize = sphereCollider.bounds.size.x;

			//敵の距離により割合を出す
			float rate = (transform.position - info.damagedObject.position).magnitude / colliderGlobalsize;

			//割合を攻撃力に反映
			atk.baseAttackPoint *= hitPowerLengthRate.Evaluate(rate);
        };
    }
	
	// Update is called once per frame
	void Update () {
	
	}

	void FixedUpdate()
	{
		Destroy(this.gameObject);
	}
}

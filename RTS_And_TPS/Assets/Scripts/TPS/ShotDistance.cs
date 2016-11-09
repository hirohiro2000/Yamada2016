using UnityEngine;
using System.Collections;
[RequireComponent(typeof(AttackPointList))]
public class ShotDistance : MonoBehaviour {

	AttackPointList _attackPointList = null;

	AttackPointList attackPointList
	{
		get
		{
			if (_attackPointList == null)
			{
				_attackPointList = GetComponent<AttackPointList>();
			}
			return _attackPointList;
		}
	}


	[SerializeField]
	AnimationCurve PowerlossCurve;
	float cntDistance = .0f;
	Vector3 BeforePos = Vector3.zero;
	void FixedUpdate()
	{
		cntDistance += (BeforePos - transform.position).magnitude;
		BeforePos = transform.position;

		if (PowerlossCurve.Evaluate(cntDistance) < .0f)
			Destroy(this.gameObject);
	}
	// Use this for initialization
	void Awake ()
	{
		BeforePos = transform.position;
		attackPointList.BeforeCalcDamegeCallBack += (ref AttackPointList atk, CollisionInfo info) =>
		{
			//距離で減衰
			atk.baseAttackPoint *= PowerlossCurve.Evaluate(cntDistance);
		};

	}

	// Update is called once per frame
	void Update () {
	
	}
}

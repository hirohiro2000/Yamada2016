using UnityEngine;
using System.Collections;
[RequireComponent(typeof(AttackPointList))]
[RequireComponent(typeof(CapsuleCollider))]
public class StraightBullet : MonoBehaviour {

	CapsuleCollider myCollider = null;
	// Use this for initialization
	void Awake() {
		myCollider = GetComponent<CapsuleCollider>();
		GetComponent<AttackPointList>().CalcContactPointBeforeDamege += (Collider collider) =>
		{
			Ray ray = new Ray(transform.position - transform.forward * 5.0f, transform.forward);
			RaycastHit hit;
			if(collider.Raycast(ray, out hit, 10.0f))
			{
				return hit.point - transform.forward * 0.3f;
			}
			return collider.ClosestPointOnBounds(transform.position);
		};

	}
	// Update is called once per frame
	void Update () {
	
	}
}

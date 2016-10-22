using UnityEngine;
using System.Collections;

public class EnemyAtkTargetManager : MonoBehaviour {

	[SerializeField]
	Transform[] targets = null;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public Transform getNearestTarget(Vector3 point,float radius, bool Zenable = true)
	{
		float sqrRadius = radius * radius;
		float minSqrDistance = float.MaxValue;
		Transform ret = null;

		for (int i = 0; i < targets.Length; i++)
		{
			Vector3 len = targets[i].position - point;
			if (Zenable)
				len.z = .0f;

			float sqrDistance = len.sqrMagnitude;
			if (minSqrDistance > sqrDistance)
			{
				minSqrDistance = sqrDistance;
				ret = targets[i];
			}
		}

		if (minSqrDistance > sqrRadius)
			return null;


		return ret;
	}
}

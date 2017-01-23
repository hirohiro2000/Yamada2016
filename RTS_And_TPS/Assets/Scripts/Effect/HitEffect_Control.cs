using UnityEngine;
using System.Collections;

public class HitEffect_Control : MonoBehaviour {
	public GameObject[] c_Emission = null;

	void OnCollisionEnter(Collision _rCollision)
	{
		Emit();
	}

	void OnTriggerEnter(Collider _rCollision)
	{
		Emit();
	}

	void Emit()
	{
		if (c_Emission == null) return;
		if (c_Emission.Length == 0) return;

		for (int i = 0; i < c_Emission.Length; i++)
		{
			GameObject rEmission = c_Emission[i];
			if (!rEmission) continue;

			Instantiate(rEmission, transform.position, transform.rotation);

		}
	}
}

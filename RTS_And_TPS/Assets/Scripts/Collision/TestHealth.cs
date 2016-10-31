using UnityEngine;
using System.Collections;

[RequireComponent(typeof(DamageBank))]
public class TestHealth : MonoBehaviour {

	DamageBank damageBank = null;
	// Use this for initialization
	void Start () {
		damageBank = GetComponent<DamageBank>();
		damageBank.DamagedCallback += (damageResult) =>
		{
			Destroy(this.gameObject);
		};
	}
	
	// Update is called once per frame
	void Update () {

	}
}

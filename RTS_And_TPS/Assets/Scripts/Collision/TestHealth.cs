using UnityEngine;
using System.Collections;

[RequireComponent(typeof(DamageBank))]
public class TestHealth : MonoBehaviour {

	DamageBank damageBank = null;
	public float health = 3.0f;
	// Use this for initialization
	void Start () {
		damageBank = GetComponent<DamageBank>();
		damageBank.DamagedCallback += (damageResult) =>
		{
			//ダメージ
			health -= damageResult.GetTotalDamage();

			//0なら死ね
			if(health <= .0f)
			{
				Destroy(gameObject);
			}

		};
	}
	
	// Update is called once per frame
	void Update () {
    }
}

using UnityEngine;
using System.Collections;

[RequireComponent(typeof(DamageBank))]
public class SparkHitEffect : MonoBehaviour {

	public ParticleSystem particle;

	DamageBank damageBank;


	// Use this for initialization
	void Awake () {
		damageBank = GetComponent<DamageBank>();

		damageBank.AdvancedDamagedCallback += (result, info) =>
		  {
			  float damage = result.GetTotalDamage();
			  ParticleSystem copy = (Instantiate(particle.gameObject, info.contactPoint,info.attackedObject.rotation) as GameObject).GetComponent<ParticleSystem>();
			  copy.startSpeed *=  1.0f + (damage / 20.0f);
			  copy.Emit((int)(1 + damage * 3.0f));

		  };
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}

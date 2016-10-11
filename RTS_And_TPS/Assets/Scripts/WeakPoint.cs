using UnityEngine;
using System.Collections;

public class WeakPoint : MonoBehaviour
{

	TPS_Enemy enemy;
	[SerializeField]
	float damageMultiple;
	// Use this for initialization
	void Start()
	{
		enemy = GetComponent<TPS_Enemy>();
		if(enemy == null)
		{
			enemy = GetComponentInParent<TPS_Enemy>();
		}
    }

	// Update is called once per frame
	void Update()
	{

	}

	public void OnCollisionEnter(Collision collision)
	{
		Debug.Log(gameObject.name);
		DamageSource source = collision.gameObject.GetComponentInParent<DamageSource>();
		if(source != null)
		{
			enemy.GiveDamage(damageMultiple * source.damage);

		}
    }
}

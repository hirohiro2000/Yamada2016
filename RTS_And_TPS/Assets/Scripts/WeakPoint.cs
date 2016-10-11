using UnityEngine;
using System.Collections;

public class WeakPoint : MonoBehaviour
{

	Enemy enemy;
	[SerializeField]
	float damageMultiple;
	// Use this for initialization
	void Start()
	{
		enemy = GetComponent<Enemy>();
		if(enemy == null)
		{
			enemy = GetComponentInParent<Enemy>();
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

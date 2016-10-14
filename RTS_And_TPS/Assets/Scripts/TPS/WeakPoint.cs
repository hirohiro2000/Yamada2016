using UnityEngine;
using System.Collections;

public class WeakPoint : MonoBehaviour
{
	public float damageMultiple;
	// Use this for initialization
	void Start()
	{
    }

	// Update is called once per frame
	void Update()
	{

	}

	//void OnCollisionEnter(Collision collision)
	//{
	//	Debug.Log(gameObject.name);
	//	//一番倍率の高い衝突を探す
	//	for( collision

	//	DamageSource source = collision.gameObject.GetComponentInParent<DamageSource>();
	//	if(source != null)
	//	{
	//		enemy.GiveDamage(damageMultiple * source.damage);

	//	}
 //   }
}

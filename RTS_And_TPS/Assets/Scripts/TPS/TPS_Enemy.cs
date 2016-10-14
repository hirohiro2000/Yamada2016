using UnityEngine;
using System.Collections;

public class TPS_Enemy : MonoBehaviour {
	[SerializeField]
	float hp;

	[SerializeField]
	HealthBar3D healthBar3D;

	[SerializeField]
	Transform hpBar;
	// Use this for initialization
	void Start () {
		if (healthBar3D != null)
			healthBar3D.setValue(1.0f);
    }
	
	// Update is called once per frame
	void Update () {
	
	}

	public void GiveDamage(float damage)
	{
		if (hp <= .0f)
			return;

		hp -= damage;
		if (healthBar3D != null)
			healthBar3D.setValue(hp / 20.0f);
		if (hp <= .0f)
		{
			if (hpBar != null)
			{
				hpBar.SetParent(null);
				Destroy(hpBar.gameObject, 1.0f);
			}

            EnemyKillCounter.killCount++;
			Destroy(this.gameObject);
		}
    }

	void OnCollisionEnter(Collision collision)
	{

		DamageSource source = collision.gameObject.GetComponentInParent<DamageSource>();
		if (source != null)
		{
			//一番倍率の高い衝突を探す
			float maxDamageMultiple = .0f;

			foreach ( ContactPoint colider in collision.contacts)
			{
				WeakPoint weakPoint = colider.thisCollider.GetComponent<WeakPoint>();

				if(weakPoint != null)
				{
					Debug.Log(weakPoint.gameObject.name + ":" + weakPoint.damageMultiple);

					if(weakPoint.damageMultiple > maxDamageMultiple)
					{
						maxDamageMultiple = weakPoint.damageMultiple;
                    }
				}
			}


			GiveDamage(maxDamageMultiple * source.damage);

		}
	
	}
}

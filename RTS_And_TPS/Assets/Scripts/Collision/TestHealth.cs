using UnityEngine;
using System.Collections;

[RequireComponent(typeof(DamageBank))]
public class TestHealth : MonoBehaviour {

	DamageBank damageBank = null;
	public float health = 3.0f;

	// Use this for initialization
	void Start () {
		damageBank = GetComponent<DamageBank>();
		damageBank.DamagedCallback += (damage) =>
		{
			//ダメージ
			health -= damage;
            UserLog.Nakano("Damaged: " + damage.ToString());

			//0なら死ね
			if(health <= .0f)
			{
				Destroy(gameObject);
			}

		};


		//damageBank.AdvancedDamagedCallback += (DamageResult, info) =>
		//{
		//	//ダメージ部分に丸を出す
		//	(Instantiate(GameObject.CreatePrimitive(PrimitiveType.Sphere), info.contactPoint, Quaternion
		//	.identity) as GameObject).transform.localScale *= 0.2f;

		//};

	}
	
	// Update is called once per frame
	void Update () {
    }
}

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
            Debug.Log("Damaged: " + damage.ToString());

			//0なら死ね
			if(health <= .0f)
			{
				Destroy(gameObject);
			}

		};

		//damageBank.OnceDamagedCollierCallback += (damagedCollider) =>
		//{

  //      };

		//	damageBank.AdvancedDamagedCallback += (DamageResult, contactPoint) =>
		//{
		//	//ダメージ部分に丸を出す
		//	(Instantiate(GameObject.CreatePrimitive(PrimitiveType.Sphere), contactPoint, Quaternion
		//		.identity) as GameObject).transform.localScale *=0.2f;

		//};
	}
	
	// Update is called once per frame
	void Update () {
    }
}

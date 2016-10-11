using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {
	[SerializeField]
	float hp;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void GiveDamage(float damage)
	{
		hp -= damage;
		if(hp <= .0f)
		{
			Destroy(this.gameObject);
		}
    }
}

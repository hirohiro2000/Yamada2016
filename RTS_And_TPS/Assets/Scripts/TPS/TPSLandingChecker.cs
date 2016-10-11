using UnityEngine;
using System.Collections;

public class TPSLandingChecker : MonoBehaviour
{
	public bool isLanding = false;
	//現在はオブジェクト名がFloorならば床判定

	// Use this for initialization
	void Start()
	{
		//無ければ自動生成
		if (GetComponent<Rigidbody>() == null)
		{
			Rigidbody rigidBody = gameObject.AddComponent<Rigidbody>();
			rigidBody.isKinematic = true;
		}
		Debug.LogWarning("現在、床判定はオブジェクト名がFloorかで判定しています");
	}

	// Update is called once per frame
	void Update()
	{
		isLanding = false;
	}

	public void OnTriggerStay(Collider other)
	{
		if(other.gameObject.name == "Floor")
		{
			isLanding = true;
        }

	}
}

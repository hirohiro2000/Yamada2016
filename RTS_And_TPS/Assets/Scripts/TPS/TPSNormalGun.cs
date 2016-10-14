using UnityEngine;
using System.Collections;

public class TPSNormalGun : MonoBehaviour
{
	[SerializeField]
	GameObject particle;

	float cntTime;
	float destroyTime;

	public void Shot_Start(float time)
	{
		destroyTime = time;
	}

	// Use this for initialization
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		cntTime += Time.deltaTime;
		if (cntTime > destroyTime)
		{
			if (destroyTime != .0f)
				Destroy(this.gameObject);
		}
	}

	public void OnCollisionEnter(Collision collision)
	{
		if(particle != null)
			Instantiate(particle, transform.position, transform.rotation * Quaternion.AngleAxis(180, Vector3.right));
		Destroy(this.gameObject);
	}
}

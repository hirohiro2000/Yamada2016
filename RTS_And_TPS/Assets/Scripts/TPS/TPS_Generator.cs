
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class TPS_Generator : MonoBehaviour {

	[SerializeField, Range(.0f, 10.0f)]
	private float m_resporn_interval_second = 2.0f;

	[SerializeField]
	private GameObject m_generate_object = null;

	// Use this for initialization
	void Start()
	{
		StartCoroutine(Resporn());
	}

	IEnumerator Resporn()
	{
		while (true)
		{
			//子供のPosと向き配置します
			var new_object = Instantiate(m_generate_object);
			var resporn_object = transform.GetChild(Random.Range(0, transform.childCount));
			var agent = new_object.GetComponent<NavMeshAgent>();
			if (agent != null)
				agent.Warp(resporn_object.position);
			else
				new_object.transform.position = resporn_object.position;
			new_object.transform.rotation = resporn_object.rotation;

			//cloneをまとめる
			string parentName = new_object.name + "s";
			GameObject parent = GameObject.Find(parentName);
			if (parent == null)
			{
				parent = new GameObject(parentName);
			}
			new_object.transform.parent = parent.transform;

			//  ネットワーク上で生成
			NetworkServer.Spawn( new_object );

			yield return new WaitForSeconds(m_resporn_interval_second);

		}

	}
}

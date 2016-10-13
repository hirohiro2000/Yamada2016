using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class EnemyGenerator : MonoBehaviour {

    private List<RespornPoint> m_sporn_point_list;

    [SerializeField, Range(.0f, 10.0f)]
    private float m_resporn_interval_second = 2.0f;

    [SerializeField]
    private GameObject m_generate_object;

	// Use this for initialization
	void Start ()
    {
        m_sporn_point_list = new List<RespornPoint>();
        var sporn_point = GameObject.Find("SpornPoint");
        for (int i = 0; i < sporn_point.transform.childCount; i++)
        {
            m_sporn_point_list.Add(sporn_point.transform.GetChild(i).GetComponent<RespornPoint>());
        }
        StartCoroutine(Resporn());
	}
	
    IEnumerator Resporn()
    {
        while (true)
        {
            
            var new_object =  Instantiate(m_generate_object);
            var resporn_object = m_sporn_point_list[UnityEngine.Random.Range(0, transform.childCount)];
            new_object.transform.position = resporn_object.GetRespornPos();
            yield return null;
            new_object.transform.parent = transform;
            yield return new WaitForSeconds(m_resporn_interval_second);

        }     

    }

	// Update is called once per frame
	void Update () {
	
	}
}

using UnityEngine;
using System.Collections;

public class RespornPoint : MonoBehaviour {

    private Vector3 m_resporn_adjust_scale; //とりあえず明示的に作った。最後は消す？

    void Awake()
    {
        m_resporn_adjust_scale = transform.localScale;
    }

	// Use this for initialization
	void Start () {
        GetComponent<MeshRenderer>().enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public Vector3 GetRespornPos()
    {
        return new Vector3(
            transform.position.x + UnityEngine.Random.Range(.0f, m_resporn_adjust_scale.x / 2.0f),
             transform.position.y,
              transform.position.z + UnityEngine.Random.Range(.0f, m_resporn_adjust_scale.z / 2.0f)
            );
    }
}

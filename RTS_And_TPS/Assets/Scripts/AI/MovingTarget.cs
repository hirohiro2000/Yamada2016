using UnityEngine;
using System.Collections;

public class MovingTarget : MonoBehaviour {

    private NavMeshAgent m_navmesh_accessor;

    private GameObject m_target_object;

    [SerializeField]
    private float m_default_steering_radius;

    [SerializeField, Range(.0f, 10.0f)]
    private float m_adjust_max_steering_radius;    

    void Awake()
    {
        m_navmesh_accessor = GetComponent<NavMeshAgent>();
        m_navmesh_accessor.radius = m_default_steering_radius + UnityEngine.Random.Range(.0f, m_adjust_max_steering_radius);
    }

	void Start () {
        //test
        m_target_object= GameObject.Find("Rocket");
        m_navmesh_accessor.SetDestination(m_target_object.transform.position);
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}
}

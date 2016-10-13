using UnityEngine;
using System.Collections;

/**
 *@brief そのうちState関連はMonoBehaviorから外す可能性あり
 */
public class MovingTarget : MonoBehaviour
{
    private NavMeshAgent m_navmesh_accessor;

    private GameObject m_target_object;
    private float m_default_steering_radius = 0.8f;

    [SerializeField, Range(.0f, 10.0f)]
    private float m_adjust_max_steering_radius = 2.0f;

    //public MovingTarget(NavMeshAgent navmesh_data)
    //{
    //    m_navmesh_accessor = navmesh_data;
    //    m_target_object = null;
    //    //test
    //    m_target_object = GameObject.Find("MechanicalGirl");
    //}

    void Awake()
    {
        m_navmesh_accessor = GetComponent<NavMeshAgent>();
       // m_navmesh_accessor.radius = m_default_steering_radius + UnityEngine.Random.Range(.0f, m_adjust_max_steering_radius);
    }

    void Start () {
        //test    
	}
	
    public void UpdateExpress()
    {
        m_navmesh_accessor.SetDestination(m_target_object.transform.position);
    }

    /**
    *@note stateは明示的に更新を行う
    */
    void Update ()
    {
        
    }

    public void SetNavmesh(NavMeshAgent agent)
    {
        m_navmesh_accessor = agent;
    }

    public void SetTarget(GameObject target)
    {
        m_target_object = target;
    }
}

using UnityEngine;
using System.Collections;

/**
 *@brief そのうちState関連はMonoBehaviorから外す可能性あり
 */
public class MovingTarget : TaskBase
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

    public override void Enter(TargetingSystem target_system, EnemyTaskDirector task_director)
    {
        base.Enter(target_system, task_director);
    }

    public override void Execute(TargetingSystem target_system, EnemyTaskDirector task_director)
    {
     //   if (m_navmesh_accessor.pathStatus != NavMeshPathStatus.PathInvalid)
            m_navmesh_accessor.SetDestination(m_target_object.transform.position);
        //m_navmesh_accessor.destination = m_target_object.transform.position;
    }

    public override void Exit(TargetingSystem target_system, EnemyTaskDirector task_director)
    {
        base.Exit(target_system, task_director);
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

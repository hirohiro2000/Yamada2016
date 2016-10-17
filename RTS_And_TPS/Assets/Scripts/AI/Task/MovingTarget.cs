using UnityEngine;
using System.Collections;

/**
 *@note そのうちState関連はMonoBehaviorから外す可能性あり
 */
public class MovingTarget : TaskBase
{
    private NavMeshAgent m_navmesh_accessor;

    //private GameObject m_target_object;
    private float m_default_steering_radius = 0.8f;

    [SerializeField, Range(.0f, 10.0f)]
    private float m_adjust_max_steering_radius = 2.0f;

    private float m_path_update_interval = 0.35f;
    private bool m_coroutine_flg = false;
    Vector3 m_target_point;

    TargetingSystem m_target_director;


    LineRenderer m_path_renderer;

 //   GameObject target_point_object;

    private float m_last_path_update_time;

    void Awake()
    {
    }

    void Start()
    {

    }

    /**
    *@breif このオブジェクトはコルーチン使えないので
    */
    void UpdateGoalPoint()
    {
        float game_time = Time.realtimeSinceStartup;
        if (game_time - m_last_path_update_time < m_path_update_interval)
            return;

        m_last_path_update_time = game_time;
        if (m_target_director.IsInsightTarget())
        {
            m_target_point = m_target_director.m_current_target.transform.position;
        }
        else
        {
            Debug.Log("Target lost");
        }
        m_navmesh_accessor.SetDestination(m_target_point);
        m_path_renderer.SetVertexCount(m_navmesh_accessor.path.corners.Length);
        m_path_renderer.SetPositions(m_navmesh_accessor.path.corners);

      //  target_point_object.transform.localPosition = m_target_point;
    }

    public override void Initialize(GameObject owner)
    {
        base.Initialize(owner);
        m_navmesh_accessor = owner.GetComponent<NavMeshAgent>();
        m_default_steering_radius += UnityEngine.Random.Range(.0f, m_adjust_max_steering_radius);
        m_navmesh_accessor.radius = m_default_steering_radius;
        m_path_renderer = GetComponent<LineRenderer>();
    }

    public override void Enter(TargetingSystem target_system, EnemyTaskDirector task_director)
    {
        var renderer_switch = target_system.m_current_target;

        m_target_director = target_system;
        m_target_point = target_system.m_current_target.transform.position;
        m_coroutine_flg = true;
        m_last_path_update_time = Time.realtimeSinceStartup;
        m_navmesh_accessor.Resume();
    }

    public override TaskBase.Status Execute(TargetingSystem target_system, EnemyTaskDirector task_director)
    {
        var path_status = m_navmesh_accessor.pathStatus;
        Debug.Log("path is " + path_status);

        UpdateGoalPoint();
        if (IsReachingGoalPoint())
        {
            return ReachingGoalPoint();
        }
        return TaskBase.Status.Active;

    }

    public override void Exit(TargetingSystem target_system, EnemyTaskDirector task_director)
    {
        m_navmesh_accessor.Stop();
        m_navmesh_accessor.ResetPath();
    }

    public Status ReachingGoalPoint()
    {
        if (m_target_director.IsInsightTarget())
            return Status.Completed;

        return Status.Failed;
    }

    public bool IsReachingGoalPoint()
    {
        float dist = (m_target_point - m_owner_object.transform.position).magnitude;
        //0.005fは調整値
        if (dist <   2.0f)
        {
            return true;
        }
        return false;
    }

}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

/**
 *@note そのうちState関連はMonoBehaviorから外す可能性あり
 */
public class MovingTarget : TaskBase
{
    //private readonly float one_frame = 1.0f / 60.0f;
    private readonly float m_normal_cost = 1.0f;
    private readonly float m_detor_cost = 10.0f;
    //ここからゴール更新間隔に関するparameter
    private readonly float m_update_adjust_static_object_min = 0.9f;
    private readonly float m_update_adjust_static_object_max = 2.0f;
    private readonly float m_update_adjust_move_object_min = 0.02f;
    private readonly float m_update_adjust_move_object_max = 0.6f;
    private readonly float m_default_update_interval = 0.1f;

    private NavMeshAgent m_navmesh_accessor;

    //private GameObject m_target_object;
    private float m_default_steering_radius = 0.8f;

    [SerializeField, Range(.0f, 10.0f)]
    private float m_adjust_max_steering_radius = 2.0f;

    private float m_path_update_interval;
    //private bool m_coroutine_flg = false;
    Vector3 m_target_point;    

    TargetingSystem m_target_director;


    LineRenderer m_path_renderer;

    [SerializeField, HeaderAttribute("このキャラクターが通ることのできるルート一覧(対防衛拠点)")]
    private string[] m_can_passing_route = null;

    private Dictionary<string, float> m_navmesh_cost_dictionary;

 //   GameObject target_point_object;

    private float m_last_path_update_time;

    delegate void CostFunction();

    private CostFunction m_cost_function;

    void Awake()
    {
        m_navmesh_cost_dictionary = new Dictionary<string, float>();
        NavMesh.pathfindingIterationsPerFrame = 500;
    }

    void InitializeCostArray()
    {
        var component = transform.root.GetComponent<CostNameContainer>();
        var layer_name_array = component.GetLayerNameArray();

        //通過できる場所だけNormalCostを挿入
       foreach(var name in layer_name_array)
        {
            if(Array.IndexOf(m_can_passing_route,name) != -1)
            {
                m_navmesh_cost_dictionary.Add(name, m_normal_cost);
            }
            else
            {
                m_navmesh_cost_dictionary.Add(name, m_detor_cost);
            }
        }


    }

    void Start()
    {
        InitializeCostArray();
        m_path_update_interval += UnityEngine.Random.Range(.0f, 0.23f);
    }

    /**
    *@breif そのうちコルーチンにもどす
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
        foreach(var it in m_navmesh_cost_dictionary)
        {
            NavMesh.SetAreaCost(NavMesh.GetAreaFromName(it.Key), it.Value);
        }
        //コスト設定を行った後、ゴールを更新する（経路探索が走る）
        m_cost_function();
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

    private void AllNormalCost()
    {
        foreach (var it in m_navmesh_cost_dictionary)
        {
            NavMesh.SetAreaCost(NavMesh.GetAreaFromName(it.Key), m_normal_cost);
        }
    }

    private void RouteCost()
    {
        foreach (var it in m_navmesh_cost_dictionary)
        {
            NavMesh.SetAreaCost(NavMesh.GetAreaFromName(it.Key), it.Value);
        }
    }

    private void CalculateUpdateInterval(TargetingSystem target_system)
    {
        if(target_system.m_is_static)
        {
            m_path_update_interval = m_default_update_interval + UnityEngine.Random.Range(m_update_adjust_static_object_min,m_update_adjust_static_object_max);
        }
        else
        {
            m_path_update_interval = m_default_update_interval + UnityEngine.Random.Range(m_update_adjust_move_object_min, m_update_adjust_move_object_max);
        }
    }

    public override void Enter(TargetingSystem target_system, EnemyTaskDirector task_director)
    {
        var renderer_switch = target_system.m_current_target;

        m_target_director = target_system;
        m_target_point = target_system.m_current_target.transform.position;
        //m_coroutine_flg = true;
        m_last_path_update_time = Time.realtimeSinceStartup;
        m_navmesh_accessor.Resume();
        task_director.m_anime_controller.SetTrigger("ToMoveTarget");
        if (target_system.m_target_tag == PerceiveTag.HomeBase)
        {
            m_cost_function = RouteCost;
        }
        else
        {
            m_cost_function = AllNormalCost;
        }
        CalculateUpdateInterval(target_system);
        m_cost_function();
        m_navmesh_accessor.SetDestination(m_target_point);
        
    }

    public override Status Execute(
        TargetingSystem target_system,
        EnemyTaskDirector task_director)
    {
        //var path_status = m_navmesh_accessor.pathStatus;
        //Debug.Log("path is " + path_status);

        task_director.m_anime_controller.SetFloat("MoveSpeed", m_navmesh_accessor.velocity.sqrMagnitude);
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

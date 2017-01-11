﻿using UnityEngine;
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
 //   private readonly float m_normal_cost = 1.0f;
   // private readonly float m_detor_cost = 10.0f;
    //ここからゴール更新間隔に関するparameter
    private readonly float m_update_adjust_static_object_min = 0.9f;
    private readonly float m_update_adjust_static_object_max = 2.0f;
    private readonly float m_update_adjust_move_object_min = 0.02f;
    private readonly float m_update_adjust_move_object_max = 0.3f;
    private readonly float m_default_update_interval = 0.1f;

    private NavMeshAgent m_navmesh_accessor = null;

    //private GameObject m_target_object;
    private float m_default_steering_radius = 0.8f;

    [SerializeField, Range(.0f, 10.0f)]
    private float m_adjust_max_steering_radius = 2.0f;

    private float m_path_update_interval;
    //private bool m_coroutine_flg = false;
    Vector3 m_target_point;    

    TargetingSystem m_target_director;

    LineRenderer m_path_renderer;

    //[SerializeField, HeaderAttribute("このキャラクターが通ることのできるルート一覧(対防衛拠点)")]
    private StringList m_can_passing_route = new StringList();

    private Dictionary<string, float> m_navmesh_cost_dictionary;

    private float m_last_path_update_time;

    delegate void CostFunction();
    private CostFunction m_cost_function;

    private float m_original_move_speed = .0f; //このタスクが実行される前のMoveSpeed
    private float m_move_speed = .0f;   //このタスクに対するMoveSpeed（現在はどのタスクも移動速度は同じになるが一応）

    void Awake()
    {
        m_navmesh_cost_dictionary = new Dictionary<string, float>();
        NavMesh.pathfindingIterationsPerFrame = 500;
    }

    void InitializeCostArray()
    {
       // var enemy_root = GameObject.Find("EnemySpawnRoot");
       // var component = enemy_root.GetComponent<CostNameContainer>();
       // var layer_name_array = component.GetLayerNameArray();

       // //通過できる場所だけNormalCostを挿入
       //foreach(var name in layer_name_array.data)
       // {
       //     if(m_can_passing_route.data.Contains(name))
       //     {
       //         m_navmesh_cost_dictionary.Add(name, m_normal_cost);
       //     }
       //     else
       //     {
       //         m_navmesh_cost_dictionary.Add(name, m_detor_cost);
       //     }
       // }//name
    }

    void Start()
    {
        InitializeCostArray();
        m_path_update_interval += UnityEngine.Random.Range(.0f, 0.23f);
    }

    public void SetPassingRoute(StringList route_list)
    {
        foreach(var name in route_list.data)
        {
            m_can_passing_route.data.Add(name);
        }
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
            UserLog.Terauchi("Target lost");
        }
        m_navmesh_accessor.SetDestination(m_target_point);
    }

    public override void SetWaveParametor(EnemyWaveParametor wave_param, 
        EnemyPersonalParametor personal_param)
    {

        int rate = wave_param.m_current_level - personal_param.m_emearge_level;
        m_move_speed = m_original_move_speed + (personal_param.GetMoveSpeedUpMultipleRate() * rate);
        m_move_speed = Mathf.Clamp(m_move_speed, .01f, personal_param.GetMaxmoveSpeed());
    }

    public override void Initialize(GameObject owner)
    {
        base.Initialize(owner);
        m_navmesh_accessor = owner.GetComponent<NavMeshAgent>();
        m_original_move_speed = m_navmesh_accessor.speed;
        m_move_speed = m_original_move_speed;
        m_default_steering_radius += UnityEngine.Random.Range(.0f, m_adjust_max_steering_radius);
        m_navmesh_accessor.radius = m_default_steering_radius;
        m_path_renderer = GetComponent<LineRenderer>();
		m_path_renderer.enabled = false;
    }

    private void AllNormalCost()
    {
        //foreach (var it in m_navmesh_cost_dictionary)
        //{
        //    NavMesh.SetAreaCost(NavMesh.GetAreaFromName(it.Key), m_normal_cost);
        //}
    }

    private void RouteCost()
    {
        //foreach (var it in m_navmesh_cost_dictionary)
        //{
        //    NavMesh.SetAreaCost(NavMesh.GetAreaFromName(it.Key), it.Value);
        //}
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
      //  m_cost_function();
        m_navmesh_accessor.SetDestination(m_target_point);
        //m_original_move_speed = m_navmesh_accessor.speed;
        m_navmesh_accessor.speed = m_move_speed;
     
    }

    public override Status Execute(
        TargetingSystem target_system,
        EnemyTaskDirector task_director)
    {
        task_director.m_anime_controller.SetFloat("MoveSpeed", m_navmesh_accessor.velocity.magnitude);
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
        m_navmesh_accessor.speed = m_original_move_speed;
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

using UnityEngine;
using System.Collections;

public class FlyingToTarget : TaskBase {

    [SerializeField, HeaderAttribute("最高高度")]
    private float m_max_height = 60.0f;
    [SerializeField, HeaderAttribute("最低高度")]
    private float m_min_height = 30.0f;
    [SerializeField, HeaderAttribute("PQSを使った動きをするかどうか")]
    private bool m_is_use_pqs = false;

    private PQSQuery m_pqs_info = null;
    private Vector3 m_move_point = Vector3.zero;

    void Awake()
    {
        m_pqs_info = GetComponent<PQSQuery>();
    }

    public override void Enter(TargetingSystem target_system, EnemyTaskDirector task_director)
    {
        base.Enter(target_system, task_director);
    }

    private void CalculateMovePoint(TargetingSystem target_system)
    {

    }

    public override Status Execute(TargetingSystem target_system, EnemyTaskDirector task_director)
    {
        return base.Execute(target_system, task_director);
    }

    public override void Exit(TargetingSystem target_system, EnemyTaskDirector task_director)
    {
        base.Exit(target_system, task_director);
    }

    public override void SetWaveParametor(EnemyWaveParametor wave_param, EnemyPersonalParametor parsonal_param)
    {
        base.SetWaveParametor(wave_param, parsonal_param);
    }

    public override void Initialize(GameObject owner)
    {
        base.Initialize(owner);
    }
}

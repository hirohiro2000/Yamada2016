using UnityEngine;
using System.Collections;

public class TEvalutionShootAndMove : TaskEvaluationBase
{

    private float m_shot_range = 5.0f;

    void Awake()
    {
        m_shot_range = GetComponent<ShootAndMove>().GetShotRange();
    }

    public override float Execute(
        TargetingSystem current_target_info, 
        EnemyTaskDirector director)
    {
        if (current_target_info.m_current_target == null)
            return .0f;

        float dist = (director.m_owner.transform.position - current_target_info.m_current_target.transform.position).magnitude;
        if (dist >= m_shot_range)
            return .0f;

        return m_base_score * 100.0f;

    }

}

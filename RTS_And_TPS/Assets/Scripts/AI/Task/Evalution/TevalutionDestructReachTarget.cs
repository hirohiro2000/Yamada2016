using UnityEngine;
using System.Collections;

public class TevalutionDestructReachTarget : TaskEvaluationBase
{

    public override float Execute(TargetingSystem current_target_info, EnemyTaskDirector director)
    {
        if (director.m_before_task_name == null ||
            director.m_before_task_status == TaskBase.Status.Error)
            return .0f;

        if (director.m_before_task_status == TaskBase.Status.Completed &&
            director.m_before_task_name == "MovingTarget")
            return 1000.0f;

        return .0f;
    }

}

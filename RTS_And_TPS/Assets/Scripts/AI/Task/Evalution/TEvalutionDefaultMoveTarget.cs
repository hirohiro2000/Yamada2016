using UnityEngine;
using System.Collections;

public class TEvalutionDefaultMoveTarget : TaskEvaluationBase
{

    public override float Execute(TargetingSystem current_target_info,
         EnemyTaskDirector director)
    {
        Vector3 me_to_target = (current_target_info.m_current_target.transform.position - director.m_owner.transform.position);
        float dist = me_to_target.magnitude;
        dist *= 0.01f;
        float score = m_base_score * dist;
        if (current_target_info.m_message_type != ViewMessageWrapper.MessageType.InVisibilityRange)
            score *= 0.01f;

        return score;
    }
}

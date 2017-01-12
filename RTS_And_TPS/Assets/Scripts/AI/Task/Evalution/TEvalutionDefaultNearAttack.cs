using UnityEngine;
using System.Collections;

public class TEvalutionDefaultNearAttack : TaskEvaluationBase {

    public override float Execute(TargetingSystem current_target_info, 
        EnemyTaskDirector director)
    {

        if (current_target_info.m_current_target == null)
            return .0f;

        //近距離にいない場合は適応されない
        if (current_target_info.m_message_type != ViewMessageWrapper.MessageType.InNearRange)
            return .0f;

        return m_base_score;
    }

}

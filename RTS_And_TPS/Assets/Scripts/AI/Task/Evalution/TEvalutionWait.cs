using UnityEngine;
using System.Collections;

public class TEvalutionWait : TaskEvaluationBase
{

    public override float Execute(TargetingSystem current_target_info, EnemyTaskDirector director)
    {
        return .00001f;
    }

}

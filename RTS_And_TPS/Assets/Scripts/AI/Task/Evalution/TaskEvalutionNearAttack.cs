using UnityEngine;
using System.Collections;

public class TaskEvalutionNearAttack : TaskEvaluationBase {

    public override float Execute(TargetingSystem current_target_info, 
        EnemyTaskDirector director, 
        ViewMessageWrapper.MessageType message_type, 
        GameObject evalution_object, 
        string evalution_tag)
    {
        return base.Execute(current_target_info, director, message_type, evalution_object, evalution_tag);
    }

}

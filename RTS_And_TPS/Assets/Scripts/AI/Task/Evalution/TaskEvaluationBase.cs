using UnityEngine;
using System.Collections;

public class TaskEvaluationBase : MonoBehaviour {

   public virtual float Execute(TargetingSystem current_target_info,
        EnemyTaskDirector director,
        ViewMessageWrapper.MessageType message_type,
        GameObject evalution_object,
        string evalution_tag)
    {
        Debug.Log("TaskEvaluationBase::Executeが呼ばれている");
        return .0f;
    }
       

}

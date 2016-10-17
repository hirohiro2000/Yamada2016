using UnityEngine;
using System.Collections;

public class TaskEvaluationBase : MonoBehaviour {

    protected readonly float m_base_score = 100.0f;

   public virtual float Execute(TargetingSystem current_target_info,
        EnemyTaskDirector director)
    {
        Debug.Log("TaskEvaluationBase::Executeが呼ばれている");
        return .0f;
    }
       


}

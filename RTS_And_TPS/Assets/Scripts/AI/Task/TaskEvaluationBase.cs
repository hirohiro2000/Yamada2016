using UnityEngine;
using System.Collections;

public class TaskEvaluationBase : MonoBehaviour {

   public virtual bool Execute(TargetingSystem target_info,EnemyTaskDirector director)
    {
        Debug.Log("TaskEvaluationBase::Executeが呼ばれている");
        return false;
    }
       

}

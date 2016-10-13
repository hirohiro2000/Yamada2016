using UnityEngine;
using System.Collections;

public class TaskBase : MonoBehaviour {

	// Use this for initialization
    
    public virtual void Execute(TargetingSystem target_system,
                                           EnemyTaskDirector task_director
                                           )
    {
        Debug.Log("TaskBase::Execute call");
    }

    public virtual void Enter(TargetingSystem target_system,
                                       EnemyTaskDirector task_director
                                       )
    {
        Debug.Log("TaskBase::Enter call");
    }

    public virtual void Exit(TargetingSystem target_system,
                                       EnemyTaskDirector task_director
                                       )
    {
        Debug.Log("TaskBase::Exit call");
    }
}

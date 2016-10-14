using UnityEngine;
using System.Collections;

public class TaskBase : MonoBehaviour {

    protected TaskEvaluationBase m_rater;
    
    void Awake()
    {
        m_rater = GetComponent<TaskEvaluationBase>();
        if (!m_rater)
            Debug.Log("this task not attach evalution object !!");
    }
               
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

    public float EvalutionScore(
        TargetingSystem current_target_info,
        EnemyTaskDirector director,
        ViewMessageWrapper.MessageType message_type,
        GameObject evalution_object,
        string evalution_tag
        )
    {
       return m_rater.Execute(current_target_info, director,message_type,evalution_object,evalution_tag);
    }
}

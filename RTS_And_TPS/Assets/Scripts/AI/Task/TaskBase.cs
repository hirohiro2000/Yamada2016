using UnityEngine;
using System.Collections;

public class TaskBase : MonoBehaviour {

    protected TaskEvaluationBase m_rater;
    protected GameObject           m_owner_object;


    public enum Status
    {
        Active,
        Completed,
        Failed,
    }

    //Awakeが何故か呼ばれなかった
    void Awake()
    {
      
    }
    
    public virtual void SetWaveParametor(EnemyWaveParametor wave_param,
        EnemyPersonalParametor parsonal_param)
    {
        
    }

    public virtual Status Execute(TargetingSystem target_system,
                                           EnemyTaskDirector task_director
                                           )
    {
        UserLog.Terauchi("TaskBase::Execute call");
        return Status.Failed;
    }

    public virtual void Enter(TargetingSystem target_system,
                                       EnemyTaskDirector task_director
                                       )
    {
        //Debug.Log("TaskBase::Enter call");
    }

    public virtual void Exit(TargetingSystem target_system,
                                       EnemyTaskDirector task_director
                                       )
    {
      //  Debug.Log("TaskBase::Exit call");
    }

    public float EvalutionScore(
        TargetingSystem current_target_info,
        EnemyTaskDirector director
        )
    {
       return m_rater.Execute(current_target_info, director);
    }

    public virtual void Initialize(GameObject owner)
    {
        m_owner_object = owner;
        m_rater = GetComponent<TaskEvaluationBase>();
        if (!m_rater)
            UserLog.Terauchi(gameObject.name +  "not attach task evalution object !!");
    }
}

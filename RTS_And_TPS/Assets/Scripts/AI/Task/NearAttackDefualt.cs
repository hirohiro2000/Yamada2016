using UnityEngine;
using System.Collections;

public class NearAttackDefualt : TaskBase {

    private int timer = 120;
    private bool flg = false;

    public override void Initialize(GameObject owner)
    {
        base.Initialize(owner);
    }

    public override void Enter(TargetingSystem target_system, EnemyTaskDirector task_director)
    {
        //  base.Enter(target_system, task_director)
        Debug.Log("Attack !!");
        flg = false;
        timer = 120;
        task_director.m_anime_controller.SetTrigger("ToNearAttack");
    }

    public override TaskBase.Status Execute(TargetingSystem target_system, EnemyTaskDirector task_director)
    {
        timer--;
        if(timer < 0)
            return Status.Completed;

        return TaskBase.Status.Active;
    }

    public override void Exit(TargetingSystem target_system, EnemyTaskDirector task_director)
    {
        base.Exit(target_system, task_director);
    }
}

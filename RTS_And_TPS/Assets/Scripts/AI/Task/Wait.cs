using UnityEngine;
using System.Collections;

public class Wait : TaskBase
{

    public override void Enter(
        TargetingSystem target_system,
        EnemyTaskDirector task_director)
    {
        AnimationController rAnimeCtrl  =   task_director.m_anime_controller;
        if( rAnimeCtrl ){
            rAnimeCtrl.SetTrigger("ToWait");
        }
    }

    public override Status Execute(TargetingSystem target_system, EnemyTaskDirector task_director)
    {
        UserLog.Terauchi(m_owner_object.name + "call Task::Wait !!");
        return Status.Active;
    }

    public override void Exit(TargetingSystem target_system, EnemyTaskDirector task_director)
    {
       // base.Exit(target_system, task_director);
    }

    public override void Initialize(GameObject owner)
    {
        base.Initialize(owner);
    }
}

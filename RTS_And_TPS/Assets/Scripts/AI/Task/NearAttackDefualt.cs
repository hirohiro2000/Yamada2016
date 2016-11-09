﻿using UnityEngine;
using System.Collections;

public class NearAttackDefualt : TaskBase {

    private int timer = 120;
    //private bool flg = false;

    [SerializeField,Range(.0f,1.0f)]
    float m_attack_begin_time = .0f;

    [SerializeField, Range(.0f, 1.0f)]
    float m_attack_end_time = 0.5f;

    private GameObject m_damage_object = null;

    public override void Initialize(GameObject owner)
    {
        base.Initialize(owner);
        m_damage_object = transform.FindChild("DamageObject").gameObject;
        m_damage_object.SetActive(false);

                
    }

    public override void Enter(TargetingSystem target_system, EnemyTaskDirector task_director)
    {
        //  base.Enter(target_system, task_director)
        //Debug.Log("Attack !!");
       // flg = false;
        timer = 120;
        task_director.m_anime_controller.SetTrigger("ToNearAttack");
    }

    public override TaskBase.Status Execute(TargetingSystem target_system, EnemyTaskDirector task_director)
    {
        var temp = task_director.m_anime_controller.GetCurrentAnimatorStateInfo();
        float current_time = temp.normalizedTime;
     if(current_time >= m_attack_begin_time && 
            current_time <m_attack_end_time)
        {
            m_damage_object.SetActive(true);
        }
        else
        {
            m_damage_object.SetActive(false);
        }
        timer--;
        if(timer < 0)
            return Status.Completed;

        return TaskBase.Status.Active;
    }

    public override void Exit(TargetingSystem target_system, EnemyTaskDirector task_director)
    {
       // base.Exit(target_system, task_director);
        m_damage_object.SetActive(false);
    }
}

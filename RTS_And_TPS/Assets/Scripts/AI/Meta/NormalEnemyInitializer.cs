﻿using UnityEngine;
using System.Collections;

public class NormalEnemyInitializer : EnemyInitializerBase {


    public override void Execute(Vector3 respawn_pos, StringList route_list, int level, float HPCorrectionRate)
    {
        var nav_agent = gameObject.GetComponent<NavMeshAgent>();
        nav_agent.Warp(respawn_pos);

        //通る経路の設定
        var controller = gameObject.GetComponent<EnemyController>();
        if (!controller)
        {
            UserLog.ErrorTerauchi(gameObject.name + "not attach EnemyController!!");
        }
        else
        {
            controller.SetRouteData(route_list);
        }
        //体力設定     
        var health = gameObject.GetComponent<Health>();
        if (!health)
        {
            UserLog.ErrorTerauchi(gameObject.name + "no attach Health !!");
        }
        else
        {
            health.CorrectionHP(level, HPCorrectionRate);
        }
    }

}
using UnityEngine;
using System.Collections;

public class CrowdEnemyInitializer : EnemyInitializerBase {

    IEnumerator Init(Vector3 respawn_pos,
        StringList route_list,
        int level,
        float HPCorrectionRate)
    {
        yield return null;
        transform.position = respawn_pos;
        for (int enemy_i = 0; enemy_i < gameObject.transform.childCount; enemy_i++)
        {
            GameObject init_object = transform.GetChild(enemy_i).gameObject;
            Vector3 pos = respawn_pos;
            pos += new Vector3(UnityEngine.Random.Range(-0.5f, 0.5f), .0f, UnityEngine.Random.Range(-0.5f, 0.5f));
            var nav_agent = init_object.GetComponent<NavMeshAgent>();
            nav_agent.Warp(respawn_pos);

            //通る経路の設定
            var controller = init_object.GetComponent<EnemyController>();
            if (!controller)
            {
                UserLog.ErrorTerauchi(gameObject.name + "not attach EnemyController!!");
            }
            else
            {
                controller.SetRouteData(route_list);
            }

            //体力設定     
            var health = init_object.GetComponent<Health>();
            if (!health)
            {
                UserLog.ErrorTerauchi(init_object.name + "no attach Health !!");
            }
            else
            {
                health.CorrectionHP(level, HPCorrectionRate);
            }

        }//enemy_i
    }

    public override void Execute(Vector3 respawn_pos,
        StringList route_list,
        int level,
        float HPCorrectionRate)
    {
        StartCoroutine(Init(respawn_pos, route_list, level, HPCorrectionRate));
    }

}

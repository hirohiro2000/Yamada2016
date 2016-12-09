using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class CrowdEnemyInitializer : EnemyInitializerBase {

    public  GameObject  c_GenerateEnemy =   null;
    public  int         c_PopCount      =   5;

    IEnumerator Init(Vector3 respawn_pos,
                               EnemyWaveParametor param)
    {
        //  エネミーを生成
        for( int i = 0; i < c_PopCount; i++ ){
            GameObject      rObj    =   Instantiate( c_GenerateEnemy );
            NavMeshAgent    rAgent  =   rObj.GetComponent< NavMeshAgent >();
            yield return null;
            //  初期座標にワープ 
            rAgent.Warp( respawn_pos );

            //  ネットワーク上でオブジェクトを共有
            NetworkServer.Spawn( rObj );
            
            //  体力設定     
            Health  rHealth =   rObj.GetComponent< Health >();
            if( rHealth )   rHealth.CorrectionHP(/*param.m_current_level*/1, param.GetHPUpPoint());
            else            UserLog.ErrorTerauchi( rObj.name + "no attach Health !!" );

            var controller = rObj.GetComponent<EnemyController>();
            controller.SetWaveParametor(param);
        }

        //  殻は破棄する
        Destroy( gameObject );

        //for (int enemy_i = 0; enemy_i < gameObject.transform.childCount; enemy_i++)
        //{
        //    GameObject init_object = transform.GetChild(enemy_i).gameObject;
        //    Vector3 pos = respawn_pos;
        //    pos += new Vector3(UnityEngine.Random.Range(-0.5f, 0.5f), .0f, UnityEngine.Random.Range(-0.5f, 0.5f));
        //    var nav_agent = init_object.GetComponent<NavMeshAgent>();
        //    nav_agent.Warp(respawn_pos);

        //    //通る経路の設定
        //    var controller = init_object.GetComponent<EnemyController>();
        //    if (!controller)
        //    {
        //        UserLog.ErrorTerauchi(gameObject.name + "not attach EnemyController!!");
        //    }
        //    else
        //    {
        //        controller.SetRouteData(route_list);
        //    }

        //    //  先に生成
        //    NetworkServer.Spawn(init_object);

        //    //体力設定     
        //    var health = init_object.GetComponent<Health>();
        //    if (!health)
        //    {
        //        UserLog.ErrorTerauchi(init_object.name + "no attach Health !!");
        //    }
        //    else
        //    {
        //        health.CorrectionHP(level, HPCorrectionRate);
        //    }

        //    Debug.Log( "Call" );
        //}//enemy_i
    }

    public override void Execute(Vector3 respawn_pos,
                                        EnemyWaveParametor param)
    {
        StartCoroutine(Init(respawn_pos, param));
    }

}

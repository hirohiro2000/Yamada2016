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
			//NavMeshのWarpは遅延があるので最初のフレームは遠くに飛ばしておいて隠す
			rObj.transform.position = new Vector3(10000.0f, 10000.0f, 10000.0f);
            NavMeshAgent    rAgent  =   rObj.GetComponent< NavMeshAgent >();
            var controller = rObj.GetComponent<EnemyController>();
            controller.SetWaveParametor(param);
            yield return null;
            //  初期座標にワープ 
            //ばらつきを入れる
            Vector3 random_point = new Vector3(UnityEngine.Random.Range(-0.5f, 0.5f),
                .0f,
                UnityEngine.Random.Range(-0.5f, 0.5f));
            rAgent.Warp( respawn_pos + random_point);

            //  ネットワーク上でオブジェクトを共有
            NetworkServer.Spawn( rObj );

            //個体別のparameter取得
            //var personal_param = rObj.GetComponent<EnemyPersonalParametor>();
            //if(personal_param == null)
            //{
            //    UserLog.Terauchi(rObj.name + "not attach EnemyPersonalParametor!!");
            //}
            
            ////  体力設定     
            //Health  rHealth =   rObj.GetComponent< Health >();
            //if( rHealth )   rHealth.CorrectionHP(param.m_current_level - personal_param.m_emearge_level, personal_param.GetHPUpMultipleRate());
            //else            UserLog.ErrorTerauchi( rObj.name + "no attach Health !!" );

    
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

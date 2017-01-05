using UnityEngine;
using System.Collections;

public class DroneRearguard : MonoBehaviour
{
    private ReferenceWrapper    m_rEnemyShell   = null;
    private float               m_interval      = 0.0f;


    // Use this for initialization
    void Start()
    {
        m_rEnemyShell = FunctionManager.GetAccessComponent<ReferenceWrapper>("EnemySpawnRoot");

    }

    // Update is called once per frame
    void Update()
    {
        //  アクセスの取得
        if (!m_rEnemyShell) m_rEnemyShell = FunctionManager.GetAccessComponent<ReferenceWrapper>("EnemySpawnRoot");
        if (!m_rEnemyShell) return;

        if ( m_interval > 0.0f )
        {
            m_interval -= Time.deltaTime;
            return;
        }


        transform.localRotation = Quaternion.identity;

        float       near        = 100.0f;
        GameObject  nearEnemy   = null;
        for (int i = 0; i < m_rEnemyShell.m_active_enemy_list.Count; i++)
        {
            //  エネミーへのアクセス
            GameObject rEnemy = m_rEnemyShell.m_active_enemy_list[i];

            Vector3 distance = ( rEnemy.transform.position - transform.position );
            float   dot      = Vector3.Dot( transform.forward, distance );

            if ( dot > near )       continue;

            RaycastHit  hit             = new RaycastHit();
            int         layerMask       = LayerMask.GetMask( "Field" );
            Ray         ray             = new Ray( transform.position, distance.normalized );
    
            if ( Physics.Raycast(ray, out hit, distance.magnitude, layerMask) ) continue;


            near        = dot;
            nearEnemy   = rEnemy;
        }

        if ( nearEnemy == null )    return;
        
        m_interval = 2.0f;

        transform.LookAt( nearEnemy.transform );
        GameObject bullet = (GameObject)GetComponent<DroneShotController>().Fire();
        bullet.transform.localScale    = new Vector3( 0.5f, 0.5f, 0.5f );
        bullet.GetComponent<Rigidbody>().velocity *= 0.1f;


    }
     
}

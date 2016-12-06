using UnityEngine;
using System.Collections;

public class EnemyAtkTargetManager : MonoBehaviour {

	[SerializeField]
	Transform[] targets = null;

    //private LinkManager m_rLinkManager  =   null;

	// Use this for initialization
	void    Start()
    {
	    //m_rLinkManager  =   FunctionManager.GetAccessComponent< LinkManager >( "LinkManager" );
	}
	
	// Update is called once per frame
	void    Update()
    {

	}

	public  Transform getNearestTarget(Vector3 point,float radius, bool Zenable = true)
	{
		float sqrRadius = radius * radius;
		float minSqrDistance = float.MaxValue;
		Transform ret = null;

        for (int i = 0; i < targets.Length; i++)
        {
            Transform   rTrans  =   targets[ i ];
            if( !rTrans )   continue;

            Vector3 len = rTrans.position - point;
            if (Zenable)
                len.z = .0f;

            float sqrDistance = len.sqrMagnitude;
            if (minSqrDistance > sqrDistance)
            {
                minSqrDistance = sqrDistance;
                ret = rTrans;
            }
        }
        //  プレイヤーを探す
        {
            GameObject[]    playerList  =   GameObject.FindGameObjectsWithTag( "Player" );
            for( int i = 0; i < playerList.Length; i++ ){
                Transform           rTrans      =   playerList[ i ].transform;
                TPSPlayer_Control   rControl    =   rTrans.GetComponent< TPSPlayer_Control >();
                if( !rControl ) continue;

                //  座標補正
                Vector3     transPosition   =   rTrans.position + Vector3.up * 1.5f;

                //  同じ処理をプレイヤーにも
                {
			        Vector3 len = transPosition - point;
			        if (Zenable)
				        len.z = .0f;

			        float sqrDistance = len.sqrMagnitude;
			        if (minSqrDistance > sqrDistance)
			        {
				        minSqrDistance = sqrDistance;
				        ret = rTrans;
			        }
                }
            }
        }

		if (minSqrDistance > sqrRadius)
			return null;


		return ret;
	}
}

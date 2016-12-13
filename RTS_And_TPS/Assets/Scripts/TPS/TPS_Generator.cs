
//using UnityEngine;
//using UnityEngine.Networking;
//using System.Collections;
//using System.Collections.Generic;

//public class TPS_Generator : MonoBehaviour {

//	[SerializeField, Range(.0f, 10.0f)]
//	private float m_resporn_interval_second = 2.0f;

//	//[SerializeField]
//	//private GameObject m_generate_object = null;

//	// Use this for initialization
//	void Start()
//	{
//		//StartCoroutine(Resporn());
//	}

//	//IEnumerator Resporn()
//	//{
//	//	while (true)
//	//	{
// //           int activeCount =   CheckActiveChildCount();
// //           if( activeCount > 0 )
//	//		{
// //               int useSpawnID  =   Random.Range( 0, activeCount );

//	//		    //子供のPosと向き配置します
//	//		    var new_object = Instantiate(m_generate_object);
//	//		    var resporn_object = GetTransformInActiveChild( useSpawnID );
//	//		    var agent = new_object.GetComponent<NavMeshAgent>();
//	//		    if (agent != null)
//	//			    agent.Warp(resporn_object.position);
//	//		    else
//	//			    new_object.transform.position = resporn_object.position;
//	//		    new_object.transform.rotation = resporn_object.rotation;

//	//		//cloneをまとめる
//	//		string parentName = new_object.name + "s";
//	//		GameObject parent = GameObject.Find(parentName);
//	//		if (parent == null)
//	//		{

// //               //  ネットワーク上で生成
// //               NetworkServer.Spawn( new_object );
// //           }

//	//		yield return new WaitForSeconds(m_resporn_interval_second);

//	//	}

//	//}
//    int         CheckActiveChildCount()
//    {
//        int activeCount =   0;
//        for( int i = 0; i < transform.childCount; i++ ){
//            Transform   rTrans  =   transform.GetChild( i );
//            if( rTrans.gameObject.activeInHierarchy == false )  continue;

//             ++activeCount;
//        }

//        return  activeCount;
//    }
//    Transform   GetTransformInActiveChild( int _ID )
//    {
//        int activeCount =   0;
//        for( int i = 0; i < transform.childCount; i++ ){
//            Transform   rTrans  =   transform.GetChild( i );
//            if( rTrans.gameObject.activeInHierarchy == false )  continue;
//            if( activeCount == _ID )                            return  rTrans;

//            ++activeCount;
//        }

//        return  null;
//    }
//}

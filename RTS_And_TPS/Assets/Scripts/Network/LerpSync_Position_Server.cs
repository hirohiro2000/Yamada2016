
using   UnityEngine;
using   UnityEngine.Networking;
using   System.Collections;

public class LerpSync_Position_Server : NetworkBehaviour {

    enum    SyncState{
        StartWait,
        StartOK,
        SyncStart,
    }

    public  float       c_LerpRatio     =   0.1f;

	[ SyncVar ]
    private Vector3     m_SyncPosition  =   Vector3.zero;
    [ SyncVar ]
    private SyncState   m_SyncState     =   SyncState.StartWait;

    void    FixedUpdate()
    {
        //  自分のキャラなら座標を送信
        if( NetworkServer.active ){
            m_SyncPosition  =   transform.position;
            if( m_SyncState == SyncState.StartWait ){
                m_SyncState =   SyncState.StartOK;
            }
        }
        //  他人のキャラなら共有座標を適用
        else{
            //  開始時はぴったり合わせる
            if( m_SyncState == SyncState.StartOK ){
                transform.position  =   m_SyncPosition;
                m_SyncState         =   SyncState.SyncStart;
            }
            //  開始以降は反映を少し遅らせる
            else
            if( m_SyncState == SyncState.SyncStart ){
                transform.position  +=  ( m_SyncPosition - transform.position ) * c_LerpRatio;
            }
            //  開始までは遠くに飛ばしておく
            else{
                transform.position  =   Vector3.one * 10000 * 100;
            }
        }
    }
}

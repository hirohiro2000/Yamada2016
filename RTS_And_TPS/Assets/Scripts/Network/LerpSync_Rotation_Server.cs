
using   UnityEngine;
using   UnityEngine.Networking;
using   System.Collections;

public class LerpSync_Rotation_Server : NetworkBehaviour {

    enum    SyncState{
        StartWait,
        StartOK,
        SyncStart,
    }

    public  float       c_LerpRatio     =   0.1f;

	[ SyncVar ]
    private Quaternion  m_SyncPose      =   Quaternion.identity;
    [ SyncVar ]
    private SyncState   m_SyncState     =   SyncState.StartWait;

    void    FixedUpdate()
    {
        //  自分のキャラなら座標を送信
        if( NetworkServer.active ){
            m_SyncPose  =   transform.rotation;
            if( m_SyncState == SyncState.StartWait ){
                m_SyncState =   SyncState.StartOK;
            }
        }
        //  他人のキャラなら共有座標を適用
        else{
            //  開始時はぴったり合わせる
            if( m_SyncState == SyncState.StartOK ){
                transform.rotation  =   m_SyncPose;
                m_SyncState         =   SyncState.SyncStart;
            }
            //  開始以降は反映を少し遅らせる
            else
            if( m_SyncState == SyncState.SyncStart ){
                transform.rotation  =   Quaternion.Lerp( transform.rotation, m_SyncPose, c_LerpRatio );
            }
        }
    }
}

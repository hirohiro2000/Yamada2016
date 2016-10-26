
using   UnityEngine;
using   UnityEngine.Networking;
using   System.Collections;

public class LerpSync_Rotation : NetworkBehaviour {
    
    enum    SyncState{
        StartWait,
        SyncStart,
    }

    public  float       c_LerpRatio     =   0.1f;

    [ SyncVar ]
    private Quaternion  m_SyncPose      =   Quaternion.identity;
    [ SyncVar ]
    private bool        m_IsStart       =   false;
    private SyncState   m_SyncState     =   SyncState.StartWait;

    void    FixedUpdate()
    {
        //  自分のキャラなら座標を送信
        if( isLocalPlayer ){
            if( isServer )  UpdateSyncPose( transform.rotation );
            else            CmdUpdateSyncPose( transform.rotation );
        }
        //  他人のキャラなら共有座標を適用
        else    if( m_IsStart ){
            //  開始時はぴったり合わせる
            if( m_SyncState == SyncState.StartWait ){
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

    //  ローカル
    void    UpdateSyncPose( Quaternion _Pose )
    {
        //  共有座標更新
        m_SyncPose  =   _Pose;
        m_IsStart   =   true;
    }
    //  コマンド
    [ Command ]
	void    CmdUpdateSyncPose( Quaternion _Pose )
	{
        UpdateSyncPose( _Pose );
	}
}


using   UnityEngine;
using   UnityEngine.Networking;
using   System.Collections;

public class LerpSync_Position : NetworkBehaviour {

    enum    SyncState{
        StartWait,
        SyncStart,
    }

    public  float       c_LerpRatio     =   0.1f;

    [ SyncVar ]
    private Vector3     m_SyncPosition  =   Vector3.zero;
    [ SyncVar ]
    private bool        m_IsStart       =   false;
    private SyncState   m_SyncState     =   SyncState.StartWait;

    void    FixedUpdate()
    {
        //  自分のキャラなら座標を送信
        if( isLocalPlayer ){
            if( NetworkServer.active )  UpdateSyncPosition( transform.position );
            else                        CmdUpdateSyncPosition( transform.position );
        }
        //  他人のキャラなら共有座標を適用
        else    if( m_IsStart ){
            //  開始時はぴったり合わせる
            if( m_SyncState == SyncState.StartWait ){
                transform.position  =   m_SyncPosition;
                m_SyncState         =   SyncState.SyncStart;
            }
            //  開始以降は反映を少し遅らせる
            else    if( m_SyncState == SyncState.SyncStart ){
                transform.position  +=  ( m_SyncPosition - transform.position ) * c_LerpRatio;
            }
        }
    }

    //  ローカル
    void    UpdateSyncPosition( Vector3 _Position )
    {
        //  共有座標更新
        m_SyncPosition  =   _Position;
        m_IsStart       =   true;
    }
    //  コマンド
    [ Command ]
    void    CmdUpdateSyncPosition( Vector3 _Position )
    {
        //  共有座標更新
        UpdateSyncPosition( _Position );
    }
}

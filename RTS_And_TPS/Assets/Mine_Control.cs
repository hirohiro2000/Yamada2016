
using   UnityEngine;
using   UnityEngine.Networking;
using   System.Collections;

public class Mine_Control : NetworkBehaviour {

    private LinkManager     m_rLinkManager  =   null;
    private BombExplosion   m_rBomb         =   null;

	// Use this for initialization
	void    Start()
    {
	    m_rLinkManager  =   FunctionManager.GetAccessComponent< LinkManager >( "LinkManager" );
        m_rBomb         =   GetComponent< BombExplosion >();
	}

    void    OnTriggerEnter( Collider _rCollider )
    {
        //  エネミー以外は無視
        if( _rCollider.tag != "Enemy" )                                 return;
        //  配置したプレイヤーのクライアントでのみ処理を行う
        if( m_rLinkManager.m_LocalPlayerID != m_rBomb.c_AttackerID )    return;

        //  起爆
        m_rLinkManager.m_rLocalNPControl.CmdDetonationMine( netId );
    }

    //  爆破
    [ Server ]
    public  void    Exploding( int _OwnerID )
    {
        //  オーナー設定
        GetComponent< DetonationObject >().m_DestroyerID    =   _OwnerID;

        //  爆破
        Destroy( gameObject );
    }
}


using   UnityEngine;
using   System.Collections;

public class ShopControl_LIFE : MonoBehaviour {

    private GameManager m_rGameManager  =   null;
    private LinkManager m_rLinkManager  =   null;

    public  float       c_InitCost      =   100;
    public  float       c_UPCost        =   50;
    private float       m_Cost          =   0;

    private float       c_ShopingTime   =   3.0f;
    private float       m_ShopTimer     =   0.0f;

    private float       c_Threshold     =   0.5f;
    private float       m_TouchTimer    =   0.0f;

	// Use this for initialization
	void    Start()
    {
	    m_rGameManager  =   FunctionManager.GetAccessComponent< GameManager >( "GameManager" );
        m_rLinkManager  =   FunctionManager.GetAccessComponent< LinkManager >( "LinkManager" );

        m_Cost          =   c_InitCost;
	}
	
	// Update is called once per frame
	void    Update()
    {
        //  アクセスを取得
        if( !m_rGameManager )    m_rGameManager  =   FunctionManager.GetAccessComponent< GameManager >( "GameManager" );
        if( !m_rLinkManager )    m_rLinkManager  =   FunctionManager.GetAccessComponent< LinkManager >( "LinkManager" );

        //  タイマー更新
	    m_TouchTimer    +=  Time.deltaTime;
        m_TouchTimer    =   Mathf.Min( m_TouchTimer, c_Threshold );
        if( m_TouchTimer >= c_Threshold ){
            m_ShopTimer =   0.0f;
        }
	}
    
    //  当たり判定
    void    OnTriggerStay( Collider _Collider )
    {
        //  アクセスチェック
        if( !m_rGameManager )                                           return;
        if( !m_rLinkManager )                                           return;

        //  プレイヤーかどうかチェック
        if( _Collider.tag != "Player" )                                 return;

        //  ローカルプレイヤーかどうかチェック
        NetPlayer_Control   rNetControl =   _Collider.GetComponentInParent< NetPlayer_Control >();
        if( !rNetControl )                                              return;
        if( rNetControl.c_ClientID != m_rLinkManager.m_LocalPlayerID )  return;

        //  しきい値更新
        m_TouchTimer    =   0.0f;

        //  購入タイマー更新
        m_ShopTimer     +=  Time.fixedDeltaTime;
        m_ShopTimer     =   Mathf.Min( m_ShopTimer, c_ShopingTime );
        if( m_ShopTimer < c_ShopingTime )                               return;

        //  購入
        Shoping( _Collider );

        //  タイマーリセット
        m_ShopTimer     =   0.0f;
    }
    void    Shoping( Collider _Collider )
    {
        //  資源が足りているかどうか
        if( m_rGameManager.GetResource() < m_Cost ) return;

        //  効果発動
        {
            //  体力へのアクセスを取得
            TPSPlayer_HP        rHP         =   _Collider.GetComponentInParent< TPSPlayer_HP >();
            if( !rHP )                              return;

            //  体力が満タンなら終了
            if( rHP.m_CurHP >= rHP.m_MaxHP )        return;

            //  制御を取得
            TPSPlayer_Control   rTPSControl =   _Collider.GetComponentInParent< TPSPlayer_Control >();
            RTSPlayer_Control   rRTSControl =   _Collider.GetComponentInParent< RTSPlayer_Control >();

            //  体力を回復
            if( rTPSControl )   rTPSControl.CmdSendDamage( -rHP.m_MaxHP );
            if( rRTSControl )   rRTSControl.CmdSendDamage( -rHP.m_MaxHP );

            //  プレイヤーに通知
            m_rGameManager.SetAcqRecord( "体力が回復した！ - " + m_Cost, m_rLinkManager.m_LocalPlayerID );
        }

        //  購入
        m_rLinkManager.m_rLocalNPControl.CmdAddResource( -m_Cost );

        //  購入コスト増加
        m_Cost  +=  c_UPCost;
    }
}


using   UnityEngine;
using   System.Collections;

public  class   ShopControl_LIFE    :   ShopControl_Base{
//================================================================================
//      初期化
//================================================================================
    protected   override    void    Initialize()
    {

    }
//================================================================================
//      更新
//================================================================================
    protected   override    void    Update_Proc()
    {

    }
//================================================================================
//      購入された効果を適用
//================================================================================
    protected   override    bool    Apply_PurchasedEffect( Collider _rCollider )
    {
        //  体力へのアクセスを取得
        TPSPlayer_HP        rHP         =   _rCollider.GetComponentInParent< TPSPlayer_HP >();
        if( !rHP )                              return  false;
        
        //  体力が満タンなら終了
        if( rHP.m_CurHP >= rHP.m_MaxHP )        return  false;
        
        //  制御を取得
        TPSPlayer_Control   rTPSControl =   _rCollider.GetComponentInParent< TPSPlayer_Control >();
        RTSPlayer_Control   rRTSControl =   _rCollider.GetComponentInParent< RTSPlayer_Control >();
        
        //  体力を回復
        if( rTPSControl )   rTPSControl.CmdSendDamage( -rHP.m_MaxHP );
        if( rRTSControl )   rRTSControl.CmdSendDamage( -rHP.m_MaxHP );
        
        //  プレイヤーに通知
        m_rGameManager.SetAcqRecord( "体力が回復した！ - " + m_Cost, m_rLinkManager.m_LocalPlayerID );

        return  true;
    }
}

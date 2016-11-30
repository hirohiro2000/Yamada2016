
using   UnityEngine;
using   System.Collections;

public  class   ShopControl_ARMORY  :   ShopControl_Base{
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
        //  制御を取得
        TPSPlayer_Control   rTPSControl =   _rCollider.GetComponentInParent< TPSPlayer_Control >();
        if( !rTPSControl )                          return  false;

        //  弾が満タンなら終了
        if( rTPSControl.CheckWhetherIsFullAmmo() )  return  false;

        //  弾薬を補充
        if( rTPSControl )   rTPSControl.SupplyAmmo();

        //  プレイヤーに通知
        m_rGameManager.SetAcqRecord( "弾薬を購入した！ - " + m_Cost, m_rLinkManager.m_LocalPlayerID );

        return  true;
    }
}


using   UnityEngine;
using   System.Collections;

public class ShopControl_Base : MonoBehaviour {
    
    public      float       c_InitCost      =   150;
    public      float       c_UPCost        =   100;

    protected   float       m_Cost          =   0;

    protected   float       c_ShopingTime   =   2.5f;
    protected   float       m_ShopTimer     =   0.0f;

    protected   float       c_Threshold     =   0.5f;
    protected   float       m_TouchTimer    =   0.0f;

    protected   TextMesh    m_rCostText     =   null;
    protected   GameManager m_rGameManager  =   null;
    protected   LinkManager m_rLinkManager  =   null;

    protected   GameObject  m_rShoppingUI   =   null;
    protected   GageControl m_rShopGage     =   null;

	//  Use this for initialization
	void    Start()
    {
	    m_rGameManager      =   FunctionManager.GetAccessComponent< GameManager >( "GameManager" );
        m_rLinkManager      =   FunctionManager.GetAccessComponent< LinkManager >( "LinkManager" );

        Transform   rCanvas =   GameObject.Find( "Canvas" ).transform;
        if( rCanvas ){
            Transform   rShopTrans  =   rCanvas.FindChild( "ShoppingGage" );
            if( rShopTrans ){
                m_rShoppingUI   =   rShopTrans.gameObject;
                m_rShopGage     =   m_rShoppingUI.GetComponent< GageControl >();
            }
        }

        m_rCostText         =   transform.FindChild( "_TextCost" ).GetComponent< TextMesh >();

        m_Cost              =   c_InitCost;

        //  コスト表示更新
        Update_CostText();

        //  初期化処理
        Initialize();
	}
    //  初期化処理（継承用）
    protected   virtual void    Initialize()
    {

    }
	
	//  Update is called once per frame
	void    Update()
    {
        //  アクセスを取得
        if( !m_rGameManager )    m_rGameManager  =   FunctionManager.GetAccessComponent< GameManager >( "GameManager" );
        if( !m_rLinkManager )    m_rLinkManager  =   FunctionManager.GetAccessComponent< LinkManager >( "LinkManager" );

        if( !m_rShoppingUI ){
            Transform   rCanvas =   GameObject.Find( "Canvas" ).transform;
            if( rCanvas ){
                Transform   rShopTrans  =   rCanvas.FindChild( "ShoppingGage" );
                if( rShopTrans ){
                    m_rShoppingUI   =   rShopTrans.gameObject;
                    m_rShopGage     =   m_rShoppingUI.GetComponent< GageControl >();
                }
            }
        }

        //  タイマー更新
        if( m_ShopTimer > 0.0f ){
            //  UI更新
            m_rShopGage.SetGage( m_ShopTimer / c_ShopingTime );
            m_rShoppingUI.SetActive( true );

            //  タッチタイマー更新
	        m_TouchTimer    +=  Time.deltaTime;
            m_TouchTimer    =   Mathf.Min( m_TouchTimer, c_Threshold );
            if( m_TouchTimer >= c_Threshold ){
                m_ShopTimer =   0.0f;
                m_rShoppingUI.SetActive( false );
            }
        }

        //  更新処理
        Update_Proc();
	}
    //  更新処理（継承用）
    protected   virtual void    Update_Proc()
    {

    }

    //  コスト表示更新
    void    Update_CostText()
    {
        m_rCostText.text    =   "( " + m_Cost + " R )";
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

        //  購入条件を満たしているかどうかチェック
        if( !Check_Condisions( _Collider ) )                            return;
        //  資源が足りているかどうかチェック
        if( m_rGameManager.GetResource() < m_Cost )                     return;

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
        m_rShoppingUI.SetActive( false );
    }
    //  購入処理
    void    Shoping( Collider _Collider )
    {
        //  資源が足りているかどうか
        if( m_rGameManager.GetResource() < m_Cost )     return;

        //  購入した効果を適用
        if( !Apply_PurchasedEffect( _Collider ) )       return;

        //  購入
        m_rLinkManager.m_rLocalNPControl.CmdAddResource( -m_Cost );

        //  支出を通知
        m_rGameManager.SetAcqResource_Minus( m_Cost );

        //  購入コスト増加
        m_Cost  +=  c_UPCost;

        //  コスト表示更新
        Update_CostText();
    }
    //  購入した効果を適用（継承用、購入しなかった場合はfalseを返す）
    protected   virtual bool    Apply_PurchasedEffect( Collider _rCollder )
    {
        return  false;
    }
    //  購入条件を満たしているかどうかチェック（継承用、条件を満たしていなかった場合はfalseを返す）
    protected   virtual bool    Check_Condisions( Collider _rCollider )
    {
        return  false;
    }
}

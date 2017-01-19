
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;


[ System.Serializable ]
public  class   ExplodedData{
    public  Transform   rPosition   =   null;
    public  GameObject  rEmitObj    =   null;
}

[System.Serializable]
public class LevelParam
{
	public int			hp			= 0;
	public int			power		= 0;
	public float		range		= 1.0f;
	public float		interval	= 0;
	[SerializeField]
	private int			upCost		= 0;

	public int GetUpCost()
	{
		return (int)(upCost * GameWorldParameter.instance.RTSPlayer.ResourceLevelUpCostMultiple);
	}
}


[System.Serializable]
public class LevelUpParamReorderableList : ReorderableList<LevelParam>
{
}


public class ResourceParameter : NetworkBehaviour
{
    public  GameObject              c_HitEmission   = null;
    public  ExplodedData[]          c_ExplodedData  = null;
    public  GameObject              c_DestroyEmit   = null;

	public  string		            m_name;
	public  string		            m_summary;

	[SerializeField]
	private int			            m_createCost    = 0;
	[SerializeField]
	private int			            m_breakCost	    = 0;

	[HideInInspector, SyncVar]
	public int			            m_level         = 0;

	[HideInInspector, SyncVar]
	public int			            m_curHp         = 0;

	[ ReorderableList( new int[]{ 50, 50, 50, 50, 50 }), HeaderAttribute ("体力ー火力ー射程[radius]ー発射間隔[sec]ーレベルアップ費用")]
	public LevelUpParamReorderableList m_levelInformations = null;


    private RTSResourece_Control    m_rResControl   = null;
    private DamageBank              m_rDamageBank   = null;

    private LinkManager             m_rLinkManager  = null;
    private GameManager             m_rGameManager  = null;
    private bool                    m_IsGameQuit    = false;

	void Start()
	{
        m_rLinkManager  =   FunctionManager.GetAccessComponent< LinkManager >( "LinkManager" );
        m_rGameManager  =   FunctionManager.GetAccessComponent< GameManager >( "GameManager" );

        m_rResControl   =   GetComponent< RTSResourece_Control >();
        m_rDamageBank   =   GetComponent< DamageBank >();
		m_curHp         =   GetCurLevelParam().hp;

        //  ダメージ処理の設定（DamageBankがアタッチされているオブジェクトのみ）
        if( m_rDamageBank ){
            m_rDamageBank.AdvancedDamagedCallback   +=  DamageProc_CallBack;
        }
	}
    void    OnEnable()
    {
        if( m_rDamageBank ){
            m_curHp         =   GetCurLevelParam().hp;
        }
    }
    void    OnApplicationQuit()
    {
        m_IsGameQuit    =   true;
    }
    public  override    void    OnNetworkDestroy()
    {
        base.OnNetworkDestroy();

        //  ゲーム終了時は処理を行わない
        if( m_IsGameQuit )                                          return;
        //  ゲーム中以外は処理を行わない
        if( !m_rGameManager )                                       return;
        if( m_rGameManager.GetState() > GameManager.State.InGame )  return;

        //  破砕オブジェクト生成
        if( c_ExplodedData != null ){
            for( int i = 0; i < c_ExplodedData.Length; i++ ){
                if( !c_ExplodedData[ i ].rEmitObj )     continue;
                if( !c_ExplodedData[ i ].rPosition )    continue;

                //  オブジェクト生成
                GameObject  rObj    =   Instantiate( c_ExplodedData[ i ].rEmitObj );
                Transform   rTrans  =   rObj.transform;
                rTrans.position     =   c_ExplodedData[ i ].rPosition.position;
                rTrans.rotation     =   c_ExplodedData[ i ].rPosition.rotation;
                rTrans.localScale   =   c_ExplodedData[ i ].rPosition.lossyScale;

                //  パラメータ設定
                ExpSylinder_Control rExpControl =   rObj.GetComponent< ExpSylinder_Control >();
                rExpControl.c_PartnerID         =   m_rResControl.c_OwnerID;
                //rExpControl.c_Score             =   m_breakCost / c_ExplodedData.Length;
            }
        }
        //  破壊時の生成オブジェクト
        if( c_DestroyEmit ){
            GameObject  rObj    =   Instantiate( c_DestroyEmit );
            Transform   rTrans  =   rObj.transform;
            rTrans.position     =   transform.position;
        }
	}

    //  ダメージ処理
    void    DamageProc_CallBack( DamageResult _rDamageResult, CollisionInfo _rInfo )
    {
        //  サーバーでのみ処理を行う
        //if( !NetworkServer.active ) return;

        //  ヒットサウンド
        if( c_HitEmission ){
            GameObject  rObj    =   Instantiate( c_HitEmission );
            Transform   rTrans  =   rObj.transform;
            rTrans.position     =   _rInfo.contactPoint;
        }

        //  オーナーのクライアントでのみダメージ処理を行う
        if( m_rLinkManager.m_LocalPlayerID == m_rResControl.c_OwnerID ){
            //  ダメージを送信
            m_rLinkManager.m_rLocalNPControl.CmdSendDamageTower( netId, ( int )_rDamageResult.GetTotalDamage() );
        }
    }


	//-------------------------------------------------------------
	//	get
	//-------------------------------------------------------------
	public LevelParam GetLevelParam( int level )
	{
		return m_levelInformations[ level ];
	}
	public LevelParam GetCurLevelParam()
	{
		return m_levelInformations[ m_level ];
	}
	public float GetRate()
	{
		return (float)m_curHp / (float)GetCurLevelParam().hp;
	}
	public bool CheckWhetherCanUpALevel()
	{
		return m_levelInformations.Length-1 > m_level;
	}
	public int GetCreateCost()
	{
		return (int)(m_createCost * GameWorldParameter.instance.RTSPlayer.ResourceCreateCostMultiple);
    }
	public int GetBreakCost()
	{
		return (int)(m_breakCost * GameWorldParameter.instance.RTSPlayer.ResourceBreakCostMultiple);
	}


	//-------------------------------------------------------------
	//	set
	//-------------------------------------------------------------
	public void LevelUp()
	{
		if( m_levelInformations.Length <= m_level+1 )
			return;

		m_level++;
		m_curHp = GetCurLevelParam().hp;

        //  強化を通知
        if( NetworkServer.active )  m_rGameManager.SetAcqRecord     ( m_name + "を強化しました！", 3.0f, m_rResControl.c_OwnerID );
                                    m_rGameManager.RpcRecordNoticeD ( m_name + "を強化しました！", 3.0f, m_rResControl.c_OwnerID );
	}
    [ Server ]
	public void GiveDamage( int damage )
	{
		m_curHp -= damage;
        m_curHp =  Mathf.Max( m_curHp, 0 );
        if( m_curHp <= 0 ){
            ////  破壊されたら非アクティブ化
            //gameObject.SetActive( false );
            ////  クライアントでも非アクティブ化するようリクエストを飛ばす
            //RpcSetActive( false );

            //  破壊を通知 
            if( NetworkServer.active )  m_rGameManager.SetAcqRecord         ( m_name + "が破壊されました！", 3.0f, m_rResControl.c_OwnerID, AcqRecord_Control.ColorType.Emergency );
                                        m_rGameManager.RpcRecordNoticeDRed  ( m_name + "が破壊されました！", 3.0f, m_rResControl.c_OwnerID );

            //  オブジェクトを破棄
            Destroy( gameObject );
        }
	}
	public void Copy( ResourceParameter param )
	{
		m_curHp = param.m_curHp;
		m_level = param.m_level;

		m_levelInformations			= new LevelUpParamReorderableList();
		m_levelInformations.list	= new List<LevelParam>();

		for( int i=0; i<param.m_levelInformations.Length; ++i )
		{
			m_levelInformations.list.Add( param.m_levelInformations[i] );
		}
	}

	public void SetCreateCost(int value)
	{
		m_createCost = value;
	}

	//  リクエスト
	[ ClientRpc ]
    void    RpcSetActive( bool _IsActive )
    {
        gameObject.SetActive( _IsActive );
    }
}
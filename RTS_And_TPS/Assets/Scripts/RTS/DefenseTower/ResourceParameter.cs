
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;


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
	public string		m_name;
	public string		m_summary;

	[SerializeField]
	private int			m_createCost		= 0;
	[SerializeField]
	private int			m_breakCost			= 0;

	[HideInInspector, SyncVar]
	public int			m_level				= 0;

	[HideInInspector, SyncVar]
	public int			m_curHp				= 0;

	[ ReorderableList( new int[]{ 50, 50, 50, 50, 50 }), HeaderAttribute ("体力ー火力ー射程[radius]ー発射間隔[sec]ーレベルアップ費用")]
	public LevelUpParamReorderableList m_levelInformations = null;


    private DamageBank  m_rDamageBank       = null;

	void Start()
	{
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

    //  ダメージ処理
    void    DamageProc_CallBack( DamageResult _rDamageResult, CollisionInfo _rInfo )
    {
        //  サーバーでのみ処理を行う
        if( !NetworkServer.active ) return;

        //  ダメージを受ける
        GiveDamage( ( int )_rDamageResult.GetTotalDamage() );
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
	}
	public void GiveDamage( int damage )
	{
		m_curHp -= damage;
        m_curHp =  Mathf.Max( m_curHp, 0 );
        if( m_curHp <= 0 ){
            //  破壊されたら非アクティブ化
            gameObject.SetActive( false );
            //  クライアントでも非アクティブ化するようリクエストを飛ばす
            RpcSetActive( false );
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
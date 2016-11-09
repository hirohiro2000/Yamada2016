
using   UnityEngine;
using   UnityEngine.Networking;
using   System.Collections;

[ RequireComponent( typeof( DamageBank ) ) ]
public class TPSPlayer_HP : NetworkBehaviour {

    [ SyncVar ]
	public  float               m_MaxHP         =   10.0f;
    [ SyncVar ]
	public  float               m_CurHP         =   0.0f;

    private DamageBank          m_rDamageBank   =   null;
    private TPSPlayer_Control   m_rTPSControl   =   null;
    private NetPlayer_Control   m_rNetPlayer    =   null;

	//  Use this for initialization
	void    Start()
    {
        //  アクセスの取得
	    m_rDamageBank   =   GetComponent< DamageBank >();
        m_rTPSControl   =   GetComponent< TPSPlayer_Control >();
        m_rNetPlayer    =   GetComponent< NetPlayer_Control >();

        //  パラメータ初期化
        m_CurHP         =   m_MaxHP;

        //  ローカルでのみ処理を行う
        if( isLocalPlayer ){
            //  ダメージ処理をセット
		    m_rDamageBank.DamagedCallback   +=  DamageProc_CallBack;

            //  ＨＰバー初期化
            TPSHpBar.Initialize( m_MaxHP );
        }
	}
	void    DamageProc_CallBack( float _Damage )
    {
        //  受けたダメージをサーバーに送信
        m_rTPSControl.CmdSendDamage( _Damage );
    }

	//  Update is called once per frame
	void    Update()
    {
	    //  ＨＰバー更新
        if( isLocalPlayer ){
            TPSHpBar.SetHP( m_CurHP );
        }
	}

    //  外部からの操作
    public  void    SetDamage( float _Damage )
    {
        //  ダメージを受ける
        m_CurHP -=  _Damage;
        m_CurHP =   Mathf.Max( m_CurHP, 0.0f );
        //  死亡チェック
        if( m_CurHP == 0.0f ){
            //  コマンダーに戻る
            m_rTPSControl.RpcChangeToCommander( m_rNetPlayer.c_ClientID );
        }
    }
}

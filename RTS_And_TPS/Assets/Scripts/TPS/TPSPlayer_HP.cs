
using   UnityEngine;
using   UnityEngine.Networking;
using   System.Collections;

[ RequireComponent( typeof( DamageBank ) ) ]
public class TPSPlayer_HP : NetworkBehaviour {

    [ SyncVar ]
	public  float                   m_MaxHP         =   10.0f;
    [ SyncVar ]
	public  float                   m_CurHP         =   0.0f;

    private DamageBank              m_rDamageBank   =   null;
    private TPSPlayer_Control       m_rTPSControl   =   null;
    private RTSPlayer_Control       m_rRTSControl   =   null;
    private NetPlayer_Control       m_rNetPlayer    =   null;

    //  落下ダメージ用パラメータ
    private float                   c_DamageHeight  =   10.0f;
    private float                   c_HDRatio       =   1.0f;

    private CharacterController     m_rCharaCtrl    =   null;
    private bool                    m_PrevGrounded  =   false;
    private float                   m_LastHeight    =   0.0f;

    //  外部へのアクセス
    private DamageFilter_Control    m_rDFCtrl       =   null;

	//  Use this for initialization
	void    Start()
    {
        //  外部へのアクセス
        m_rDFCtrl       =   FunctionManager.GetAccessComponent< DamageFilter_Control >( "Canvas/DamageFilter" );

        //  アクセスの取得
	    m_rDamageBank   =   GetComponent< DamageBank >();
        m_rCharaCtrl    =   GetComponent< CharacterController >();
        m_rTPSControl   =   GetComponent< TPSPlayer_Control >();
        m_rRTSControl   =   GetComponent< RTSPlayer_Control >();
        m_rNetPlayer    =   GetComponent< NetPlayer_Control >();

        //  パラメータ初期化
        m_CurHP         =   m_MaxHP;

        //  ローカルでのみ処理を行う
        if( isLocalPlayer ){
            //  ダメージ処理をセット
		    m_rDamageBank.AdvancedDamagedCallback   +=  DamageProc_CallBack;

            //  ＨＰバー初期化
            TPSHpBar.Initialize( m_MaxHP );
        }
	}
	void    DamageProc_CallBack( DamageResult _rDamageResult, CollisionInfo _rInfo )
    {
        //  受けたダメージ
        float   damage  =   _rDamageResult.GetTotalDamage();

        //  ダメージがなければ終了
        if( damage <= 0.0f )    return;

        //  受けたダメージをサーバーに送信
        if( m_rRTSControl ) m_rRTSControl.CmdSendDamage( damage );
        if( m_rTPSControl ) m_rTPSControl.CmdSendDamage( damage );

        //  カメラをシェイク
        {
            Shaker_Control  rShaker     =   Camera.main.GetComponent< Shaker_Control >();
            if( rShaker ){
                Vector3     atkPos      =   ( _rInfo.attackedObject )? _rInfo.attackedObject.position : Vector3.zero;
                Vector3     dfePos      =   ( _rInfo.damagedObject  )? _rInfo.damagedObject.position  : Vector3.zero;

                Transform   rCamTrans   =   rShaker.transform;
                Vector3     vShake      =   ( dfePos - atkPos ).normalized;
                            vShake      =   rCamTrans.InverseTransformDirection( vShake );
                float       shakePower  =   0.5f;
                            shakePower  =   Mathf.Min( shakePower, 1.0f );
                
                rShaker.SetShake( vShake, 0.5f, 0.1f, shakePower );
            }
        }
        //  ダメージエフェクト
        if( m_rDFCtrl ){
            m_rDFCtrl.SetEffect( 0.6f, 1.0f );
        }
    }

	//  Update is called once per frame
	void    Update()
    {
	    //  ローカルプレイヤーのみ処理を行う
        if( !isLocalPlayer )    return;

        //  落下ダメージ処理
        FallDamageProc();

        //  ＨＰバー更新
        TPSHpBar.SetHP( m_CurHP );
	}
    void    FallDamageProc()
    {
        if( !m_rCharaCtrl ) return;
        
        //  落下判定
        if( m_rCharaCtrl.isGrounded
        &&  m_PrevGrounded == false ){
            //  落下ダメージ計算
            float   damage  =   Mathf.Max( 0, ( m_LastHeight - transform.position.y ) - c_DamageHeight );
                    damage  =   damage * c_HDRatio;

            //  ダメージチェック
            if( damage > 0.0f ){
                //  ダメージを送信
                if( m_rRTSControl ) m_rRTSControl.CmdSendDamage( damage );
                if( m_rTPSControl ) m_rTPSControl.CmdSendDamage( damage );
                //  ダメージエフェクト
                if( m_rDFCtrl )     m_rDFCtrl.SetEffect( 0.6f, 1.5f );
            }

            //  カメラシェイク
            if( damage > 0.0f ){
                Shaker_Control  rShaker     =   Camera.main.GetComponent< Shaker_Control >();
                if( rShaker ){
                    Transform   rCamTrans   =   rShaker.transform;
                    Vector3     vShake      =   -Vector3.up.normalized * damage;
                                vShake      =   rCamTrans.InverseTransformDirection( vShake );
                    float       shakePower  =   5 / vShake.magnitude;
                                shakePower  =   Mathf.Min( shakePower, 1.0f );
            
                    rShaker.SetShake( vShake.normalized, 0.5f, 0.2f, shakePower );
                }
            }
        }

        //  ダメージ用の高さを更新（滞空中は最大高度のみ更新）
        if( m_rCharaCtrl.isGrounded )   m_LastHeight    =   transform.position.y;
        else                            m_LastHeight    =   Mathf.Max( m_LastHeight, transform.position.y );
        //  設置判定を保存
        m_PrevGrounded      =   m_rCharaCtrl.isGrounded;
    }

    //  外部からの操作
    public  void    SetDamage( float _Damage )
    {
        //  ダメージを受ける 
        m_CurHP -=  _Damage;
        m_CurHP =   Mathf.Max( m_CurHP, 0.0f );
        m_CurHP =   Mathf.Min( m_CurHP, m_MaxHP );

        //  死亡チェック
        if( m_CurHP == 0.0f ){
            //  コマンダーに戻る
            if( m_rTPSControl ) m_rTPSControl.RpcChangeToCommander( m_rNetPlayer.c_ClientID );
            if( m_rRTSControl ) m_rRTSControl.RpcChangeToCommander( m_rNetPlayer.c_ClientID );
        }
    }
}

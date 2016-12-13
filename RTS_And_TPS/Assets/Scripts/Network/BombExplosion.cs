using UnityEngine;
using System.Collections;

public class BombExplosion : MonoBehaviour {
    public  int         c_AttackerID    =   0;
    public  GameObject  c_Explosion     =   null;
    public  bool        c_CallOnDestroy =   true;

    private GameManager m_rGameManager  =   null;
    private bool        m_IsGameQuit    =   false;

    void    Awake()
    {
        m_rGameManager  =   FunctionManager.GetAccessComponent< GameManager >( "GameManager" );
    }
    void    OnApplicationQuit()
    {
        m_IsGameQuit    =   true;
    }
    void    OnDestroy()
    {
        //  呼び出し設定
        if( !c_CallOnDestroy )  return;
        
        //  爆発処理
        Explosion();
    }
    public  void    Explosion()
    {
        //  ゲーム終了時は処理を行わない
        if( m_IsGameQuit )                                              return;
        //  ゲーム中以外は処理を行わない
        if( !m_rGameManager )                                           return;
        if( m_rGameManager.GetState() > GameManager.State.GameOver )    return;

        //  爆発オブジェクト生成 
        GameObject  rObj    =   Instantiate( c_Explosion );
        Transform   rTrans  =   rObj.transform;
        
        rTrans.position     =   transform.position;
        rTrans.rotation     =   transform.rotation;

        //  あたり判定へのアクセス
        Transform   rCol    =   rTrans.FindChild( "AttackArea" );
        //if( !rCol ) return;
        
        //  識別番号設定
        TPSAttack_Net   rAtkNet =   rCol.GetComponent< TPSAttack_Net >();
        rAtkNet.c_AttackerID    =   c_AttackerID;

        //  あたり判定のフレーム数を設定
        ExplosionAttack rEXATK  =   rCol.GetComponent< ExplosionAttack >();
        rEXATK.c_DestroyCounter =   1;

        //  オブジェクトをアクティベート
        rObj.SetActive( true );

        //  カメラシェイク 
        {
            Shaker_Control  rShaker     =   Camera.main.GetComponent< Shaker_Control >();
            if( rShaker ){
                Transform   rCamTrans   =   rShaker.transform;
                Vector3     vToCamera   =   ( Camera.main.transform.position - transform.position );
                Vector3     vShake      =   vToCamera.normalized;
                            vShake      =   rCamTrans.InverseTransformDirection( vShake );
                float       shakePower  =   20 / vToCamera.magnitude;
                            shakePower  =   Mathf.Min( shakePower, 1.0f );


                rShaker.SetShake( vShake, 1.0f, 0.25f, shakePower );
                rShaker.SetShake( Vector3.up, 1.5f, 0.25f, shakePower * 0.5f );
                rShaker.SetShake( Vector3.right, 2.5f, 0.25f, shakePower * 0.5f );
            }
        }
    }
}

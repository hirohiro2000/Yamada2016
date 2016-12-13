
using   UnityEngine;
using   UnityEngine.Networking;
using   System.Collections;

public class TPSCloneAnime_Control : NetworkBehaviour {

    private float                   c_AnimeSpeed    =   0.5f;
    private float                   c_AnimeRatio    =   0.73f;

    TPS_PlayerAnimationController   m_rAnimeControl =   null;
    Vector3                         m_PrevPos       =   Vector3.zero;
    float                           m_PrevYMove     =   0.0f;

	// Use this for initialization
	void    Start()
    {
	    m_rAnimeControl =   GetComponent< TPS_PlayerAnimationController >();

        //  本体側では処理を行わない
        if( isLocalPlayer ){
            this.enabled    =   false;
        }
	}
	
	// Update is called once per frame
	void    Update()
    {
	    //  クローンプレイヤーのみ処理を行う
        if( isLocalPlayer )     return;

        //  速度を計算
        Vector3 vMove       =   transform.position - m_PrevPos;
        float   moveSpeed   =   vMove.magnitude / Time.deltaTime;
        bool    isGround    =   Mathf.Abs( vMove.y - m_PrevYMove ) <= 0.005f;
        
        //  アニメーション
        if( moveSpeed > 0.1f ){
            float   axisX   =   Vector3.Dot( transform.right,   vMove );
            float   axisZ   =   Vector3.Dot( transform.forward, vMove );

            //  前後
            if( Mathf.Abs( axisZ ) > Mathf.Abs( axisX ) ){
                if( axisZ > 0.0f )  m_rAnimeControl.ChangeStateMove( TPS_PlayerAnimationController.InputDpad.eFORWARD );
                else                m_rAnimeControl.ChangeStateMove( TPS_PlayerAnimationController.InputDpad.eBACK );
            }
            //  左右
            else{
                if( axisX > 0.0f )  m_rAnimeControl.ChangeStateMove( TPS_PlayerAnimationController.InputDpad.eRIGHT );
                else                m_rAnimeControl.ChangeStateMove( TPS_PlayerAnimationController.InputDpad.eLEFT );
            }

            //  アニメーション速度
            m_rAnimeControl.ChangeSpeed( moveSpeed * c_AnimeRatio * c_AnimeSpeed );
        }
        //  停止中は待機
        else{
            m_rAnimeControl.ChangeStateIdle();
            m_rAnimeControl.ChangeSpeed( 1.0f );
        }

        //  浮いている間はアニメーションを再生しない
        if( !isGround){
            m_rAnimeControl.ChangeSpeed( 0.0f );
        }

        //  現在の座標を保存
        m_PrevPos   =   transform.position;
        m_PrevYMove =   vMove.y;
	}
}


using   UnityEngine;
using   UnityEngine.Networking;
using   System.Collections;

public class RTSCloneAnime_Control : NetworkBehaviour {

    private RTS_PlayerAnimationController
                        m_rAnimeControl     =   null;
    private Vector3     m_PrevPos           =   Vector3.zero;

	// Use this for initialization
	void    Start()
    {
	    m_rAnimeControl =   GetComponent< RTS_PlayerAnimationController >();

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
        float   moveSpeed   =   vMove.magnitude;

        //  アニメーション
        if( moveSpeed > 0.05f )  m_rAnimeControl.ChangeStateMove( moveSpeed / moveSpeed / Time.deltaTime );
        else                    m_rAnimeControl.ChangeStateIdle();

        //  現在の座標を保存
        m_PrevPos   =   transform.position;
	}
}

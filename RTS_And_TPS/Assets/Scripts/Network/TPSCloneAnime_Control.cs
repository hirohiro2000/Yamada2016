
using   UnityEngine;
using   UnityEngine.Networking;
using   System.Collections;

public class TPSCloneAnime_Control : NetworkBehaviour {

    private float           c_AnimeSpeed    =   0.5f;
    private float           c_AnimeRatio    =   0.73f;

    TPS_AnimationController m_rAnimeControl =   null;
    Vector3                 m_PrevPos       =   Vector3.zero;
    float                   m_PrevYMove     =   0.0f;

	// Use this for initialization
	void    Start()
    {
	    m_rAnimeControl =   GetComponent< TPS_AnimationController >();
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

        //  速度に応じてモーションを更新
        if( moveSpeed > 0.0f
        &&  isGround ){
            m_rAnimeControl.ChangeStateMove();
            m_rAnimeControl.ChangeSpeed( moveSpeed * c_AnimeRatio * c_AnimeSpeed );
        }
        //  ジャンプ中あるいは停止中は待機
        else{
            m_rAnimeControl.ChangeStateIdle();
            m_rAnimeControl.ChangeSpeed( 1.0f );
        }

        //  現在の座標を保存
        m_PrevPos   =   transform.position;
        m_PrevYMove =   vMove.y;
	}
}

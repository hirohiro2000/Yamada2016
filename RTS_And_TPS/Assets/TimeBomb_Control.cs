
using   UnityEngine;
using   UnityEngine.Networking;
using   System.Collections;

public class TimeBomb_Control : NetworkBehaviour {
    [ SyncVar ]
    public  int     c_OwnerID       =   0;

    public  float   c_CountTime     =   3.0f;
    public  bool    c_IsActive      =   true;
    private float   c_LightTime     =   0.1f;

    private float   m_CountTimer    =   0.0f;

    private Light   m_rLight        =   null;

    void    Start()
    {
        m_rLight    =   transform.FindChild( "_Light" ).GetComponent< Light >();
    }
	void    Update()
    {
        //  非アクティブ状態では処理を行わない
        if( !c_IsActive )           return;

        //  共通処理
        {
            //  カウントダウン
            m_CountTimer    +=  Time.deltaTime;
            m_CountTimer    =   Mathf.Min( m_CountTimer, c_CountTime );

            //  ライトの更新
            float   timeInSecond    =   m_CountTimer - ( int )m_CountTimer;
            m_rLight.enabled        =   timeInSecond <= c_LightTime
                                    ||  m_CountTimer >= c_CountTime - 1.0f;
        }

	    //  サーバーでのみ更新処理を行う
        if( NetworkServer.active ){
            //  カウントダウンが終了したら消滅
            if( m_CountTimer >= c_CountTime ){
                //  オーナー設定
                GetComponent< DetonationObject >().m_DestroyerID    =   c_OwnerID;

                //  爆破
                Destroy( gameObject );
                return;
            }
        }
	}
}


using   UnityEngine;
using   UnityEngine.Networking;
using   System.Collections;

public class TimeBomb_Control : NetworkBehaviour {
    [ SyncVar ]
    public  int     c_OwnerID       =   0;

    public  float   c_CountTime     =   3.0f;
    public  bool    c_IsActive      =   true;

    private float   m_CountTimer    =   0.0f;

	void    Update()
    {
	    //  サーバーでのみ処理を行う
        if( !NetworkServer.active ) return;
        //  非アクティブ状態では処理を行わない
        if( !c_IsActive )           return;

        //  カウントダウンが終了したら消滅
        m_CountTimer    +=  Time.deltaTime;
        m_CountTimer    =   Mathf.Min( m_CountTimer, c_CountTime );
        if( m_CountTimer >= c_CountTime ){
            //  オーナー設定
            GetComponent< DetonationObject >().m_DestroyerID    =   c_OwnerID;

            //  爆破
            Destroy( gameObject );
        }
	}
}

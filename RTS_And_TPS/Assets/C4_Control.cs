
using   UnityEngine;
using   UnityEngine.Networking;
using   System.Collections;

public class C4_Control : NetworkBehaviour {
    [ SyncVar ]
    public  int     c_OwnerID       =   0;

    private float   c_Delay         =   0.2f;
    private float   m_DelayTimer    =   0.0f;
    private bool    m_IsActive      =   false;

	// Use this for initialization
	void    Start()
    {
	    
	}
	
	// Update is called once per frame
	void    Update()
    {
        //  サーバーでの処理
	    if( NetworkServer.active ){
            if( m_IsActive ){
                m_DelayTimer    +=  Time.deltaTime;
                m_DelayTimer    =   Mathf.Min( m_DelayTimer, c_Delay );
                if( m_DelayTimer >= c_Delay ){
                    //  爆発
                    Exploding();
                }
            }
        }
	}

    void    OnCollisionEnter( Collision _rCollision )
    {
        if( _rCollision.gameObject.layer == LayerMask.NameToLayer( "Field" ) ){
            GetComponent< Rigidbody >().isKinematic =   true;
        }
    }

    //  爆発
    [ Server ]
    void    Exploding()
    {
        //  オーナー設定
        GetComponent< DetonationObject >().m_DestroyerID    =   c_OwnerID;

        //  爆破
        Destroy( gameObject );
    }
    //  起爆
    [ Server ]
    public  void    SetExploding()
    {
        m_IsActive  =   true;
    }
}


using   UnityEngine;
using   UnityEngine.Networking;
using   System.Collections;

public class C4_Control : NetworkBehaviour {

    public  Color   c_ActiveColor   =   Color.red;

    [ SyncVar ]
    public  int     c_OwnerID       =   0;
    [ SyncVar ]
    private bool    m_IsActive      =   false;

    [ SyncVar ]
    public  Vector3 m_StartForce    =   Vector3.zero;

    private float   c_Delay         =   0.4f;
    private float   m_DelayTimer    =   0.0f;

    private Light   m_rLight        =   null;

	// Use this for initialization
	void    Start()
    {
	    m_rLight    =   transform.FindChild( "_Light" ).GetComponent< Light >();

        //  初速設定
        {
            Rigidbody   rRigid  =   GetComponent< Rigidbody >();
            if( rRigid ){
                rRigid.AddForce( m_StartForce, ForceMode.Impulse );
            }
        }

        //  オブジェクトをリストに登録
        C4Shell_Control rShell  =   FunctionManager.GetAccessComponent< C4Shell_Control >( "C4_Shell" );
        if( rShell ){
            rShell.m_rC4List.Add( gameObject );
        }
	}
	
	// Update is called once per frame
	void    Update()
    {
        //  共通処理
        if( m_IsActive ){
            Color   prevColor   =   m_rLight.color;
            m_rLight.color  =   c_ActiveColor;

            //  効果音を鳴らす
            if( prevColor != m_rLight.color ){
                SoundController.PlayNow( "TB_CountDown_End", transform, transform.position, 0.0f, 0.5f, 1.0f, 12.0f );
            }
        }

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
        Transform   rTarget =   null;

        //  接触したのがステージだった場合、大元のオブジェクトにひっつける
        if( _rCollision.gameObject.layer == LayerMask.NameToLayer( "Field" ) ){
            DamageBank      rDBank  =   _rCollision.gameObject.GetComponentInParent< DamageBank >();
            if( rDBank )    rTarget =   rDBank.transform;
        }
        
        //  接触したオブジェクトにくっつく
        if( rTarget )   transform.SetParent( rTarget );
        else            transform.SetParent( _rCollision.transform );

        //  停止
        GetComponent< Rigidbody >().isKinematic =   true;
        //GetComponent< Collider >().enabled      =   false;
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

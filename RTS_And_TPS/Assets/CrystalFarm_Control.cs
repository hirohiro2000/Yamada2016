
using   UnityEngine;
using   UnityEngine.Networking;
using   System.Collections;

public class CrystalFarm_Control : NetworkBehaviour {

    public  GameObject              c_Crystal       =   null;

    [ SyncVar ]
    private int                     m_Energy        =   3;
    private bool                    m_StandbyOK     =   false;

    private Transform               m_rEnergy       =   null;
    private RTSResourece_Control    m_rRTSRControl  =   null;

	// Use this for initialization
	void    Start()
    {
	    m_rEnergy       =   transform.FindChild( "_Energy" );
        m_rRTSRControl  =   GetComponent< RTSResourece_Control >();
	}
    void    OnEnable()
    {
        if( !m_rRTSRControl )                   return;
        if( m_rRTSRControl.c_OwnerID == -1 )    return;
        if( !NetworkServer.active )             return;
        if( !m_StandbyOK )                      return;

        //  ウェーブの境目にクリスタルを生成
        GenerateCrystal();
    }
    void    OnDisable()
    {
        m_StandbyOK =   true;
    }
	
	// Update is called once per frame
	void    Update()
    {
	    if( NetworkServer.active ){
            if( Input.GetKeyDown( KeyCode.K ) ){
                GenerateCrystal();
            }
        }

        //  エネルギー表示更新
        UpdateEnergyMesh( m_Energy );
	}


    void    UpdateEnergyMesh( int _Energy )
    {
        for( int i = 0; i < 3; i++ ){
            m_rEnergy.GetChild( i ).gameObject.SetActive( _Energy > i );
        }
    }

    //  クリスタル生成
    [ Server ]
    public  void    GenerateCrystal()
    {
        //  クリスタル生成 
        {
            //  オブジェクトを生成
            GameObject  rObj    =   Instantiate( c_Crystal );
            Transform   rTrans  =   rObj.transform;

            //  配置設定
            rTrans.position     =   transform.position + Vector3.up * 2.0f;

            //  パラメータ設定 
            ResourceObject_Control  rControl    =   rObj.GetComponent< ResourceObject_Control >();
            rControl.m_Resource =   25 + 25 * ( 3 - m_Energy );
            rControl.m_Resource *=  ( GetComponent< ResourceParameter >().m_level + 1 );

            //  ネットワークで共有
            NetworkServer.Spawn( rObj );
        }

        //  エネルギー消費
        m_Energy    =   Mathf.Max( m_Energy - 1, 0 );
        //  エネルギーがなくなったら消滅
        if( m_Energy == 0 ){
            Destroy( gameObject );
        }
    }
}

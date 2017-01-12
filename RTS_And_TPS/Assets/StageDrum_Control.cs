
using   UnityEngine;
using   UnityEngine.Networking;
using   System.Collections;
using   System.Collections.Generic;

public class StageDrum_Control : MonoBehaviour {

    public  GameObject          c_Drum          =   null;
    public  List< GameObject >  m_rDrumList     =   new List< GameObject >();
    private Transform           m_rSDSource     =   null;

	// Use this for initialization
	void    Start()
    {
        //  見本へのアクセス
        m_rSDSource =   GameObject.Find( "Drum_Shell" ).transform.FindChild( "Stage_Drum" );

        //  ドラムをネットワークで共有
	    for( int i = 0; i < transform.childCount; i++ ){
            //  アクセスの取得
            Transform   rSourceTrans    =   transform.GetChild( i );

            //  オブジェクト生成
            GameObject  rObj            =   Instantiate( c_Drum );
            Transform   rTrans          =   rObj.transform;

            //  配置設定
            rTrans.localPosition        =   rSourceTrans.localPosition;
            rTrans.localRotation        =   rSourceTrans.localRotation;
            rTrans.localScale           =   rSourceTrans.localScale;

            //  ネットワークで共有
            NetworkServer.Spawn( rObj );

            //  リストに登録
            m_rDrumList.Add( rObj );
        }

        //  子オブジェクトを削除
        for( int i = 0; i < transform.childCount; i++ ){
            Destroy( transform.GetChild( i ).gameObject );
        }
	}
	
    //  アクセス
    public  void    EraseAll()
    {
        //  すべてのステージドラムを削除
        for( int i = 0; i < m_rDrumList.Count; i++ ){
            if( !m_rDrumList[ i ] ) continue;

            m_rDrumList[ i ].GetComponent< DetonationObject >().m_DestroyerID   =   -1;

            //  オブジェクトを破棄 
            Destroy( m_rDrumList[ i ], 1.0f );
        }
    }
    public  void    Supple()
    {
        for( int i = 0; i < m_rDrumList.Count; i++ ){
            //  以前配置したドラム缶がまだ残っている場合は処理しない
            if( m_rDrumList[ i ] )  continue;

            //  新たなドラム缶を生成
            GameObject  rObj            =   Instantiate( c_Drum );
            Transform   rTrans          =   rObj.transform;

            //  配置設定
            {
                Transform   rSource     =   m_rSDSource.transform.GetChild( i );
                rTrans.localPosition    =   rSource.localPosition;
                rTrans.localRotation    =   rSource.localRotation;
                rTrans.localScale       =   rSource.localScale;
            }

            //  ネットワークで共有
            NetworkServer.Spawn( rObj );

            //  リストに登録
            m_rDrumList[ i ]    =   rObj;
        }
    }
}

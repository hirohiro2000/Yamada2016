using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UICompasCore : MonoBehaviour
{

    [SerializeField]
    private Texture  m_girl  = null;
    [SerializeField]
    private Texture  m_robot = null;

    GameObject   m_rPlayerOriginal = null;

    class DATA                                             
    {
        public GameObject reference;
        public GameObject dst;
    }
	private	List<DATA>	m_compasPlayers  = null;


    // アクセス
    private GameObject  m_rPlayer   = null;


    private static UICompasCore instance = null;

    void Awake()
    {
        instance = this;
        m_compasPlayers = new List<DATA>();
        m_rPlayerOriginal = transform.FindChild("Player_Original").gameObject;
    }
    void OnEnable()
    {
        instance = this;
        
        //  リストをクリア
        int loopCount   =   m_compasPlayers.Count;
        for( int i = 0; i < loopCount; i++ ){
            DATA    rData   =   m_compasPlayers[ 0 ];

            //  表示を削除
            Destroy( rData.dst );
            //  項目を削除
            m_compasPlayers.Remove( rData );
        }
    }

    void Update()
    {
        if ( m_rPlayer == null )  return;

        {
            GameObject[]    playerList  =   GameObject.FindGameObjectsWithTag( "Player" );
            for( int i = 0; i < playerList.Length; i++ ){

                if ( m_rPlayer == playerList[ i ] )  continue;
              
                if ( playerList[ i ].GetComponent<PlayerCommander_Control>() )  continue;

                //  リストに登録されているかチェック
                if( CheckWhetherRegistedInList( playerList[ i ] ) )  continue;

                //  登録されていない場合は追加
                AddOwnPlayer( playerList[ i ] );

            }


            //  無効になった項目を削除
            for( int i = 0; i < m_compasPlayers.Count; i++ ){
                DATA    rData   =   m_compasPlayers[ i ];
                if( rData.reference )   continue;

                //  表示を削除
                Destroy( rData.dst );
                //  項目を削除
                m_compasPlayers.Remove( rData );

                //  最初に戻る
                i   =   -1;
            }
        }

        foreach (var item in m_compasPlayers)
        {
            UICompassPosition compasPosition = item.dst.GetComponent<UICompassPosition>();
            compasPosition.m_rTargetPos = item.reference.transform.position;

            TPSPlayer_HP hp = item.reference.GetComponent<TPSPlayer_HP>();
            if ( hp == null )   continue;

            if ( hp.m_IsDying )
            {
                item.dst.GetComponent<RawImage>().color = Color.red;
            }
            else
            {
                item.dst.GetComponent<RawImage>().color = Color.white;
            }

            if ( item.reference.GetComponent<GirlController>() && item.reference.GetComponent<GirlController>().IsRide() )
            {
                item.dst.SetActive(false);
            }
            else
            {
                item.dst.SetActive(true);
            }

        }

    }

    //  リストに登録されているかどうかチェック
    bool    CheckWhetherRegistedInList( GameObject _rObj )
    {
        for( int i = 0; i < m_compasPlayers.Count; i++ ){
            if( m_compasPlayers[ i ].reference == _rObj )    return  true;
        }

        return  false;
    }

    static public void SetPlayer(GameObject player)
    {
        instance.m_rPlayer = player;
    }
    static public void AddOwnPlayer( GameObject src )
    {
        DATA data = new DATA();
        data.reference = src;
        data.dst = Instantiate(instance.m_rPlayerOriginal);
        data.dst.transform.SetParent( instance.transform );
        data.dst.transform.localScale = Vector3.one;
        data.dst.SetActive(true);

        UICompassPosition compasPosition = data.dst.GetComponent<UICompassPosition>();
        compasPosition.m_clipDistanceSq = 100000.0f;

        if ( src.GetComponent<TPSPlayer_Control>() )
        {
            data.dst.GetComponent<RawImage>().texture = instance.m_robot;
        }
        if ( src.GetComponent<RTSPlayer_Control>() )
        {
            data.dst.GetComponent<RawImage>().texture = instance.m_girl;
        }

        instance.m_compasPlayers.Add(data);
    }


}

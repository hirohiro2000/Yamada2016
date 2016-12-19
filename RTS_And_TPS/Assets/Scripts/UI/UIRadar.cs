using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class UIRadar : MonoBehaviour
{
    [SerializeField]
    private GameObject m_playerFighter  = null;
    [SerializeField]
    private GameObject m_enemyFighter   = null;
    [SerializeField]
    private GameObject m_backGround     = null;

    [SerializeField]
    private Texture     m_defaultIcon       = null;
    [SerializeField]
    private Texture     m_upperIcon         = null;
    [SerializeField]
    private Texture     m_lowerIcon         = null;

    [SerializeField]
    private Color      m_ownColor           = Color.blue;
    [SerializeField]
    private Color      m_enemyColor         = Color.red;
    [SerializeField]
    private Color      m_resourceColor      = Color.red;

    //  外部へのアクセス
    private GameObject          m_player            = null;
    private ReferenceWrapper    m_rEnemyShell       =   null;
    private GameObject          m_rResourceShell    =   null;
    
    class DATA
    {
        public GameObject reference;
        public GameObject dst;
    }
	private	List<DATA>	m_uiSymbolList  = null;

    private static UIRadar instance = null;

    void Awake()
    {
        instance = GetComponent<UIRadar>();

        m_uiSymbolList = new List<DATA>();

        //  アクセスを取得
        m_rEnemyShell       =   FunctionManager.GetAccessComponent< ReferenceWrapper >( "EnemySpawnRoot" );
		m_rResourceShell    = GameObject.Find("FieldResources");

    }
    void OnEnable()
    {
        instance = GetComponent<UIRadar>();
        
        //  リストをクリア
        int loopCount   =   m_uiSymbolList.Count;
        for( int i = 0; i < loopCount; i++ ){
            DATA    rData   =   m_uiSymbolList[ 0 ];

            //  表示を削除
            Destroy( rData.dst );
            //  項目を削除
            m_uiSymbolList.Remove( rData );
        }
    }

    public float devParam = 10.0f;
    void Update()
    {
        if( m_player == null )  return;

        //  アクセスの取得
        if( !m_rEnemyShell )        m_rEnemyShell   =   FunctionManager.GetAccessComponent< ReferenceWrapper >( "EnemySpawnRoot" );
        if( !m_rEnemyShell )        return;

        if( !m_rResourceShell )    m_rResourceShell    = GameObject.Find("FieldResources");
        if( !m_rResourceShell )    return;

        //  リストを更新
        {
            GameObject[]    playerList  =   GameObject.FindGameObjectsWithTag( "Player" );
            for( int i = 0; i < playerList.Length; i++ ){

                if ( m_player == playerList[ i ] )  continue;

                //  リストに登録されているかチェック
                if( CheckWhetherRegistedInList( playerList[ i ] ) )  continue;
                //  登録されていない場合は追加
                AddOwnObject( playerList[ i ] );

            }
            
            //  新しいアクセスを追加
            for( int i = 0; i < m_rEnemyShell.m_active_enemy_list.Count; i++ ){
                //  エネミーへのアクセス
                GameObject  rEnemy  =   m_rEnemyShell.m_active_enemy_list[ i ];

                //  リストに登録されているかチェック
                if( CheckWhetherRegistedInList( rEnemy ) )  continue;
                //  登録されていない場合は追加
                AddEnemy( rEnemy );
            }
            
            //  新しいアクセスを追加
            int loopCount = m_rResourceShell.transform.childCount;
            for( int i = 0; i < loopCount; i++ ){
                //  リソースへのアクセス
                GameObject  rResource =   m_rResourceShell.transform.GetChild( i ).gameObject;
                
                //  リストに登録されているかチェック
                if( CheckWhetherRegistedInList( rResource ) )  continue;
                //  登録されていない場合は追加
                AddResource( rResource );
            }

            //  無効になった項目を削除
            for( int i = 0; i < m_uiSymbolList.Count; i++ ){
                DATA    rData   =   m_uiSymbolList[ i ];
                if( rData.reference )   continue;

                //  表示を削除
                Destroy( rData.dst );
                //  項目を削除
                m_uiSymbolList.Remove( rData );

                //  最初に戻る
                i   =   -1;
            }
        }


        //  表示を更新
        foreach (var item in m_uiSymbolList)
        {
            item.dst.transform.SetParent(transform);

            RectTransform rt = item.dst.GetComponent<RectTransform>();

            // 位置の更新
            float searchRange   = 76.0f;
            float maxLength     = 115.0f;

            Vector3 rtPosition = ( m_player.transform.position - item.reference.transform.position ) / searchRange;
            rt.localPosition = new Vector3(rtPosition.x, rtPosition.z, 0.0f) * maxLength;

            // clamp
            rt.localPosition = rt.localPosition.normalized * ( Mathf.Min(rt.localPosition.magnitude, maxLength) );
            
             
            // アイコンの更新
            float   relativeHeight  =   item.reference.transform.position.y
                                    -   m_player.transform.position.y;
            float   heightFactor    =   5.0f;
                    if( relativeHeight > heightFactor ) item.dst.GetComponent< RawImage >().texture =   m_upperIcon;
            else    if( relativeHeight < -heightFactor) item.dst.GetComponent< RawImage >().texture =   m_lowerIcon;
            else                                        item.dst.GetComponent< RawImage >().texture =   m_defaultIcon;

        }

        { 
            RectTransform rt = m_playerFighter.GetComponent<RectTransform>();
            rt.eulerAngles = new Vector3(rt.eulerAngles.x, rt.eulerAngles.y, -m_player.transform.eulerAngles.y );
        }
        {
            Vector3 position = m_player.transform.position / 76.0f;
            RectTransform rt = m_backGround.GetComponent<RectTransform>();  
            rt.localPosition = new Vector3(position.x, position.z, 0.0f) * 145.0f;
        }

    }

    //  リストに登録されているかどうかチェック
    bool    CheckWhetherRegistedInList( GameObject _rObj )
    {
        for( int i = 0; i < m_uiSymbolList.Count; i++ ){
            if( m_uiSymbolList[ i ].reference == _rObj )    return  true;
        }

        return  false;
    }

    static public void SetPlayer(GameObject player)
    {
        instance.m_player = player;
    }

    static public void Add(GameObject src, Color rgba)
    {
        DATA data = new DATA();
        data.reference = src;
        data.dst = Instantiate(instance.m_enemyFighter);
        data.dst.transform.SetParent( instance.transform );
        data.dst.transform.localScale = Vector3.one;
        data.dst.GetComponent<RawImage>().color = rgba;
        data.dst.SetActive(true);

        instance.m_uiSymbolList.Add(data);
    }
    static public void Remove(GameObject obj)
    {
        //if( !instance )                                 return;
        //if( !instance.gameObject.activeInHierarchy )    return;

        int numSymbols = instance.m_uiSymbolList.Count;
        for (int i = numSymbols - 1; i >= 0; --i)
        {
            DATA item = instance.m_uiSymbolList[i];
            if (item.reference.Equals(obj))
            {
                Destroy(item.dst);
                instance.m_uiSymbolList.RemoveAt(i);
                break;
            }
        }

    }

    static public void AddOwnObject(GameObject src)
    {
        Add(src, instance.m_ownColor);
    }
    static public void AddEnemy(GameObject src)
    {
        Add(src, instance.m_enemyColor);
    }
    static public void AddResource(GameObject src)
    {
        Add(src, instance.m_resourceColor);
    }



}

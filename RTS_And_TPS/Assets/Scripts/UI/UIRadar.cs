using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class UIRadar : MonoBehaviour {

    [SerializeField]
    private GameObject m_player         = null;
    //[SerializeField]
    //private GameObject m_ownFighter      = null;
    [SerializeField]
    private GameObject m_enemyFighter    = null;
    //[SerializeField]
    //private GameObject m_backGround    = null;

    [SerializeField]
    private float     m_searchRange      = 50.0f;

    [SerializeField]
    private Color      m_ownColor       = Color.blue;
    [SerializeField]
    private Color      m_enemyColor      = Color.red;

    //  外部へのアクセス
    private ReferenceWrapper    m_rEnemyShell   =   null;
    

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
        m_rEnemyShell   =   FunctionManager.GetAccessComponent< ReferenceWrapper >( "EnemySpawnRoot" );
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

    void Update()
    {
        if( m_player == null )  return;

        //  アクセスの取得
        if( !m_rEnemyShell )    m_rEnemyShell   =   FunctionManager.GetAccessComponent< ReferenceWrapper >( "EnemySpawnRoot" );
        if( !m_rEnemyShell )    return;

        //  リストを更新
        {
            //  新しいアクセスを追加
            for( int i = 0; i < m_rEnemyShell.m_active_enemy_list.Count; i++ ){
                //  エネミーへのアクセス
                GameObject  rEnemy  =   m_rEnemyShell.m_active_enemy_list[ i ];

                //  リストに登録されているかチェック
                if( CheckWhetherRegistedInList( rEnemy ) )  continue;
                //  登録されていない場合は追加
                AddEnemy( rEnemy );
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
        Matrix4x4 worldToLocalMatrix = Camera.main.transform.worldToLocalMatrix;//m_player.transform.worldToLocalMatrix;
        foreach (var item in m_uiSymbolList)
        {
            RectTransform rt = item.dst.GetComponent<RectTransform>();

            Vector3 relativePosition = worldToLocalMatrix.MultiplyPoint(item.reference.transform.position);

            item.dst.transform.SetParent( transform );

            Vector3 rtPosition   = relativePosition / m_searchRange;
            float   maxLength    = 70.0f;

            Vector3 enemyIconPos = new Vector3(rtPosition.x, rtPosition.z, 0.0f) * maxLength;

            rt.localPosition = enemyIconPos.normalized * ( Mathf.Min( enemyIconPos.magnitude, maxLength ) );

        }
        //{
        //    //RectTransform rt = m_backGround.GetComponent<RectTransform>();  
        //    //rt.eulerAngles = new Vector3(rt.eulerAngles.x, rt.eulerAngles.y, m_player.transform.eulerAngles.y);
        //}

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
        //if( !instance )                                 return;
        //if( !instance.gameObject.activeInHierarchy )    return;
        

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



}


using UnityEngine;
using System.Collections;

public class GridPanel_Control : MonoBehaviour {

    public  enum    GridType{
        None,           //  何もない
        Wall,           //  壁
        Slope,          //  斜面
        SpawnPoint,     //  敵の発生地点
        LaunchPoint,    //  出撃地点
        Base,           //  拠点
        Empty,          //  空
    }

    //  関連オブジェクトへのアクセス
    public  GameObject          m_rRelatedObj   =   null;

    //  上下へのアクセス
    public  GridPanel_Control   m_rUP           =   null;
    public  GridPanel_Control   m_rDOWN         =   null;

    //  公開パラメータ
    public  GridType            m_GridType      =   GridType.None;
    public  Vector3             m_GridPos       =   Vector3.zero;   //  座標（ 空間 ）
    public  Vector3             m_GridPoint     =   Vector3.zero;   //  座標（ マス目 ）
    public  bool                m_IsNavMesh     =   false;
    public  bool                m_IsActive      =   true;
    public  bool                m_IsStorage     =   false;

    //  内部パラメータ
    private string              m_DefaultStr    =   "";
    private string              m_SubStr        =   "";
    private bool                m_UseSubStr     =   false;
    
    //  内部アクセス
    private Renderer            m_rRenderer     =   null;
    private Collider            m_rCollider     =   null;
    private Renderer            m_rHeightText   =   null;
    private TextMesh            m_rHeightTextM  =   null;

    //  外部へのアクセス
    private EditManager         m_rEditManager  =   null;

	//	
	private	Color[]				c_HeightColor	=	new Color[]{
		Color.red,
		Color.blue,
		Color.green,
	};
	private	Renderer			m_rHeightMesh	= null;

    //  コピー用
    public  void    CopyParam( GridPanel_Control _rSource )
    {
        m_rRelatedObj   =   _rSource.m_rRelatedObj;
        
        m_GridType      =   _rSource.m_GridType;
        m_GridPos       =   _rSource.m_GridPos;
        m_GridPoint     =   _rSource.m_GridPoint;
        m_IsNavMesh     =   _rSource.m_IsNavMesh;
        m_IsActive      =   _rSource.m_IsActive;
    }

	// Use this for initialization
	void    Awake()
    {
	    m_rRenderer     =   transform.FindChild( "_Mesh" ).GetComponent< Renderer >();
        m_rCollider     =   GetComponent< Collider >();
        m_rHeightText   =   transform.FindChild( "_HeightText" ).GetComponent< Renderer >();
        m_rHeightTextM  =   m_rHeightText.GetComponent< TextMesh >();
		m_rHeightMesh	=	transform.FindChild( "_HeightMesh" ).GetComponent< Renderer >();

		m_rEditManager  =   FunctionManager.GetAccessComponent< EditManager >( "EditManager" );
	}
    void    Start()
    {
        m_DefaultStr        =   ( m_GridPoint.z > 0 )? ( ( int )m_GridPoint.z ).ToString() : "";
        m_SubStr            =   ( ( int )m_GridPoint.z + 1 ).ToString();

        m_rHeightTextM.text =   m_DefaultStr;
        m_UseSubStr         =   false;
    }
	
	// Update is called once per frame
	void    Update()
    {
		if ( !m_IsActive )   return;

        //  数字を更新
        UpdateTextMesh();

        //  空の場合は下へ
        if( m_rRelatedObj == null ){
            m_GridType  =   GridType.None;

            //  無効化
            SetActive_Recursion( false );
        }
        //  埋まっている場合上へ
        else
        if( m_rRelatedObj ){
            SetActive_Recursion( true );
        }
	}

    //  テキスト更新
    void    UpdateTextMesh()
    {
        bool    isMaxHeight =   CheckIsMaxHeight();
        if( isMaxHeight != m_UseSubStr ){
            m_rHeightTextM.text =   ( isMaxHeight )? m_SubStr : m_DefaultStr;
            m_UseSubStr         =   isMaxHeight;
        }

		int		height			=	( int )m_GridPoint.z;
		m_rHeightMesh.material.color	=
			c_HeightColor[ ( height + c_HeightColor.Length - 1 ) % c_HeightColor.Length ];

		m_rHeightMesh.enabled	= false;
    }
    bool    CheckIsMaxHeight()
    {
        if( m_rUP )             return  false;
        if( !m_rRelatedObj )    return  false;

        return  true;
    }

    //  上下方向へ再帰
    public  void    SetActive_Recursion( bool _IsActive )
    {
        //  有効化
        if( _IsActive ){
            //  有効化して終了（最上層、または最前面）
            if( m_rUP == null || m_rRelatedObj == null ){
                //  パラメータ有効化
                SetActive_Param( true );
            }
            //  マスが埋まっている場合、最上層でなければ上へ
            else{
                //  パラメータ無効化
                SetActive_Param( false );

                //  上へ
                m_rUP.SetActive_Recursion( true );
            }
        }
        //  無効化
        else{
            //  最下層、または下のグリッドが埋まっている場合は最前面なので有効化して終了
            if( m_rDOWN == null || m_rDOWN.m_rRelatedObj ){
                //  パラメータ有効化
                SetActive_Param( true );
            }
            //  最下層ではない場合、下が空なら降りる
            else{
                //  パラメータ無効化
                SetActive_Param( false );

                //  最前面まで降りる
                m_rDOWN.SetActive_Recursion( false );
            }
        }
    }
    public  void    SetActive_Param( bool _IsActive )
    {
        m_rCollider.enabled     =   _IsActive;
        m_rRenderer.enabled     =   _IsActive;
        m_rHeightText.enabled   =   _IsActive;
        m_IsActive              =   _IsActive;

        m_rHeightMesh.enabled   =   _IsActive && CheckUseHeightColor() && m_rEditManager.m_UseHeightColor;
    }
    private bool    CheckUseHeightColor()
    {
        if( m_GridPoint.z == 0 )                            return  false;
        if( !m_rDOWN )                                      return  false;
        if( !m_rDOWN.m_rRelatedObj )                        return  false;

        GameObject  rDownObj    =   m_rDOWN.m_rRelatedObj;
        Transform   rDownParent =   rDownObj.transform.parent;

        if( rDownParent.name.IndexOf( "Base" )    != -1 )   return  false;
        if( rDownParent.name.IndexOf( "Spawn" )   != -1 )   return  false;
        if( rDownParent.name.IndexOf( "Launch" )  != -1 )   return  false;
        if( rDownParent.name.IndexOf( "Empty" )   != -1 )   return  false;

        return  true;
    }

    public  void    SetColor( Color _Color )
    {
        m_rRenderer.material.color  =   _Color;
    }

    public  void    SetRelatedObject( GameObject _rObj )
    {
        if( m_rRelatedObj ){
            Destroy( m_rRelatedObj );
        }

        m_rRelatedObj   =   _rObj;
    }
}

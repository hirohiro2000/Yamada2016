using   UnityEngine;
using   UnityEngine.Networking;
using   System.Collections;
using   System.Collections.Generic;

public class FunctionManager : MonoBehaviour {

//******************************************************************************************************
//      パラメーター
//******************************************************************************************************
    //  公開
    public  static  int         c_BezieBufferMax    =   256;

    //  非公開
    private static  float[,]    s_BezieBuffer       =   null;
//******************************************************************************************************
//      メイン関数
//******************************************************************************************************
	// Use this for initialization
	void    Awake()
    {
	    //  ベジェ曲線計算用バッファ初期化
        s_BezieBuffer   =   new float[ c_BezieBufferMax, c_BezieBufferMax - 1 ];
	}
	
	// Update is called once per frame
	void    Update()
    {
	    
	}
//******************************************************************************************************
//
//      グローバル関数
//
//******************************************************************************************************
    //      ベジェ曲線計算
    //**************************************************************************************************
    public  static  float   CalcBezie( float[] _Points, float _Time )
    {
        //  バッファの準備が終わるまでは計算しない
        if( s_BezieBuffer == null )                     return  0.0f;

        //	例外に対処
		if( _Points.Length  <   1 )                     return  0.0f;
		if( _Points.Length  ==  1 )	                    return  _Points[0];
        if( _Points.Length  >   c_BezieBufferMax - 1 )  return  0.0f;

        //  計算結果
        float   result  =   0.0f;

        //	引数のデータをコピー
		for( int i = 0; i < _Points.Length; i++ ){
			s_BezieBuffer[ 0, i ]   =   _Points[i];
		}

		//	全項目の計算
		for( int i = 1; i < _Points.Length; i++ ){
			int     numItem     =   _Points.Length - i;
			for( int k = 0; k < numItem; k++ ){
				s_BezieBuffer[ i, k ]	=	s_BezieBuffer[ i - 1, k + 0 ] + ( s_BezieBuffer[ i - 1, k + 1 ] - s_BezieBuffer[ i - 1, k + 0 ] ) * _Time;
			}
		}

		//	結果
		result		=	s_BezieBuffer[ _Points.Length - 1, 0 ];

        //  結果を返す
        return  result;
    }
    //**************************************************************************************************
    //      矩形計算（CanvasのRectTransformからOnGUIへの変換用）
    //**************************************************************************************************
    public  enum    AR_TYPE_X{  LEFT,   CENTER,     RIGHT,    }
    public  enum    AR_TYPE_Y{  TOP,    MIDDLE,     BOTTOM,   }
    public  enum    AR_TYPE{
        TOP_CENTER,
        TOP_LEFT,
        TOP_RIGHT,
        MIDDLE_CENTER,
        MIDDLE_LEFT,
        MIDDLE_RIGHT,
        BOTTOM_CENTER,
        BOTTOM_LEFT,
        BOTTOM_RIGHT,
    }
    public  static  Rect    AdjustRectCanvasToGUI( AR_TYPE _Type, Rect _Rect )
    {
        return  AdjustRectCanvasToGUI( _Type, _Rect, new Vector2( 0.5f, 0.5f ), new Vector2( Screen.width, Screen.height ) );
    }
    public  static  Rect    AdjustRectCanvasToGUI( AR_TYPE _Type, Rect _Rect, Vector2 _Pivot )
    {
        return  AdjustRectCanvasToGUI( _Type, _Rect, _Pivot, new Vector2( Screen.width, Screen.height ) );
    }
    public  static  Rect    AdjustRectCanvasToGUI( AR_TYPE _Type, Rect _Rect, Vector2 _Pivot, Vector2 _ParentSize )
    {
        switch( _Type ){
            case    AR_TYPE.TOP_CENTER:
                _Rect.x =   _Rect.x - _Rect.width * _Pivot.x + _ParentSize.x * 0.5f;
                _Rect.y =   _ParentSize.y * 0.5f -_Rect.height * ( 0.5f - ( _Pivot.y - 0.5f ) ) - _ParentSize.y * 0.5f - _Rect.y;
                break;
            case    AR_TYPE.TOP_LEFT:
                _Rect.x =   _Rect.x - _Rect.width * _Pivot.x + _ParentSize.x * 0.0f;
                _Rect.y =   _ParentSize.y * 0.5f -_Rect.height * ( 0.5f - ( _Pivot.y - 0.5f ) ) - _ParentSize.y * 0.5f - _Rect.y;
                break;
            case    AR_TYPE.TOP_RIGHT:
                _Rect.x =   _Rect.x - _Rect.width * _Pivot.x + _ParentSize.x * 1.0f;
                _Rect.y =   _ParentSize.y * 0.5f -_Rect.height * ( 0.5f - ( _Pivot.y - 0.5f ) ) - _ParentSize.y * 0.5f - _Rect.y;
                break;
            case    AR_TYPE.MIDDLE_CENTER:
                _Rect.x =   _Rect.x - _Rect.width * _Pivot.x + _ParentSize.x * 0.5f;
                _Rect.y =   _ParentSize.y * 0.5f -_Rect.height * ( 0.5f - ( _Pivot.y - 0.5f ) ) + _ParentSize.y * 0.0f - _Rect.y;
                break;
            case    AR_TYPE.MIDDLE_LEFT:
                _Rect.x =   _Rect.x - _Rect.width * _Pivot.x + _ParentSize.x * 0.0f;
                _Rect.y =   _ParentSize.y * 0.5f -_Rect.height * ( 0.5f - ( _Pivot.y - 0.5f ) ) + _ParentSize.y * 0.0f - _Rect.y;
                break;
            case    AR_TYPE.MIDDLE_RIGHT:
                _Rect.x =   _Rect.x - _Rect.width * _Pivot.x + _ParentSize.x * 1.0f;
                _Rect.y =   _ParentSize.y * 0.5f -_Rect.height * ( 0.5f - ( _Pivot.y - 0.5f ) ) + _ParentSize.y * 0.0f - _Rect.y;
                break;
            case    AR_TYPE.BOTTOM_CENTER:
                _Rect.x =   _Rect.x - _Rect.width * _Pivot.x + _ParentSize.x * 0.5f;
                _Rect.y =   _ParentSize.y * 0.5f -_Rect.height * ( 0.5f - ( _Pivot.y - 0.5f ) ) + _ParentSize.y * 0.5f - _Rect.y;
                break;
            case    AR_TYPE.BOTTOM_LEFT:
                _Rect.x =   _Rect.x - _Rect.width * _Pivot.x + _ParentSize.x * 0.0f;
                _Rect.y =   _ParentSize.y * 0.5f -_Rect.height * ( 0.5f - ( _Pivot.y - 0.5f ) ) + _ParentSize.y * 0.5f - _Rect.y;
                break;
            case    AR_TYPE.BOTTOM_RIGHT:
                _Rect.x =   _Rect.x - _Rect.width * _Pivot.x + _ParentSize.x * 1.0f;
                _Rect.y =   _ParentSize.y * 0.5f -_Rect.height * ( 0.5f - ( _Pivot.y - 0.5f ) ) + _ParentSize.y * 0.5f - _Rect.y;
                break;
        }

        return  _Rect;
    }
    //**************************************************************************************************
    //      コンポーネントへのアクセスを取得
    //**************************************************************************************************
    public  static  TYPE    GetAccessComponent< TYPE >( string _SearchName ) where TYPE : Object
    {
        GameObject  rObj    =   GameObject.Find( _SearchName );

        if( rObj )  return  rObj.GetComponent< TYPE >();
        else        return  ( TYPE )null;
    }
    public  static  NetworkIdentity FindIdentityAtNetID( NetworkInstanceId _NetID )
    {
        NetworkIdentity rIdentity   =   null;

        //  オブジェクトを検索
        try{
            rIdentity   =   NetworkServer.objects[ _NetID ];
        }
        //  失敗したらnullを返す
        catch( System.Exception _Expection ){
            //  ダミー処理
            if( _Expection.Message.Length != 0 ){}
            return  null;
        }

        return  rIdentity;
    }
    //**************************************************************************************************
    //      コンポーネントへのアクセスを取得
    //**************************************************************************************************
}

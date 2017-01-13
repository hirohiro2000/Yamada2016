
using   UnityEngine;
using   System.Collections;
using   System.Collections.Generic;

public class EditBox_Control : MonoBehaviour {

    public  float       c_BaseScale         =   1.0f;
    public  bool        c_CombineBox        =   false;
    public  bool        c_CombineMesh       =   false;
    public  bool        c_CreateCollider    =   false;
    public  bool        c_SetMaterial       =   true;
    public  bool        c_SetUV             =   true;

    public  Material[]  c_Material          =   null;
    public  Material[]  c_MaterialCache     =   null;

	// Use this for initialization
	void    Start()
    {
        //  ポリゴンのクローンを作成
        if( c_CombineBox ){
            //  クローンの入れ物を作成
            GameObject  rCloneShell =   new GameObject();
            Transform   rCloneTrans =   rCloneShell.transform;
            rCloneTrans.position    =   transform.position;
            rCloneTrans.rotation    =   transform.rotation;
            rCloneTrans.parent      =   transform;
            rCloneShell.name        =   "CloneShell";

            //  クローンを作成
            Vector3     size        =   transform.localScale / c_BaseScale;
            for( int i = 0; i < transform.childCount - 1; i++ ){
                Vector2     uvScale =   GetUVScale( size, i );

                Transform   rChild  =   transform.GetChild( i );
                GameObject  rObj    =   CreatePolygon( rChild.position, rChild.eulerAngles, rChild.lossyScale, uvScale.x, uvScale.y );
                Transform   rTrans  =   rObj.transform;
                rTrans.parent       =   rCloneTrans;
                rTrans.name         =   rChild.name + "[ Clone ]";
            }

            //  マテリアルを設定
            int loopCount   =   Mathf.Min( c_Material.Length, transform.childCount );
            for( int i = 0; i < loopCount; i++ ){
                rCloneTrans.GetChild( i ).GetComponent< Renderer >().material =   c_Material[ i ];
            }

            //  クローン以外を破棄
            {
                rCloneTrans.parent  =   null;
                int     numLoop     =   transform.childCount;
                for( int i = 0; i < numLoop; i++ ){
                    DestroyImmediate( transform.GetChild( 0 ).gameObject );
                }

                numLoop   =   rCloneTrans.childCount;
                for( int i = 0; i < numLoop; i++ ){
                    rCloneTrans.GetChild( 0 ).parent    =   transform;
                }
                DestroyImmediate( rCloneTrans.gameObject );
            }

            //  メッシュを作成 
            CreateMesh();

            return;
        }

        //  マテリアル初期化
        if( c_SetMaterial ){
            SetMaterial();
        }

        //  ＵＶ調整
        if( c_SetUV ){
            Vector3 size    =   transform.localScale / c_BaseScale;
	        SetUV( ( int )size.x, ( int )size.y, ( int )size.z );
        }

        //  メッシュ最適化
        if( c_CombineMesh ){
            CreateMesh();
        }

        //  スクリプト無効化
        //this.enabled    =   false;
	}
	
    //  メッシュ作成  
    void        CreateMesh()
    {
        //  合体させるメッシュの数
        int numMesh =   transform.childCount;

        //  リストにまとめる
        List< Transform >   rTransList  =   new List< Transform >();
        for( int i = 0; i < transform.childCount; i++ ) rTransList.Add( transform.GetChild( i ) );

        //  合体させるためのデータを作成
        CombineInstance[]   combine     =   new CombineInstance[ rTransList.Count ];
        MeshRenderer[]      combRender  =   new MeshRenderer[ rTransList.Count ];
        for( int i = 0; i < combine.Length; i++ ){
            combine[ i ].mesh       =   rTransList[ i ].GetComponent< MeshFilter >().sharedMesh;
            combine[ i ].transform  =   rTransList[ i ].localToWorldMatrix;

            combRender[ i ]         =   rTransList[ i ].GetComponent< MeshRenderer >();
        }

        //  プレハブを作成
        MeshFilter      rFilter     =   gameObject.AddComponent< MeshFilter >();
        MeshRenderer    rRenderer   =   gameObject.AddComponent< MeshRenderer >();
        MeshCollider    rCollider   =   null;
        if( c_CreateCollider ){
            rCollider   =   gameObject.AddComponent< MeshCollider >();
        }

        //  準備開始
        for( int i = 0; i < rTransList.Count; i++ ){
            rTransList[ i ].gameObject.SetActive( false );
        }
        gameObject.SetActive( false );

        //  現在の配置情報を保存
        Vector3 prevPos     =   transform.position;
        Vector3 prevAngle   =   transform.eulerAngles;
        Vector3 prevScale   =   transform.localScale;

        //  配置情報初期化
        transform.position      =   Vector3.zero;
        transform.eulerAngles   =   Vector3.zero;
        transform.localScale    =   Vector3.one;

        //  マテリアルの数を調べる
        List< MeshRenderer >    rMatList    =   new List< MeshRenderer >();
        for( int i = 0; i < combRender.Length; i++ ){
            bool    isFounded   =   false;
            //  すでに発見されているマテリアルかどうかをチェック
            for( int m = 0; m < rMatList.Count; m++ ){
                if( combRender[ i ].material.name != rMatList[ m ].material.name )  continue;
        
                //  すでに発見されていた
                isFounded       =   true;
                break;
            }

            //  未知の材質だった場合は登録
            if( !isFounded ){
                rMatList.Add( combRender[ i ] );
            }
        }

        //  材質順にセットしなおす
        List< Transform >       rMTransList =   new List< Transform >();
        List< CombineInstance > rMCombList  =   new List< CombineInstance >();
        List< MeshRenderer >    rMRendList  =   new List< MeshRenderer >();
        for( int m = 0; m < rMatList.Count; m++ ){
            for( int i = 0; i < numMesh; i++ ){
                if( combRender[ i ].material.name != rMatList[ m ].material.name )  continue;

                //  リストに登録
                rMTransList.Add( rTransList[ i ] );
                rMCombList.Add( combine[ i ] );
                rMRendList.Add( combRender[ i ] );
            }
        }

        //  メッシュをひとつにまとめる
        {
            //  メッシュ作成
            rFilter.mesh                =   new Mesh();
            rFilter.mesh.name           =   name;
            rFilter.mesh.subMeshCount   =   numMesh;

            //  構成情報
            List< Vector3 > vertexList  =   new List< Vector3 >();
            List< Vector2 > uvList      =   new List< Vector2 >();
            List< int >     indexList   =   new List< int >();
            {
                int             indexOffset =   0;
                for( int m = 0; m < numMesh; m++ ){
                    Transform   rTrans  =   rMTransList[ m ];//rTransList[ m ];
                    Mesh        rMesh   =   rMCombList[ m ].mesh;//combine[ m ].mesh;

                    //  頂点
                    for( int i = 0; i < rMesh.vertices.Length; i++ ){
                        vertexList.Add( rTrans.TransformPoint( rMesh.vertices[ i ] ) );
                    }
                    //  ＵＶ
                    for( int i = 0; i < rMesh.uv.Length; i++ ){
                        uvList.Add( rMesh.uv[ i ] );
                    }
                    //  インデックス
                    for( int i = 0; i < rMesh.triangles.Length; i++ ){
                        indexList.Add( rMesh.triangles[ i ] + indexOffset );
                    }

                    //  インデックスをずらす
                    indexOffset +=  rMesh.vertexCount;
                }
            }

            //  構成情報ををセット
            rFilter.mesh.vertices   =   vertexList.ToArray();
            rFilter.mesh.uv         =   uvList.ToArray();
            rFilter.mesh.triangles  =   indexList.ToArray();

            //  構成情報計算
            rFilter.mesh.RecalculateNormals();
            rFilter.mesh.RecalculateBounds();

            //  サブメッシュの数
            rFilter.mesh.subMeshCount   =   rMatList.Count;//numMesh;

            //  コライダー設定
            if( rCollider ){
                rCollider.sharedMesh    =   rFilter.mesh;
            }

            //  サブメッシュの構成情報を設定 
            {
                int indexOffset     =   0;
                for( int mt = 0; mt < rMatList.Count; mt++ ){
                    //  サブメッシュ内の三角形リスト
                    List< int > triList =   new List< int >();

                    //  材質が一致する三角形をサブメッシュにまとめる
                    for( int m = 0; m < numMesh; m++ ){
                        //  材質チェック
                        if( rMRendList[ m ].material.name != rMatList[ mt ].material.name ) continue;

                        //  サブメッシュのインデックスリストを作成
                        Mesh        rMesh   =   rMCombList[ m ].mesh;
                        for( int i = 0; i < rMesh.triangles.Length; i++ ){
                            triList.Add( rMesh.triangles[ i ] + indexOffset );
                        }
                        //  インデックスをずらす
                        indexOffset     +=  rMesh.vertexCount;
                    }

                    //  サブメッシュ登録
                    rFilter.mesh.SetTriangles( triList.ToArray(), mt );
                }
            }

            //  マテリアルを生成 
            rRenderer.materials     =   new Material[ rMatList.Count ];

            //  マテリアルを設定 
            Material[]  rMaterials  =   rRenderer.materials;
            for( int m = 0; m < rMatList.Count; m++ ){
                Material    rMaterial   =   rMatList[ m ].material;//combRender[ m ].sharedMaterial;
                if( c_MaterialCache != null ){
                    for( int c = 0; c < c_MaterialCache.Length; c++ ){
                        if( c_MaterialCache[ c ] == null )                              continue;
                        if( rMaterial.name.IndexOf( c_MaterialCache[ c ].name ) < 0 )   continue;

                        rMaterial   =   c_MaterialCache[ c ];
                        break;
                    }
                }
                
                rMaterials[ m ]     =   rMaterial;
            }
            rRenderer.materials     =   rMaterials;
        }

        //  配置情報復元
        transform.localScale    =   prevScale;
        transform.eulerAngles   =   prevAngle;
        transform.position      =   prevPos;

        //  子オブジェクトを削除
        for( int i = 0; i < transform.childCount; i++ ){
            Destroy( transform.GetChild( i ).gameObject );
        }
        
        //  アクティブ化
        gameObject.SetActive( true );

        //  静的オブジェクトに戻す
        gameObject.isStatic =   true;
    }
    GameObject  CreatePolygon( Vector3 _Position, Vector3 _Angles, Vector3 _Scale, float _UVScaleX, float _UVScaleY )
    {
        //  メッシュ作成
        GameObject  rObj    =   new GameObject();
        Transform   rTrans  =   rObj.transform;
        Mesh        rMesh   =   new Mesh();
        rMesh.vertices      =   new Vector3[]{
            new Vector3(  0.5f,  0.5f ),
            new Vector3( -0.5f, -0.5f ),
            new Vector3( -0.5f,  0.5f ),
            new Vector3(  0.5f, -0.5f ),
        };
        rMesh.triangles     =   new int[]{
            0,  1,  2,
            0,  3,  1
        };
        rMesh.uv            =   new Vector2[]{
            new Vector2( 0.0f,              1.0f * _UVScaleY ),
            new Vector2( 1.0f * _UVScaleX,  0.0f ),
            new Vector2( 1.0f * _UVScaleX,  1.0f * _UVScaleY ),
            new Vector2( 0.0f,              0.0f ),
        };

        //  法線情報計算
        rMesh.RecalculateNormals();
        rMesh.RecalculateBounds();

        //  描画設定
        rObj.AddComponent< MeshFilter >().sharedMesh    =   rMesh;
        rObj.AddComponent< MeshRenderer >().material    =   c_Material[ 0 ];

        //  配置設定
        rTrans.position     =   _Position;
        rTrans.localScale   =   _Scale;
        rTrans.eulerAngles  =   _Angles;

        return  rObj;
    }
    void    SetUV( int _X, int _Y, int _Z )
    {
        _X  =   Mathf.Max( 1, _X );
        _Y  =   Mathf.Max( 1, _Y );
        _Z  =   Mathf.Max( 1, _Z );

        int[]   xAffectFace =   {   0,  1,  3,  5   };
        int[]   yAffectFace =   {   1,  2,  3,  4   };
        int[]   zAffectFace =   {   0,  2,  4,  5   };

        for( int i = 0; i < 6; i++ ){
            Vector2 uvScale =   Vector2.one;

            if( CheckInNumber( i, xAffectFace ) )   uvScale.x   +=  Mathf.Max( _X - 1, 0 );
            if( CheckInNumber( i, yAffectFace ) )   uvScale.y   +=  Mathf.Max( _Y - 1, 0 );
            if( CheckInNumber( i, zAffectFace ) ){
                if( i == 0 || i == 5 )              uvScale.y   +=  Mathf.Max( _Z - 1, 0 );
                else                                uvScale.x   +=  Mathf.Max( _Z - 1, 0 );
            }

            Renderer    rRenderer   =   transform.GetChild( i ).GetComponent< Renderer >();
            rRenderer.material.SetTextureScale( "_MainTex", uvScale );
        }
    }
    Vector2 GetUVScale( Vector3 _Size, int _FaceID )
    {
        _Size.x  =   Mathf.Max( 1, _Size.x );
        _Size.y  =   Mathf.Max( 1, _Size.y );
        _Size.z  =   Mathf.Max( 1, _Size.z );

        int[]   xAffectFace =   {   0,  1,  3,  5   };
        int[]   yAffectFace =   {   1,  2,  3,  4   };
        int[]   zAffectFace =   {   0,  2,  4,  5   };

        Vector2 uvScale =   Vector2.one;
        
        if( CheckInNumber( _FaceID, xAffectFace ) ) uvScale.x   +=  Mathf.Max( _Size.x - 1, 0 );
        if( CheckInNumber( _FaceID, yAffectFace ) ) uvScale.y   +=  Mathf.Max( _Size.y - 1, 0 );
        if( CheckInNumber( _FaceID, zAffectFace ) ){
            if( _FaceID == 0 || _FaceID == 5 )      uvScale.y   +=  Mathf.Max( _Size.z - 1, 0 );
            else                                    uvScale.x   +=  Mathf.Max( _Size.z - 1, 0 );
        }

        return  uvScale;
    }
    void    SetUVToMesh( int _X, int _Y, int _Z )
    {
        //  合体させるメッシュの数
        int numMesh =   transform.childCount;

        //  壁と斜面をリストにまとめる
        List< Transform >   rTransList  =   new List< Transform >();
        for( int i = 0; i < transform.childCount; i++ ) rTransList.Add( transform.GetChild( i ) );

        //  合体させるためのデータを作成 
        //CombineInstance[]   combine     =   new CombineInstance[ rTransList.Count ];
        //MeshRenderer[]      combRender  =   new MeshRenderer[ rTransList.Count ];
        //for( int i = 0; i < combine.Length; i++ ){
        //    combine[ i ].mesh       =   rTransList[ i ].GetComponent< MeshFilter >().sharedMesh;
        //    combine[ i ].transform  =   rTransList[ i ].localToWorldMatrix;

        //    combRender[ i ]         =   rTransList[ i ].GetComponent< MeshRenderer >();
        //}

        //  ＵＶ設定
        {
            Mesh            rMesh   =   rTransList[ 0 ].GetComponent< MeshFilter >().sharedMesh;
            List< Vector2 > uvList  =   new List< Vector2 >();

            
            
            
            
            
            
            
            
            
            uvList.Add( new Vector2( 1, 0 ) );
            uvList.Add( new Vector2( 0, 1 ) ); 
            uvList.Add( new Vector2( 0, 0 ) );
            uvList.Add( new Vector2( 1, 1 ) );

            //  設定 
            rMesh.SetUVs( 0, uvList );
        }
    }
    void    SetMaterial()
    {
        int loopCount   =   Mathf.Min( c_Material.Length, transform.childCount );
        for( int i = 0; i < loopCount; i++ ){
            transform.GetChild( i ).GetComponent< Renderer >().material =   c_Material[ i ];
        }
    }

    //  その他
    bool    CheckInNumber( int _CheckID, int[] _NumberList )
    {
        for( int i = 0; i < _NumberList.Length; i++ ){
            if( _CheckID == _NumberList[ i ] )  return  true;
        }

        return  false;
    }
}

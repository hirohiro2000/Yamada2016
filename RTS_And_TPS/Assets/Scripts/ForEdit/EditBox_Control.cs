
using   UnityEngine;
using   System.Collections;
using   System.Collections.Generic;

public class EditBox_Control : MonoBehaviour {

    public  float       c_BaseScale     =   1.0f;
    public  bool        c_CombineMesh   =   false;
    public  bool        c_SetMaterial   =   true;
    public  bool        c_SetUV         =   true;

    public  Material[]  c_Material      =   null;

	// Use this for initialization
	void    Start()
    {
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
    void    CreateMesh()
    {
        //  合体させるメッシュの数
        int numMesh =   transform.childCount;

        //  壁と斜面をリストにまとめる
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
                    Transform   rTrans  =   rTransList[ m ];
                    Mesh        rMesh   =   combine[ m ].mesh;

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
            rFilter.mesh.subMeshCount   =   numMesh;

            //  サブメッシュの構成情報を設定 
            {
                int indexOffset     =   0;
                for( int m = 0; m < numMesh; m++ ){
                    //  サブメッシュのインデックスリストを作成
                    Mesh        rMesh   =   combine[ m ].mesh;
                    List< int > triList =   new List< int >();
                    for( int i = 0; i < rMesh.triangles.Length; i++ ){
                        triList.Add( rMesh.triangles[ i ] + indexOffset );
                    }

                    //  サブメッシュ登録
                    rFilter.mesh.SetTriangles( triList.ToArray(), m );

                    //  インデックスをずらす
                    indexOffset     +=  rMesh.vertexCount;
                }
            }

            //  共有メッシュに設定
            rFilter.sharedMesh      =   rFilter.mesh;

            //  マテリアルを設定
            rRenderer.materials     =   new Material[ numMesh ];
            for( int m = 0; m < numMesh; m++ ){
                rRenderer.materials[ m ].CopyPropertiesFromMaterial( combRender[ m ].material );
            }
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

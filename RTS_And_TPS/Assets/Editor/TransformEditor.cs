
using   UnityEditor;
using   UnityEngine;
using   System.Collections;
using   System.Collections.Generic;

public class TransformEditor : MonoBehaviour {

    [ MenuItem( "Tools/SetCenterPosition" ) ]
    static  void    SetCenterPosition_ToTransform()
    {
        for( int i = 0; i < Selection.gameObjects.Length; i++ ){
            GameObject  rObj            =   Selection.gameObjects[ i ];
            Transform   rTrans          =   rObj.transform;
            MeshFilter  rFilter         =   rObj.GetComponent< MeshFilter >();
            if( !rFilter )  continue;

            bool            isStatic    =   rObj.isStatic;
            if( !isStatic ){
                rObj.isStatic   =   true;
            }

            Mesh            rMesh       =   rFilter.sharedMesh;
            Vector3         curCenter   =   rMesh.bounds.center;
            Vector3[]       rVtx        =   rMesh.vertices;
            List< Vector3 > vtxList     =   new List< Vector3 >();

            for( int v = 0; v < rVtx.Length; v++ ){
                vtxList.Add( rVtx[ v ] - curCenter );
            }

            //  メッシュの頂点を原点に移動
            rMesh.SetVertices( vtxList );

            //  元の中心位置に移動
            Vector3 worldCenter =   rTrans.TransformPoint( curCenter );
            rTrans.position     =   worldCenter;

            //  静止状態を元に戻す
            rObj.isStatic       =   isStatic;
        }
    }

    //  一番目に選択されているオブジェクトが持つメッシュの中心座標を、二番目に選択されているオブジェクトに設定
    [ MenuItem( "Tools/SetCenterPos_1To2Select" ) ]
    static  void    SetCenterPosition_1To2()
    {
        if( Selection.gameObjects.Length < 2 )  return;

        GameObject  rSource =   Selection.gameObjects[ 0 ];
        GameObject  rDest   =   Selection.gameObjects[ 1 ];

        Transform   rSTrans =   rSource.transform;
        MeshFilter  rFilter =   rSource.GetComponent< MeshFilter >();
        if( !rFilter )                          return;

        Vector3     centerL =   rFilter.sharedMesh.bounds.center;
        Vector3     centerW =   rSTrans.TransformPoint( centerL );

        //  座標をセット
        rDest.transform.position    =   centerW;
    }
}

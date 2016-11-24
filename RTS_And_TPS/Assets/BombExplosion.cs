using UnityEngine;
using System.Collections;

public class BombExplosion : MonoBehaviour {
    public  int         c_AttackerID    =   0;
    public  GameObject  c_Explosion     =   null;

    void    OnDestroy()
    {
        //  爆発オブジェクト生成 
        GameObject  rObj    =   Instantiate( c_Explosion );
        Transform   rTrans  =   rObj.transform;
        
        rTrans.position     =   transform.position;
        rTrans.rotation     =   transform.rotation;

        //  あたり判定へのアクセス
        Transform   rCol    =   rTrans.FindChild( "AttackArea" );
        //if( !rCol ) return;
        
        //  識別番号設定
        TPSAttack_Net   rAtkNet =   rCol.GetComponent< TPSAttack_Net >();
        rAtkNet.c_AttackerID    =   c_AttackerID;

        //  あたり判定のフレーム数を設定
        ExplosionAttack rEXATK  =   rCol.GetComponent< ExplosionAttack >();
        rEXATK.c_DestroyCounter =   1;

        //  オブジェクトをアクティベート
        rObj.SetActive( true );
    }
}

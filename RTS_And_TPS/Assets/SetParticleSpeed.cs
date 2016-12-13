using UnityEngine;
using System.Collections;

public class SetParticleSpeed : MonoBehaviour {
    public  float   PlaySpeed   =   1.0f;

    void    Awake()
    {
        //  コンポーネント取得
        ParticleSystem      rParticle   =   GetComponent< ParticleSystem >();
        if( !rParticle )    return;

        //  速度を設定
        rParticle.playbackSpeed     =   PlaySpeed;

        //  処理を終了
        this.enabled    =   false;
    }
}

using UnityEngine;
using System.Collections;

public class SoundPlay_Control : MonoBehaviour {
    public  enum    PlayType{
        Play,
        OneShot,
    };
    public  enum    PlayMode{
        Start,
        Destroy,
    };

    public  string      c_SEName        =   null;
    public  PlayType    c_PlayType      =   PlayType.OneShot;
    public  PlayMode    c_PlayMode      =   PlayMode.Start;
    public  float       c_VolumeRatio   =   1.0f;
    public  float       c_PitchRatio    =   1.0f;

    public  float       c_LifeTime      =   0.0f;
    public  bool        c_ParentMe      =   false;
    
	void    Start()
    {
        if( c_PlayMode == PlayMode.Start ){
            Play();
        }
	}
    void    OnDestroy()
    {
        if( c_PlayMode == PlayMode.Destroy ){
            Play();
        }
    }
    void    Play()
    {
        SoundController     rCountrol   =   SoundController.Create( c_SEName, ( c_ParentMe )? transform : null );
        if( !rCountrol )    return;

        //  再生パラメータ設定
        rCountrol.m_audioSource.volume  *=  c_VolumeRatio;
        rCountrol.m_audioSource.pitch   *=  c_PitchRatio;

        //  座標設定
        rCountrol.transform.position    =   transform.position;
        //  破棄設定
        Destroy( rCountrol.gameObject, c_LifeTime );

        //  再生
        switch( c_PlayType ){
            case    PlayType.Play:      rCountrol.Play();           break;
            case    PlayType.OneShot:   rCountrol.PlayOneShot();    break;
        }

        //  スクリプト無効化
        this.enabled    =   false;
    }
}

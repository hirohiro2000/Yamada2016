using UnityEngine;
using System.Collections;
using System.Collections.Generic;



[RequireComponent(typeof(AudioSource))]
public class SoundController : MonoBehaviour {

    public AudioSource m_audioSource;
    public AudioClip m_clip
    {
        get { return m_audioSource.clip; }
        set { m_audioSource.clip = value; }
    }

    public SoundController()
    {
        m_audioSource = GetComponent<AudioSource>();
    }

    public void Play()
    {
        m_audioSource.Play();
    }
    public void PlayDelay( float _Delay )
    {
        m_audioSource.PlayDelayed( _Delay );
    }
    public void Stop()
    {
        m_audioSource.Stop();
    }
    public bool isPlaying()
    {
        return m_audioSource.isPlaying;
    }

    // SE用
    public void PlayOneShot()
    {
        m_audioSource.PlayOneShot(m_clip);
    }

    // スタティック
    static GameObject referenceAudio = null;

    static public SoundController Create(string name,Transform parent)
    {
        if (referenceAudio == null)
        {
            referenceAudio = GameObject.Find("Audio");
        }

        if ( referenceAudio == null ) return null;

        Transform trans = referenceAudio.transform.FindChild(name);
        if ( trans == null )
            Debug.LogError( "サウンドの読み込みに失敗しました[FileName: "+name+" ]" );



        GameObject gameObj = Instantiate( trans.gameObject );
        gameObj.transform.parent = parent;
        SoundController controller = gameObj.AddComponent<SoundController>();
        controller.m_audioSource.playOnAwake = false;
        return controller;

    }

    static public SoundController PlayNow( string _Name, float _Delay, float _Volume, float _Pitch, float _DestroyTime )
    {
        return  PlayNow( _Name, null, Vector3.zero, _Delay, _Volume, _Pitch, _DestroyTime );
    }
    static public SoundController PlayNow( string _Name, Transform _rParent, Vector3 _Pos, float _Delay, float _Volume, float _Pitch, float _DestroyTime )
    {
        SoundController rControl    =   Create( _Name, _rParent );
        if( !rControl ) return  null;

        rControl.transform.position     =   _Pos;

        rControl.m_audioSource.volume   =   _Volume;
        rControl.m_audioSource.pitch    =   _Pitch;
        rControl.m_audioSource.PlayDelayed( _Delay );

        if( _DestroyTime >= 0.0f ){
            Destroy( rControl.gameObject, _Delay + _DestroyTime );
        }

        return  rControl;
    }

    // ゲームOnly
    static public SoundController CreateShotController(Transform parent = null)
    {
        SoundController controller = Create("Shot0", parent);
        return controller;
    }


}

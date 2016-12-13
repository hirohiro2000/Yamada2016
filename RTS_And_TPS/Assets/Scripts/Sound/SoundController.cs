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
        GameObject gameObj = Instantiate( trans.gameObject );
        gameObj.transform.parent = parent;
        SoundController controller = gameObj.AddComponent<SoundController>();
        controller.m_audioSource.playOnAwake = false;
        return controller;

    }

    // ゲームOnly
    static public SoundController CreateShotController(Transform parent = null)
    {
        SoundController controller = Create("Shot0", parent);
        return controller;
    }


}

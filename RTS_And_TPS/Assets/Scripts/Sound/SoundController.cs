using UnityEngine;
using System.Collections;
using System.Collections.Generic;



[RequireComponent(typeof(AudioSource))]
public class SoundController : MonoBehaviour {

    AudioSource m_audioSource;
    AudioClip m_clip
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
    static public SoundController Create(Transform parent)
    {
        GameObject gameObj = new GameObject();
        gameObj.name    = "SoundController";
        gameObj.transform.parent = parent;
        SoundController controller = gameObj.AddComponent<SoundController>();
        controller.m_audioSource.playOnAwake = false;
        return controller;
    }

    // ゲームOnly
    static public  SoundController CreateShotController(Transform parent = null)
    {
        SoundController controller = Create(parent);
        controller.m_clip = Resources.Load<AudioClip>("Sounds/Designed_Game_Gun_Shot_3");
        return controller;
    }


}

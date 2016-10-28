using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour
{
    // member
    [SerializeField]
    private List<AudioClip>     m_clip   = null;

    private List<AudioSource>   m_sounds = null;

    private static SoundManager instance = null;

    void Awake()
    {
        instance = GetComponent<SoundManager>();

        AudioClip seShot = Resources.Load("Sounds/sen_ge_kijuu01") as AudioClip;
        m_clip.Add(seShot);

    }

    static public int FindOfIndex(string clipName)
    {
        int num = instance.m_clip.Count;
        for (int i = 0; i < instance.m_clip.Count; i++)
        {
            if (clipName == instance.m_clip[i].name)
                return i;
        }
        return -1;
    }

    static public GameObject Create(int srcID, Vector3 posision)
    {
        GameObject go = new GameObject();
        go.transform.position = posision;
        go.transform.parent = instance.transform;
        go.AddComponent<AudioSource>();

        AudioSource src = go.GetComponent<AudioSource>();
        src.clip = instance.m_clip[srcID];
        src.Play();

        return go;
    }


}




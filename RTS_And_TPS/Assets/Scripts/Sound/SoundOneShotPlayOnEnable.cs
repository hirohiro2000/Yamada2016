using UnityEngine;
using System.Collections;

public class SoundOneShotPlayOnEnable : MonoBehaviour {

    [SerializeField]
    private string m_seFileName = "";

    SoundController m_se = null;

    // Use this for initialization
    void OnEnable()
    {
        if (m_se == null)
        {
            m_se = SoundController.Create(m_seFileName, transform);
        }
        if (m_se != null)
        {
            m_se.Play();
        }
    }

}

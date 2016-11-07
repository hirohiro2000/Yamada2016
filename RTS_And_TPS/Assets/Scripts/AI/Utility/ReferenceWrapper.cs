using UnityEngine;
using System.Collections;

public class ReferenceWrapper : MonoBehaviour {

    public GameObject m_home_base { get; private set; }

    void Awake()
    {
        m_home_base = GameObject.Find("Homebase");
    }

}

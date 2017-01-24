using UnityEngine;
using System.Collections;

public class StaffCredit : MonoBehaviour {

    public delegate void EndCredit();

    EndCredit m_endnotify_function = null;

    // Use this for initialization
    void Start()
    {
        gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) ||
            Input.GetMouseButtonDown(1))
            m_endnotify_function();
    }

    public void SetCreditEndNotifyFunction(EndCredit func)
    {
        m_endnotify_function = func;
    }
}

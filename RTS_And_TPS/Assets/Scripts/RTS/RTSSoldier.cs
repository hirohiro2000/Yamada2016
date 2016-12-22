using UnityEngine;
using System.Collections;

public class RTSSoldier : MonoBehaviour,
    IRTSSoldierHandler
{

    NavMeshAgent m_agent    = null;
    bool         m_isForcus = false;

	// Use this for initialization
	void Start ()
    {
        m_agent = GetComponent<NavMeshAgent>();
	    m_agent.SetDestination(Vector3.zero);
         
        Transform   rHUD = GameObject.Find( "Canvas" ).transform.FindChild("RTS_HUD");
        rHUD.GetComponent<RTSCommander>().AddSoldier(this.gameObject);
	}
	
	// Update is called once per frame
	void Update ()
    {
	    if ( m_isForcus == false )  return;

	}

    public void OnCommanderClick()
    {
        m_isForcus = !m_isForcus;
        m_agent.enabled = m_isForcus;
        if (m_agent.enabled)
        {
            GameObject[] objList = GameObject.FindGameObjectsWithTag("Player");
            m_agent.SetDestination(objList[0].transform.position);
        }
    }

}

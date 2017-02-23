using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class DistanceEvent : MonoBehaviour {

	[SerializeField]
	UnityEvent m_Events = new UnityEvent();

	public float m_Distance = .0f;

	public bool m_Reverse = false;

	public bool m_OneFrame = false;

	public bool m_IgnoreX = false;

	public bool m_IgnoreY = false;

	public bool m_IgnoreZ = false;

	public bool m_Once = false;

	LinkManager m_rLinkManager = null;

	bool m_beforeFlag = false;
	bool m_Flag = false;

	// Use this for initialization
	void Start () {
		m_rLinkManager  =   FunctionManager.GetAccessComponent< LinkManager >( "LinkManager" );
	}
	
	// Update is called once per frame
	void Update () {
		m_Flag = false;

		Vector3 vec = ( transform.position - m_rLinkManager.m_rLocalPlayer.transform.position );

		if(m_IgnoreX == true)
			vec.x = .0f;

		if(m_IgnoreY == true)
			vec.y = .0f;

		if(m_IgnoreZ == true)
			vec.z = .0f;


		float distance = vec.sqrMagnitude;

		distance = distance - (m_Distance * m_Distance);

		if(m_Reverse == true)
			distance = -distance;

		if(distance < .0f)
		{
			if(m_OneFrame == false)
			{
				m_Events.Invoke();
				if(m_Once == true)
					enabled = false;
			}

			else
				m_Flag = true;
		}
		if(m_OneFrame == true)
		{
			if(m_Flag == true && m_beforeFlag == false)
			{
				m_Events.Invoke();
				if(m_Once == true)
					enabled = false;
			}
		}
		m_beforeFlag = m_Flag;
	}
}

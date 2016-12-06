using UnityEngine;
using System.Collections;

public class GirlDroneSwitcher : MonoBehaviour
{
	private Camera				m_rtsCamera					= null;
	private Camera				m_droneCamera				= null;
	private DroneCamera			m_script					= null;
	private GameObject			m_drone						= null;

	// Use this for initialization
	void Start ()
	{
		m_rtsCamera		= GameObject.Find("RTSCamera_Shell").transform.FindChild("RTSCamera").GetComponent<Camera>();
		m_droneCamera	= GameObject.Find("DroneCamera").GetComponent<Camera>();
		m_script		= GameObject.Find("DroneCamera").GetComponent<DroneCamera>();
	}
	
	// Update is called once per frame
	void Update ()
	{				
	}

	public void On( GameObject drone )
	{
		m_rtsCamera.enabled		= false;
		m_droneCamera.enabled	= true;
		m_script.m_target		= drone.transform;
		m_drone					= drone;
	}
	public void Off()
	{
		m_rtsCamera.enabled		= true;
		m_droneCamera.enabled	= false;
		m_script.m_target		= null;
		Destroy( m_drone );
	}
}

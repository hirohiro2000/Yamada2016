using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ResourceFactory : MonoBehaviour 
{
	private GameObject 			m_mechanicalGirl 	= null;
	public 	GameObject 			m_barricade 		= null;

	private GameObject 			m_resourceParent 	= null;
	private List<GameObject> 	m_resources 		= null;

	private enum State
	{
		Common,
		PutResourseReady,
	};

	//private State 				m_actionState 		= State.Common;


	void Start () 
	{	
		m_mechanicalGirl 		= GameObject.Find ("MechanicalGirl");

		m_resourceParent 		= new GameObject();
		m_resourceParent.name 	= "ResourceParent";

		m_resources 			= new List<GameObject>();
	}
	
	void Update () 
	{
		AddResource ();
	}

	void AddResource()
	{
		if (!Input.GetKeyDown (KeyCode.Space)) 
			return;


		GameObject 	add 		= Instantiate( m_barricade );
		Vector3 	forward 	= m_mechanicalGirl.transform.forward;
		float 		dist 		= 3.0f;

		add.transform.parent 	= m_resourceParent.transform;
		add.transform.position 	= m_mechanicalGirl.transform.position + forward * dist;
		
		m_resources.Add( add );
	}
}

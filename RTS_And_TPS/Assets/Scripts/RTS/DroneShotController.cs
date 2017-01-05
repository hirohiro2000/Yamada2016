using UnityEngine;
using System.Collections;

public class DroneShotController : MonoBehaviour
{
	[SerializeField]
	private GameObject m_gun;

	[SerializeField]
	private KeyCode m_fireKey = KeyCode.J;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
	{
		if( Input.GetKey( m_fireKey ))
		{
			Instantiate( m_gun, transform.position, transform.rotation );
		}
	}

    public Object Fire()
    {
        Object obj = Instantiate( m_gun, transform.position, transform.rotation );
        return obj;
    }

}

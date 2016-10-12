using UnityEngine;
using System.Collections;

public class Pauser : MonoBehaviour
{
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	}

	public void Enable( bool enable )
	{
		MonoBehaviour[] monos = GetComponents<MonoBehaviour>();
		
		for( int i=0; i<monos.Length; ++i )
		{
			monos[i].enabled = enable;
		}
	}
}

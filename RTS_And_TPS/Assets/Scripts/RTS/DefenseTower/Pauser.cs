using UnityEngine;
using System.Collections;

public class Pauser : MonoBehaviour
{
	public void Enable( bool enable )
	{
		MonoBehaviour[] monos = GetComponents<MonoBehaviour>();
		
		for( int i=0; i<monos.Length; ++i )
		{
			monos[i].enabled = enable;
		}
	}
}

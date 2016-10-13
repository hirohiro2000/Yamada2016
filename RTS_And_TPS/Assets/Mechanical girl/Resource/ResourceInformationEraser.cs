using UnityEngine;
using System.Collections;

public class ResourceInformationEraser : MonoBehaviour
{
	// Use this for initialization
	void Start ()
	{
	}
	
	// Update is called once per frame
	void Update ()
	{
	}

	void OnDestroy()
	{
		var g = GameObject.Find("ResourceInformation");

		if ( g )
			g.GetComponent<ResourceInformation>().SetGridInformation( null, transform.position, false );
	}
}

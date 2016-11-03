using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ResourceCommon : MonoBehaviour
{
	// Use this for initialization
	void Start ()
	{
	}

	//
	void OnDestroy()
	{
		var g = GameObject.Find("ResourceInformation");

		if ( g )
			g.GetComponent<ResourceInformation>().SetGridInformation( null, transform.position, false );
	}
	
	// Update is called once per frame
	void Update ()
	{
	}
}

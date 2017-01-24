using UnityEngine;
using System.Collections;

public class TitleButton : MonoBehaviour {

    GameObject safe_image = null;

	// Use this for initialization
	void Start ()
    {
        safe_image = transform.GetChild(0).gameObject;
        safe_image.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Execute()
    {
        safe_image.SetActive(!safe_image.activeInHierarchy);
    }
}

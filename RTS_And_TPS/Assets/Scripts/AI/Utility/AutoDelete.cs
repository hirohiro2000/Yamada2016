using UnityEngine;
using System.Collections;

public class AutoDelete : MonoBehaviour {

    [SerializeField]
    float DeleteSecond = 10.0f;

    IEnumerator Delete()
    {
        yield return new WaitForSeconds(DeleteSecond);
        Destroy(gameObject);
    }

	// Use this for initialization
	void Start () {
        StartCoroutine(Delete());
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

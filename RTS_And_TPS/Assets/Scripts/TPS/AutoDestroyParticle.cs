using UnityEngine;
using System.Collections;

public class AutoDestroyParticle : MonoBehaviour {
	[SerializeField]
	ParticleSystem destroyParticle = null;
	// Use this for initialization
	void Start () {
		Destroy(this.gameObject,destroyParticle.duration);
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HealthBar3D : MonoBehaviour {

	Slider _slider;

	Slider slider
	{
		get
		{
			if(_slider == null)
				_slider = GetComponent<Slider>();
			return _slider;
		}
	}
	
	// Use this for initialization
	void Start () {

    }
	
	// Update is called once per frame
	void Update () {
        if( !Camera.main )  return;

		transform.rotation = Camera.main.transform.rotation;
    }

	public void setValue(float value)
	{
		slider.value = value;
    }
}

using UnityEngine;
using System.Collections;

public class LightIntensityRandamize : MonoBehaviour {


    [SerializeField,Range(-1.0f,1.0f)]
    private float MaxShakeingWidth = 0.1f;

    [SerializeField, Range(-1.0f, 1.0f)]
    private float MinShakingWidth = -0.1f;

    private Light m_owner_light = null;
    private float m_current_intensity = .0f;
    private float m_default_intensity = .0f;
    private bool m_is_active = true;


	// Use this for initialization
	void Start () {
        m_owner_light = GetComponent<Light>();
        m_default_intensity = m_owner_light.intensity;
        m_current_intensity = m_default_intensity;
        StartCoroutine(UpdateIntensity());
	}
	
    IEnumerator UpdateIntensity()
    {
       while(m_is_active)
        {
            m_current_intensity = m_default_intensity * (1.0f + UnityEngine.Random.Range(-MinShakingWidth, MaxShakeingWidth));
            m_owner_light.intensity = m_current_intensity;
            yield return new WaitForSeconds(UnityEngine.Random.Range(.1f, 0.4f));
        } 
    }

    void OnDestroy()
    {
        m_is_active = false;
    }

	// Update is called once per frame
	void Update () {
        //m_owner_light.intensity = m_current_intensity;
    }
}

using UnityEngine;
using System.Collections;

public class ResourceAppear : MonoBehaviour
{
	[SerializeField]
	AnimationCurve height = null; 
    
	public float originHeight = 0.0f;

    [SerializeField]
    float timer = 0.0f;

    [SerializeField]
    float timeSpeed = 1.0f;

    void Start()
    {
        timer = 0.0f;
        originHeight = transform.localPosition.y;
    }
    void Update()
    {
        Vector3 localHeight = transform.localPosition;
        localHeight.y = originHeight + height.Evaluate( timer );

        transform.localPosition = localHeight;

        timer += timeSpeed*Time.deltaTime;
        this.enabled = ( timer <= 1.0f );
    }
    public void End()
    {
        timer = 1.0f;
    }


}
